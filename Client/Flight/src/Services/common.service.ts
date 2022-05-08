import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIRequestType, HeaderInfo } from 'src/Models/HeaderInfo';
import { RefreshTokenRequest } from 'src/Models/Users';
import { ApiExecutorService } from './api-executor.service';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  baseUrl:string;

  getTokenUrl:string;
  refreshTokenUrl :string;

  constructor(private apiExecutor:ApiExecutorService) {
    this.baseUrl = "http://localhost:9050";

    this.getTokenUrl = this.baseUrl.trim() + '/' + "api/AuthTokens/GetAuthToken";
    this.refreshTokenUrl = this.baseUrl.trim() + '/' + "api/AuthTokens/RefreshToken";
  }

  refreshToken(headerInfo:HeaderInfo) : Observable<any>
  {
    let response:any|null = null;
    let refreshTokenBody:RefreshTokenRequest = 
    {
        RefreshToken: headerInfo.RefreshToken ?? "",
        ExpiredToken: headerInfo.Authorization?.split(" ").pop() ?? ""
    }; 
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.refreshTokenUrl, headerInfo ,
      refreshTokenBody, true);
  }
}
