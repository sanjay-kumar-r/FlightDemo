import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {HeaderInfo, APIRequestType} from '../Models/HeaderInfo';
import {AuthRequest, AuthResponse, RefreshTokenRequest} from '../Models/Users';
import { Observable, throwError, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ApiExecutorService {

  httpClient:HttpClient;

  constructor(httpClient:HttpClient) { this.httpClient = httpClient;}

  CallAPIWithRetry(requestType:APIRequestType, requestUrl:string, headerInfo:HeaderInfo, 
    requestBody:any|null = null, refreshTokenUrl:string|null = null,
     isAuthorizatonRequired:boolean = false) : Observable<any>
  {
    let response:any|null;
    try
    {
      this.CallAPI(requestType, requestUrl, headerInfo, requestBody, isAuthorizatonRequired)
        .subscribe((result) => {
          response = result;
        }, (e) => {
          if(isAuthorizatonRequired 
            && refreshTokenUrl != undefined && refreshTokenUrl != null 
            && headerInfo.RefreshToken != undefined && headerInfo.RefreshToken != null
            && e.status == 401)
          {
            //retry
            this.RefreshTokenAndCallAPI(requestType, requestUrl, headerInfo, requestBody,
              refreshTokenUrl ?? "", isAuthorizatonRequired).subscribe(
               (result) => {
                 response = result;
               }, (error) => {
                console.error(error);
                alert("session expired");
                localStorage.clear();
                throw error;
               });
          }
          else
          {
            console.error(e);
            alert("session expired");
            localStorage.clear();
            throw e;
          }
      });
    }
    catch(e:any)
    {
      if(isAuthorizatonRequired 
        && refreshTokenUrl != undefined && refreshTokenUrl != null 
        && headerInfo.RefreshToken != undefined && headerInfo.RefreshToken != null
        && e.status == 401)
      { 
        //retry
        this.RefreshTokenAndCallAPI(requestType, requestUrl, headerInfo, requestBody,
           refreshTokenUrl ?? "", isAuthorizatonRequired).subscribe(
            (result) => {
              response = result;
            }, (error) => {
             console.error(error);
             alert("session expired");
             localStorage.clear();
             throw error;
            });
      }
      else{
        console.error(e);
        alert("session expired");
        localStorage.clear();
        throw e;
      }
    }
    return response;
  }

  CallAPI(requestType:APIRequestType, requestUrl:string, headerInfo:HeaderInfo, 
    requestBody:any|null = null, isAuthorizatonRequired:boolean = false) : Observable<any>
  {
    let response:any;
    let headers= new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*');
      if((headerInfo ?? null) != null)
      {
        if((headerInfo.UserId ?? null) != null)
          headers = headers.set('UserId', headerInfo.UserId);
        if((headerInfo.TenantId ?? null) != null)
          headers = headers.set('TenantId', headerInfo.TenantId ?? "");
        if(isAuthorizatonRequired)
        {
          if((headerInfo.Authorization ?? null) != null)
            headers = headers.set('Authorization', `${headerInfo.Authorization}` );
          if((headerInfo.RefreshToken ?? null) != null)  
            headers = headers.set('RefreshToken', `${headerInfo.RefreshToken}` );
        }
      }

    switch(requestType)
    {
      case APIRequestType.Get:
        {
          response = this.httpClient.get(requestUrl, {'headers' : headers});
          break;
        }
      case APIRequestType.Post:
        {
          response = this.httpClient.post(requestUrl, requestBody, {'headers' : headers});
          break;
        }
      case APIRequestType.Delete:
        {
          response = this.httpClient.delete(requestUrl, {'headers' : headers});
          break;
        }
    }
    return response;
  }

  RefreshTokenAndCallAPI_Test(requestType:APIRequestType, requestUrl:string, headerInfo:HeaderInfo, 
    requestBody:any|null = null, refreshTokenUrl:string, isAuthorizatonRequired:boolean)
    : Observable<any>
  {
    let response:any|null = null;
    let refreshTokenBody:RefreshTokenRequest = 
    {
        RefreshToken: headerInfo.RefreshToken ?? "",
        ExpiredToken: headerInfo.Authorization?.split(" ").pop() ?? ""
    };

    this.CallAPI(requestType, refreshTokenUrl, headerInfo, refreshTokenBody, isAuthorizatonRequired)
      .subscribe((result) => {
        let refreshResult:AuthResponse = result;
        if(refreshResult != undefined && refreshResult != null && refreshResult.IsSuccess)
        {
          localStorage.setItem('Token', refreshResult.Token ?? "" );
          headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
          response = this.CallAPI(requestType, requestUrl, headerInfo, requestBody, isAuthorizatonRequired);
        }
      },
      (e) => {
          console.error(e);
          alert("session expired");
          localStorage.clear();
          throw e;
      });
    return response;
  }

  RefreshTokenAndCallAPI(requestType:APIRequestType, requestUrl:string, headerInfo:HeaderInfo, 
    requestBody:any|null = null, refreshTokenUrl:string, isAuthorizatonRequired:boolean)
    : Observable<any>
  {
    let response:any|null = null;
    let refreshTokenBody:RefreshTokenRequest = 
    {
        RefreshToken: headerInfo.RefreshToken ?? "",
        ExpiredToken: headerInfo.Authorization?.split(" ").pop() ?? ""
    };

    return this.CallAPI(requestType, refreshTokenUrl, headerInfo, refreshTokenBody, isAuthorizatonRequired)
      .pipe(
        map((authResponse) => {
          let refreshResult:AuthResponse = {
            IsSuccess: authResponse.isSuccess,
            Token : authResponse.token,
            RefreshToken : authResponse.refreshToken,
            Reason: authResponse.reason
          };
          // if(refreshResult != undefined && refreshResult != null && refreshResult.IsSuccess)
          // {
            //localStorage.setItem('Token', refreshResult.Token ?? "" );
            localStorage.setItem("authResponse", JSON.stringify(refreshResult));
            headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
            return this.CallAPI(requestType, requestUrl, headerInfo, requestBody, isAuthorizatonRequired);
          //}
        })
      );
  }

}
