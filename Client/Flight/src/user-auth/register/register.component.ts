import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {UserService} from '../../Services/user.service';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { Users } from 'src/Models/Users';
import { invalid } from '@angular/compiler/src/render3/view/util';
import { CustomErrorCode } from 'src/Models/Commons';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  alert?:{type:string|null, message:string|null}|null;

  firstName:FormControl;
  lastName:FormControl;
  emailId:FormControl;
  password:FormControl;
  confirmPassword:FormControl;
  isSuperAdmin:FormControl;

  registerForm:FormGroup;
  
  router:Router;
  userService:UserService;

  constructor(router:Router, userService:UserService) {
    this.firstName = new FormControl("", [Validators.required]);
    this.lastName = new FormControl("");
    this.emailId = new FormControl("", [Validators.required, 
      Validators.pattern("^[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$")]);
    this.password = new FormControl("", [Validators.required, Validators.minLength(5)]);
    this.confirmPassword= new FormControl("", [Validators.required]);
    this.isSuperAdmin = new FormControl("false");

    this.registerForm = new FormGroup({
      firstName : this.firstName,
      lastName : this.lastName,
      emailId : this.emailId,
      password : this.password,
      confirmPassword : this.confirmPassword,
      isSuperAdmin : this.isSuperAdmin
    });

    this.router = router;
    this.userService = userService;
    this.alert = null;
  }

  ngOnInit(): void {
    this.alert = null;
    this.firstName = new FormControl("", [Validators.required]);
    this.lastName = new FormControl("");
    this.emailId = new FormControl("", [Validators.required, 
      Validators.pattern("^[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$")]);
    this.password = new FormControl("", [Validators.required, Validators.minLength(5)]);
    this.confirmPassword= new FormControl("", [Validators.required]);
    this.isSuperAdmin = new FormControl("false");

    this.registerForm = new FormGroup({
      firstName : this.firstName,
      lastName : this.lastName,
      emailId : this.emailId,
      password : this.password,
      confirmPassword : this.confirmPassword,
      isSuperAdmin : this.isSuperAdmin
    });
  }

  closeAlert() {
    this.alert = null;
  }

  redirectTologinr()
  {
    this.router.navigateByUrl("users/login");
  }

  registerUser()
  {
    if(this.registerForm.valid && ((this.registerForm?.value ?? null) != null))
    {
      if(((this.registerForm.value.firstName ?? null) != null)
        && ((this.registerForm.value.lastName ?? null) != null)
        && ((this.registerForm.value.emailId ?? null) != null)
        && ((this.registerForm.value.password ?? null) != null)
        && ((this.registerForm.value.confirmPassword ?? null) != null))
      {
        if(this.registerForm.value.password == this.registerForm.value.confirmPassword)
        {
          //register
          let headerInfo:HeaderInfo|any = null;
          let user:Users ={
            FirstName:this.firstName.value,
            LastName:this.lastName.value,
            EmailId:this.emailId.value,
            Password:this.password.value
          }

          if((this.isSuperAdmin.value ?? false) ?? true)
          {
            this.registerAsAdmin(user, headerInfo);
          }
          else
          {
            this.registerAsNormal(user, headerInfo);
          }
          // this.userService.registerUser(user, headerInfo).subscribe(
          //   (response) =>{
          //     //console.log("response :=>" ,response);
          //     if((response ?? null) != null)
          //     {
          //       this.alert = {type : "success", message : "registration successful"};
          //       this.router.navigateByUrl("users/login");
          //     }
          //     else
          //     {
          //       this.alert = {type : "danger", message : "internal server error"};
          //     }
          //     //alert(`<span style="color:green">login successful </span>`);
          //   },
          //   (error:any)=>{
          //     if(error?.error?.CustomErrorCode ?? null != null )
          //     {
          //       if(error?.error?.CustomErrorCode == CustomErrorCode.Duplicate)
          //       {
          //         this.alert = {type : "danger", message : error?.error?.CustomErrorMessage ?? ""};
          //       }
          //       else
          //       {
          //         this.alert = {type : "danger", message : "internal server error"};
          //       }
          //     }
          //     else
          //     {
          //       this.alert = {type : "danger", message : "internal server error"};
          //     }
          //   });


        }
        else
        {
          //error is password
          this.alert = {type : "danger", message : "password & confirm password does not match"};
        }
      }
      else
      {
        //invalid inputs
        this.alert = {type : "danger", message : "invalid inputs"};
      }
    }
    else
    {
      //invalid inputs
      this.alert = {type : "danger", message : "invalid inputs"};
    }
  }

  registerAsNormal(user:Users, headerInfo:HeaderInfo)
  {
    this.userService.registerUser(user, headerInfo).subscribe(
      (response) =>{
        //console.log("response :=>" ,response);
        if((response ?? null) != null)
        {
          this.alert = {type : "success", message : "registration successful"};
          this.router.navigateByUrl("users/login");
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
          if(error?.error?.CustomErrorCode == CustomErrorCode.Duplicate)
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

  registerAsAdmin(user:Users, headerInfo:HeaderInfo)
  {
    this.userService.registerAsAdmin(user, headerInfo).subscribe(
      (response) =>{
        //console.log("response :=>" ,response);
        if((response ?? null) != null)
        {
          this.alert = {type : "success", message : "registration successful"};
          this.router.navigateByUrl("users/login");
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
          if(error?.error?.CustomErrorCode == CustomErrorCode.Duplicate)
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


// export function ConfirmPasswordValidator(controlName: string, matchingControlName: string) {
//   return (formGroup: FormGroup) => {
//     let control = formGroup.controls[controlName];
//     let matchingControl = formGroup.controls[matchingControlName]
//     if (
//       matchingControl.errors &&
//       !matchingControl.errors.confirmPasswordValidator
//     ) {
//       return;
//     }
//     if (control.value !== matchingControl.value) {
//       matchingControl.setErrors({ confirmPasswordValidator: true });
//     } else {
//       matchingControl.setErrors(null);
//     }
//   };
//}
