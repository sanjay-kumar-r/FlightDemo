import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { DiscountTags } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';

@Component({
  selector: 'app-add-or-edit-discount-tag',
  templateUrl: './add-or-edit-discount-tag.component.html',
  styleUrls: ['./add-or-edit-discount-tag.component.css']
})
export class AddOrEditDiscountTagComponent implements OnInit {

  @Output() airlineSaveResult : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isErrorOutput : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isCancel : EventEmitter<boolean> = new EventEmitter<boolean>(); 

  isEdit:boolean = false;
  discountTagId?:number|null;
  discountTag?:DiscountTags|null;

  isFormSubmitted:boolean = false;
  isDuplicate:boolean = false;
  isDiscountInvalid:boolean = false;
  isCanceled:boolean = false;

  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(@Inject(MAT_DIALOG_DATA) public addEditDiscountTagDate:any,
    private addEditDiscountTagCompRef: MatDialogRef<AddOrEditDiscountTagComponent>,
    private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router
    ) 
  {
    this.discountTagId = 0;
    this.discountTag = {
      Discount:0
    };
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
  }

  ngOnInit(): void {
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();
    if((this.addEditDiscountTagDate ?? null) != null)
    {
      this.isEdit = this.addEditDiscountTagDate?.isEdit ?? false;
      this.discountTagId = this.addEditDiscountTagDate?.discountTagId ?? 0;
      if(!this.isEdit)
      {
        this.discountTag = {
          Discount:0
        };
      }
      else
      {
        //edit
        if((this.discountTagId ?? 0) >= 0)
        {
          //get discountTag
          this.airlineService.getDiscountTags(null, this.headerInfo, this.discountTagId ?? 0)
          .subscribe(
            (result) => {
              this.getDiscountTag(result[0]);
              if(((this.discountTag ?? null) != null) && ((this.discountTag?.Id ?? 0) == this.discountTagId))
              {
                //valid
              }
              else
              {
                this.emitError();
              }
            },
            (error) =>{
              if(error?.error?.CustomErrorCode ?? null != null )
              {
                console.error(error);
                this.emitError();
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
                    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
                    this.getHeader();
                    //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
                    //retry
                    this.airlineService.getDiscountTags(null, this.headerInfo, this.discountTagId ?? 0).subscribe(
                      (result) =>{
                        this.getDiscountTag(result[0]);
                        if(((this.discountTag ?? null) != null) && ((this.discountTag?.Id ?? 0) == this.discountTagId))
                        {
                          //valid
                        }
                        else
                        {
                          this.emitError();
                        }
                      },
                      (e) =>{
                        console.error(e);
                        this.emitError();  
                      }
                    );
                  },
                  (refreshError) => {
                    console.error(refreshError);
                    this.emitError();
                  }
                );
              }
              else
              {
                console.error(error);
                this.emitError();
              }
            }
          );
        }
        else
        {
          console.error("discountTag id is not present");
          this.emitError();
        }
      }
    }
    else
    {
      this.emitError();
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

  getAuthResponse(authResponse:any)
  {
    if(((authResponse ?? null) != null) && ((authResponse?.Token ?? null) != null)
      && ((authResponse?.RefreshToken ?? null) != null))
    {
      this.authResponse = authResponse;
    }
  }

  getUserDetails(user:any)
  {
    if(((user ?? null) != null) && ((user?.Id ?? null) != null) && (user.Id != 0))
    {
      this.userDetails = user;
    }
  }

  getDiscountTag(discountTagResult:any)
  {
    this.discountTag = {
      Id : discountTagResult.id,
      Name : discountTagResult.name,
      DiscountCode: discountTagResult.discountCode,
      Description: discountTagResult.description,
      Discount: discountTagResult.discount ?? 0,
      IsByRate: discountTagResult.isByRate ?? false,
      IsActive: discountTagResult.isActive ?? false,
      CreatedOn: discountTagResult.createdOn,
      ModifiedOn: discountTagResult.modifiedOn
    }
  }

  saveDiscountTag(discountTagForm:any){
    if(!this.isCanceled)
    {
      if(discountTagForm.valid)
      {
        this.isFormSubmitted = true;
        if(discountTagForm.value.discount > 0)
        {
          this.prepareDiscountTagBodyFromForm(discountTagForm.value);
          if(!this.isEdit)
            this.addDiscountTag(this.discountTag);
          else
            this.editDiscountTag(this.discountTag);
        }
        else{
          this.isDiscountInvalid = true;
        }
      }
      else
      {
        console.error("form is invalid");
      }
    }
  }

  addDiscountTag(discountTagRequest:any){
    this.airlineService.addDiscountTag(discountTagRequest, this.headerInfo).subscribe(
      (result) => {
        if((result ?? null) != null)
        {
          alert("discount tag details added successfully");
          this.closePopup();
        }
        else{
          this.emitError();
        }
      },
      (error) =>{
        if(error?.error?.CustomErrorCode ?? null != null )
        {
          if(error?.error?.CustomErrorCode == CustomErrorCode.Duplicate)
          {
            this.isDuplicate = true;
            alert("invalid inputs");
          }
          else
          {
            console.error(error);
            this.emitError();
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
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.addDiscountTag(discountTagRequest, this.headerInfo).subscribe(
                (result) =>{
                  if((result ?? null) != null)
                  {
                    alert("discount tag details added successfully");
                    this.closePopup();
                  }
                  else{
                    this.emitError();
                  }
                },
                (e) =>{
                  if(e?.e?.CustomErrorCode ?? null != null )
                  {
                    if(e?.e?.CustomErrorCode == CustomErrorCode.Duplicate)
                    {
                      this.isDuplicate = true;
                      alert("invalid inputs");
                    }
                    else
                    {
                      console.error(e);
                      this.emitError();
                    }
                  }
                  else
                  {
                    console.error(e);
                    this.emitError();
                  }
                }
              );
            },
            (refreshError) => {
              console.error(refreshError);
              this.emitError();
            }
          );
        }
        else
        {
          console.error(error);
          this.emitError();
        }
      }
    );
  }

  editDiscountTag(discountTagRequest:any){
    this.airlineService.updateDiscountTag(discountTagRequest, this.headerInfo).subscribe(
      (result) => {
        if(((result ?? null) != null) && result?.res == true)
        {
          alert("discount tag details updated successfully");
          this.closePopup();
        }
        else{
          this.emitError();
        }
      },
      (error) =>{
        if(error?.error?.CustomErrorCode ?? null != null )
        {
          if(error?.error?.CustomErrorCode == CustomErrorCode.Duplicate)
          {
            this.isDuplicate = true;
            alert("invalid inputs");
          }
          else
          {
            console.error(error);
            this.emitError();
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
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.updateDiscountTag(discountTagRequest, this.headerInfo).subscribe(
                (result) =>{
                  if(((result ?? null) != null) && result?.res == true)
                  {
                    alert("discount tag details updated successfully");
                    this.closePopup();
                  }
                  else{
                    this.emitError();
                  }
                },
                (e) =>{
                  if(e?.e?.CustomErrorCode ?? null != null )
                  {
                    if(e?.e?.CustomErrorCode == CustomErrorCode.Duplicate)
                    {
                      this.isDuplicate = true;
                      alert("invalid inputs");
                    }
                    else
                    {
                      console.error(e);
                      this.emitError();
                    }
                  }
                  else
                  {
                    console.error(e);
                    this.emitError();
                  }
                }
              );
            },
            (refreshError) => {
              console.error(refreshError);
              this.emitError();
            }
          );
        }
        else
        {
          console.error(error);
          this.emitError();
        }
      }
    );
  }

  prepareDiscountTagBodyFromForm(discountTagForm:any)
  {
    let discountTagRequest:DiscountTags = {
      Name : discountTagForm.discountTagName,
      DiscountCode: discountTagForm.discountTagCode,
      Description: discountTagForm.description,
      Discount: discountTagForm?.discount ?? 0,
      IsByRate: discountTagForm.isByRate ?? false,
      IsActive: discountTagForm.isActive ?? false
    };
    if(this.isEdit)
    discountTagRequest.Id = this.discountTag?.Id;
    this.discountTag = discountTagRequest;
  }

  emitError()
  {
    this.isErrorOutput.emit(true);
    this.closePopup();
  }

  closePopup(){
    this.addEditDiscountTagCompRef.close();
  }

  cancel()
  {
    this.isCanceled = true;
    this.isCancel.emit(true);
    this.closePopup();
  }

}
