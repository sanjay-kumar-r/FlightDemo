import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { map } from 'rxjs';
import { Airlines } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';
import { AirlinesComponent } from '../airlines/airlines.component';

@Component({
  selector: 'app-add-or-edit-airline',
  templateUrl: './add-or-edit-airline.component.html',
  styleUrls: ['./add-or-edit-airline.component.css']
})
export class AddOrEditAirlineComponent implements OnInit {

  @Output() airlineSaveResult : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isErrorOutput : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isCancel : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isSaveTrigger : EventEmitter<boolean> = new EventEmitter<boolean>(); 

  isEdit:boolean = false;
  airlineId?:number|null;
  airline?:Airlines|null;

  isFormSubmitted:boolean = false;
  isDuplicate:boolean = false;
  isSeatsInvalid:boolean = false;
  isCanceled:boolean = false;
  isSavedTriggered:boolean = false;

  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  // name?:FormControl;
  // code?:FormControl;
  // contactNumber?:FormControl;
  // contactAddress?:FormControl;
  // totalSeats?:FormControl;
  // totalBcSeats?:FormControl;
  // totalNbcSeats?:FormControl;
  // bcCost?:FormControl;
  // nbcCost?:FormControl;
  // isActive?:FormControl;

  //airlinesForm?:FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public addEditAirlineDate:any,
    private addEditAirlineCompRef: MatDialogRef<AddOrEditAirlineComponent>,
    private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router
    ) 
  {
    this.airlineId = 0;
    this.airline = {
      TotalSeats:0,
      TotalBCSeats:0,
      TotalNBCSeats:0,
      BCTicketCost:0,
      NBCTicketCost:0,
    };
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
  }

  ngOnInit(): void {
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();
    if((this.addEditAirlineDate ?? null) != null)
    {
      this.isEdit = this.addEditAirlineDate?.isEdit ?? false;
      this.airlineId = this.addEditAirlineDate?.airlineId ?? 0;
      if(!this.isEdit)
      {
        this.airline = {
          TotalSeats:0,
          TotalBCSeats:0,
          TotalNBCSeats:0,
          BCTicketCost:0,
          NBCTicketCost:0,
        };
      }
      else
      {
        //edit
        if((this.airlineId ?? 0) >= 0)
        {
          //get airline
          this.airlineService.getAirlines(null, this.headerInfo, this.airlineId ?? 0)
          .subscribe(
            (result) => {
              this.getAirline(result[0]);
              if(((this.airline ?? null) != null) && ((this.airline?.Id ?? 0) == this.airlineId))
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
                if(error?.error?.CustomErrorCode == CustomErrorCode.Invalid)
                {
                  console.error(error);
                  this.emitError();
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
                    this.airlineService.getAirlines(null, this.headerInfo, this.airlineId ?? 0).subscribe(
                      (result) =>{
                        this.getAirline(result[0]);
                        if(((this.airline ?? null) != null) && ((this.airline?.Id ?? 0) == this.airlineId))
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
          console.error("airline id is not present");
          this.emitError();
        }
      }
    }
    else
    {
      this.emitError();
    }

    //this.prepareAirlineForm();
  }

  // prepareAirlineForm()
  // {
  //   this.name = new FormControl(this.airline?.Name, [Validators.required]);
  //   this.code = new FormControl(this.airline?.AirlineCode, [Validators.required]);
  //   this.contactNumber = new FormControl(this.airline?.ContactNumber, [Validators.required]);
  //   this.contactAddress = new FormControl(this.airline?.ContactAddress, [Validators.required]);
  //   this.totalSeats= new FormControl(this.airline?.TotalSeats, [Validators.required]);
  //   this.totalBcSeats= new FormControl(this.airline?.TotalBCSeats, [Validators.required]);
  //   this.totalNbcSeats= new FormControl(this.airline?.TotalNBCSeats, [Validators.required]);
  //   this.bcCost= new FormControl(this.airline?.BCTicketCost, [Validators.required]);
  //   this.nbcCost= new FormControl(this.airline?.NBCTicketCost, [Validators.required]);
  //   this.isActive= new FormControl(this.airline?.IsActive, [Validators.required]);

  //   this.airlinesForm = new FormGroup({
  //     name : this.name,
  //     code : this.code,
  //     contactNumber : this.contactNumber,
  //     contactAddress : this.contactAddress,
  //     totalSeats : this.totalSeats,
  //     totalBcSeats: this.totalBcSeats,
  //     totalNbcSeats: this.totalNbcSeats,
  //     bcCost: this.bcCost,
  //     nbcCost: this.nbcCost,
  //     isActive: this.isActive
  //   });
  // }

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
      // this.authResponse = {
      //   Token : authResponse.token,
      //   RefreshToken : authResponse.refreshToken
      // };
      this.authResponse = authResponse;
    }
  }

  getUserDetails(user:any)
  {
    if(((user ?? null) != null) && ((user?.Id ?? null) != null) && (user.Id != 0))
    {
      // this.userDetails = {
      //   Id : user.id,
      //   FirstName : user.firstName,
      //   LastName : user.lastName,
      //   EmailId : user.emailId,
      //   Password : user.password,
      //   AccountStatus : user.accountStatus.status,
      //   IsSuperAdmin : user.isSuperAdmin,
      //   CreatedOn : new Date(user.createdOn ?? ""),
      //   ModifiedOn : new Date(user.modifiedOn ?? "")
      // };
      this.userDetails = user;
    }
  }

  getAirline(airlineResult:any)
  {
    this.airline = {
      Id : airlineResult.id,
      Name : airlineResult.name,
      AirlineCode: airlineResult.airlineCode,
      ContactNumber: airlineResult.contactNumber,
      ContactAddress: airlineResult.contactAddress,
      TotalSeats: airlineResult.totalSeats,
      TotalBCSeats: airlineResult.totalBCSeats,
      TotalNBCSeats: airlineResult.totalNBCSeats,
      BCTicketCost: airlineResult.bcTicketCost,
      NBCTicketCost: airlineResult.nbcTicketCost,
      IsActive: airlineResult.isActive ?? false,
      CreatedOn: airlineResult.createdOn,
      ModifiedOn: airlineResult.modifiedOn
    }
  }

  changeTotalSeats(bcSeats:any, nbcSeats:any, totalSeats:any)
  {
    totalSeats.value = bcSeats.value + nbcSeats.value;
  }

  saveAirline(airlineForm:any){
    //console.log(airlineForm);
    if(!this.isCanceled)
    {
      if(airlineForm.valid)
      {
        this.isFormSubmitted = true;
        if(airlineForm.value.bcSeats + airlineForm.value.nbcSeats > 0)
        {
          this.prepareAirlineBodyFromForm(airlineForm.value);
          if(!this.isEdit)
            this.addAirline(this.airline);
          else
            this.editAirline(this.airline);
        }
        else{
          this.isSeatsInvalid = true;
        }
      }
      else
      {
        console.error("form is invalid");
      }
    }
  }

  addAirline(airlineRequest:any){
    this.airlineService.addAirlines(airlineRequest, this.headerInfo).subscribe(
      (result) => {
        if((result ?? null) != null)
        {
          alert("airline details added successfully");
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
              this.airlineService.addAirlines(airlineRequest, this.headerInfo).subscribe(
                (result) =>{
                  if((result ?? null) != null)
                  {
                    alert("airline details added successfully");
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

  editAirline(airlineRequest:any){
    this.airlineService.updateAirline(airlineRequest, this.headerInfo).subscribe(
      (result) => {
        if(((result ?? null) != null) && result?.res == true)
        {
          alert("airline details updated successfully");
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
              this.airlineService.updateAirline(airlineRequest, this.headerInfo).subscribe(
                (result) =>{
                  if(((result ?? null) != null) && result?.res == true)
                  {
                    alert("airline details updated successfully");
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

  prepareAirlineBodyFromForm(airlineForm:any)
  {
    let airlineRequest:Airlines = {
      Name : airlineForm.airlineName,
      AirlineCode: airlineForm.airlineCode,
      ContactNumber: airlineForm.contactNumber,
      ContactAddress: airlineForm.contactAddress,
      TotalSeats: airlineForm.bcSeats + airlineForm.nbcSeats,
      TotalBCSeats: airlineForm.bcSeats,
      TotalNBCSeats: airlineForm.nbcSeats,
      BCTicketCost: airlineForm.bcTicketCost,
      NBCTicketCost: airlineForm.nbcTicketCost,
      IsActive: airlineForm.isActive ?? false
    };
    if(this.isEdit)
      airlineRequest.Id = this.airline?.Id;
    this.airline = airlineRequest;
  }

  emitError()
  {
    this.isErrorOutput.emit(true);
    this.closePopup();
  }

  closePopup(){
    this.addEditAirlineCompRef.close();
  }

  cancel()
  {
    this.isCanceled = true;
    this.isCancel.emit(true);
    this.closePopup();
  }

}
