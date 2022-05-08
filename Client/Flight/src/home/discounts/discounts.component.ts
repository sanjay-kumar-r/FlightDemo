import { Component, OnInit, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { DiscountTags } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';
import { AddOrEditDiscountTagComponent } from '../add-or-edit-discount-tag/add-or-edit-discount-tag.component';
import { DeleteConfirmationComponent } from '../delete-confirmation/delete-confirmation.component';

@Component({
  selector: 'app-discounts',
  templateUrl: './discounts.component.html',
  styleUrls: ['./discounts.component.css']
})
export class DiscountsComponent implements OnInit {

  @Input() data :any;

  isAddEditError:boolean = false;
  isAddEditCancel:boolean = false;
  confirmDelete:boolean = false;

  alert?:{type:string|null, message:string|null}|null;

  discountTags?:DiscountTags[]|null;
  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router,
    private dynamicComponent: DynamicComponentService,
    private matDialog: MatDialog) { 
    //console.log("data: ", this.data);
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
    this.alert = null;
  }

  ngOnInit(): void {
    this.alert = null;
    //console.log("data: ", this.data);
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();

    if(this.userDetails == null || this.authResponse == null)
    {
      this.logout();
      return;
    }
    this.loadDiscountTags();
  }

  getUserDetails(user:any)
  {
    if(((user ?? null) != null) && ((user?.Id ?? null) != null) && (user.Id != 0))
    {
      this.userDetails = user;
    }
  }

  getAuthResponse(authResponse:any)
  {
    if(((authResponse ?? null) != null) && ((authResponse?.Token ?? null) != null)
      && ((authResponse?.RefreshToken ?? null) != null))
    {
      // this.authResponse = {
      //   Token : authResponse.token,
      //   RefreshToken : authResponse.refreshToken
      // };
      this.authResponse = authResponse;
    }
  }

  getHeader()
  {
    this.headerInfo = {
      UserId: this.userDetails?.Id?.toString() ?? "",
      Authorization: `Bearer ${this.authResponse?.Token}`,
      RefreshToken: this.authResponse?.RefreshToken ?? ""
    };
  }

  logout()
  {
    localStorage.clear();
    this.router.navigateByUrl("users/login");
  }

  closeAlert() {
    this.alert = null;
  }

  getDiscountTagList(discountTags:any[]|null)
  {
    if(((discountTags ?? null) != null) && ((discountTags?.length ?? 0) > 0) )
    {
      this.discountTags = [];
      discountTags?.forEach(x => 
      {
        this.discountTags?.push(this.getDiscountTag(x));
      });
    }
  }

  getDiscountTag(discountTag:any)
  {
    let res:DiscountTags = {
      Id : discountTag.id,
      Name : discountTag.name,
      DiscountCode: discountTag.discountCode,
      Description: discountTag.description,
      Discount: discountTag.discount ?? 0,
      IsByRate: discountTag?.isByRate ?? false,
      IsActive: discountTag?.isActive ?? false,
      CreatedOn: discountTag.createdOn,
      ModifiedOn: discountTag.modifiedOn
    }
    return res;
  }

  loadDiscountTags(){
    this.airlineService.getDiscountTags(null, this.headerInfo).subscribe(
      (result) =>{
        this.getDiscountTagList(result);
        if(((this.discountTags ?? null) == null) || ((this.discountTags?.length ?? 0) <= 0) )
        {
          this.alert = {type : "danger", message : "internal server error"};
        }
        else
        {
          //success loaded
        }
      },
      (error) => {
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
        else if(error.status == 401)
        {
          //refreshToken
          this.commonService.refreshToken(this.headerInfo).subscribe(
            (authResponse) =>{
              let refreshResult:AuthResponse = {
                IsSuccess: authResponse.isSuccess,
                Token : authResponse.token,
                RefreshToken : authResponse.refreshToken,
                Reason: authResponse.reason
              };
              localStorage.setItem("authResponse", JSON.stringify(refreshResult));
              //this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.getDiscountTags(null, this.headerInfo).subscribe(
                (result) =>{
                  this.getDiscountTagList(result);
                  if(((this.discountTags ?? null) == null) || ((this.discountTags?.length ?? 0) <= 0) )
                  {
                    this.alert = {type : "danger", message : "internal server error"};
                  }
                  else
                  {
                    //success loaded
                  }
                },
                (e) =>{
                  console.error(e);
                  this.alert = {type : "danger", message : "internal server error"};  
                }
              );
            },
            (refreshError) => {
              console.error(refreshError);
              this.logout();
            }
          );
        }
        else
        {
          this.alert = {type : "danger", message : "internal server error"};
        }
      }
    );
  }

  addDiscountTag()
  {
    const addDiscountTagPopupRef = this.matDialog.open(AddOrEditDiscountTagComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:false,discountTagId:0},
      "autoFocus": false
    });
    addDiscountTagPopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    addDiscountTagPopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    addDiscountTagPopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          else
            this.loadDiscountTags();
        }
      }
    );
  }

  editDiscountTagPopup(discountTagId:number){
    const editDiscountTagPopupRef = this.matDialog.open(AddOrEditDiscountTagComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:true,discountTagId:discountTagId},
      "autoFocus": false
    });
    editDiscountTagPopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    editDiscountTagPopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    editDiscountTagPopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          else
            this.loadDiscountTags();
        }
      }
    );
  }

  deleteDiscountTagPopup(discountTagId:number)
  {
    const deletePopupRef = this.matDialog.open(DeleteConfirmationComponent, {
      "width": '400px',
      "maxHeight": '90vh',
      "data": {title:"DiscountTag",itemName:this.discountTags?.find(x => x.Id == discountTagId)?.Name},
      "autoFocus": false
    });
    deletePopupRef.componentInstance.isYes.subscribe(
      isYes => this.confirmDelete = isYes
    );
    deletePopupRef.afterClosed().subscribe(
      (result) => {
        if(this.confirmDelete)
        {
          //delete api //and delete row
          this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
          this.getHeader();
          this.airlineService.deleteDiscountTag(discountTagId, this.headerInfo).subscribe(
            (result) => {
              if(((result ?? null) != null) && result?.res == true)
              {
                this.alert = {type : "success", message : `discountTag ${this.discountTags?.find(x => x.Id == discountTagId)?.Name} deleted successfully`};
                //remove element
                this.discountTags = this.discountTags?.filter(x => x.Id !== discountTagId);
              }
              else{
                this.alert = {type : "danger", message : "internal server error"};
              }
            },
            (error) =>{
              if(error.status == 401)
              {
                //refreshToken
                this.commonService.refreshToken(this.headerInfo).subscribe(
                (authResponse) =>{
                  let refreshResult:AuthResponse = {
                    IsSuccess: authResponse.isSuccess,
                    Token : authResponse.token,
                    RefreshToken : authResponse.refreshToken,
                    Reason: authResponse.reason
                  };
                  localStorage.setItem("authResponse", JSON.stringify(refreshResult));
                  this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
                  this.getHeader();
                  //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
                  //retry
                  this.airlineService.deleteDiscountTag(discountTagId, this.headerInfo).subscribe(
                  (result) =>{
                    if(((result ?? null) != null) && result?.res == true)
                    {
                      this.alert = {type : "success", message : `discountTag ${this.discountTags?.find(x => x.Id == discountTagId)?.Name} deleted successfully`};
                      //remove element
                      this.discountTags = this.discountTags?.filter(x => x.Id !== discountTagId);
                    }
                    else{
                      this.alert = {type : "danger", message : "internal server error"};
                    }
                  },
                  (e) =>{
                    this.alert = {type : "danger", message : "internal server error"};
                  });
                },
                (refreshError) => {
                  console.error(refreshError);
                  this.alert = {type : "danger", message : "internal server error"};
                });
              }
              else
                this.alert = {type : "danger", message : "internal server error"};
            }
          );
        }
      }
    );
  }

}
