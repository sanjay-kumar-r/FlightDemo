import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import {UserService} from '../../Services/user.service';
import { ActivatedRoute } from '@angular/router';
import { CustomErrorCode } from 'src/Models/Commons';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  alert?:{type:string|null, message:string|null}|null;

  router:Router;
  userService:UserService;
  loginSuccess:boolean;
  route:ActivatedRoute;

  constructor(router:Router, userService:UserService, route:ActivatedRoute) {
    this.router = router;
    this.userService = userService;
    this.loginSuccess = false;
    this.route = route;
   }

  ngOnInit(): void {
    this.alert = null;
  }

  closeAlert() {
    this.alert = null;
  }

  redirectToRegister()
  {
    //this.router.navigateByUrl("users/register");
    this.router.navigate(['users/register']);
    //this.router.navigate(['/register'], { relativeTo: this.route });
  }

  loginUser(loginFormData:any)
  {
    if((loginFormData ?? null) != null && (loginFormData.emailId ?? null) != null 
      && (loginFormData.password ?? null) != null)
    {
      let headerInfo:HeaderInfo|any = null;
      let user:Users = {
        EmailId : loginFormData.emailId,
        Password : loginFormData.password
      }
      console.log(loginFormData);
      console.log(headerInfo);
      console.log(user);
      this.userService.LoginUser(user, headerInfo).subscribe(
        (response) =>{
          //console.log("response :=>" ,response);
          if((response ?? null) != null)
          {
            localStorage.setItem("userId", response.user.id);
            localStorage.setItem("userDetails", JSON.stringify(response.user));
            let authResponse:any = response.authResponse;
            if(((authResponse?.token ?? null) != null) && ((authResponse?.refreshToken ?? null) != null))
            { 
              localStorage.setItem("authResponse", JSON.stringify(authResponse));
              this.alert = {type : "success", message : "login successful"};
              this.router.navigateByUrl("home/home");
            }
            else
            {
              this.alert = {type : "danger", message : "internal server error"};
            }
          }
          else
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          //alert(`<span style="color:green">login successful </span>`);
        },
        (error:any)=>{
          if(error?.error?.CustomErrorCode ?? null != null )
          {
            if(error?.error?.CustomErrorCode == CustomErrorCode.Invalid)
            {
              this.alert = {type : "danger", message : error?.error?.CustomErrorMessage ?? ""};
            }
            else
            {
              this.alert = {type : "danger", message : "internal server error"};
            }
          }
          else
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
        });
    }
  }

}
