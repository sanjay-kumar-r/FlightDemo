import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIRequestType, HeaderInfo } from 'src/Models/HeaderInfo';
import { Users } from 'src/Models/Users';
import { ApiExecutorService } from './api-executor.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl:string;
  registerUrl:string;
  loginUrl:string;
  updateUserUrl:string;
  updateAsAdminUrl:string;
  deleteUserUrl:string;

  getTokenUrl:string;
  refreshTokenUrl :string;

  apiExecutor:ApiExecutorService;

  constructor(apiExecutor:ApiExecutorService) { 
    this.baseUrl = "http://localhost:9050";
    this.registerUrl = this.baseUrl.trim() + '/' + "api/v1.0/Users/Register";
    this.loginUrl = this.baseUrl.trim() + '/' + "api/v1.0/Users/Login";
    this.updateUserUrl = this.baseUrl.trim() + '/' + "api/v1.0/Users/Update";
    this.updateAsAdminUrl = this.baseUrl.trim() + '/' + "api/v1.0/Users/UpdateUserAsSuperAdmin";
    this.deleteUserUrl = this.baseUrl.trim() + '/' + "api/v1.0/Users/Delete";

    this.getTokenUrl = "api/AuthTokens/GetAuthToken";
    this.refreshTokenUrl = "api/AuthTokens/RefreshToken";

    this.apiExecutor = apiExecutor;
  }

  registerUser_Test(user:Users, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPIWithRetry(APIRequestType.Post, this.registerUrl, headerInfo ,
        user, this.refreshTokenUrl, false);
  }

  LoginUser_Test(user:Users, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPIWithRetry(APIRequestType.Post, this.loginUrl, headerInfo ,
      user, this.refreshTokenUrl, false);
  }

  LoginUser(user:Users, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.loginUrl, headerInfo ,
      user, false);
  }

  registerUser(user:Users, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.registerUrl, headerInfo ,
        user, false);
  }
  

}
