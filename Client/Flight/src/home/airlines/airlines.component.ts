import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { right } from '@popperjs/core';
import { Airlines } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { ApiExecutorService } from 'src/Services/api-executor.service';
import { CommonService } from 'src/Services/common.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { AddOrEditAirlineComponent } from '../add-or-edit-airline/add-or-edit-airline.component';
import { DeleteConfirmationComponent } from '../delete-confirmation/delete-confirmation.component';

@Component({
  selector: 'app-airlines',
  templateUrl: './airlines.component.html',
  styleUrls: ['./airlines.component.css']
})
export class AirlinesComponent implements OnInit {

  @Input() data :any;
  //@ViewChild(AddOrEditAirlineComponent, { static : true }) addEditAirlineComp!: AddOrEditAirlineComponent;

  isAddEditError:boolean = false;
  isAddEditCancel:boolean = false;
  confirmDelete:boolean = false;

  alert?:{type:string|null, message:string|null}|null;

  airlines?:Airlines[]|null;
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
    this.loadAirlines();
  }

  getAirlinesList(airlineList:any[]|null)
  {
    if(((airlineList ?? null) != null) && ((airlineList?.length ?? 0) > 0) )
    {
      this.airlines = [];
      airlineList?.forEach(x => 
      {
        this.airlines?.push(this.getAirline(x));
      });
    }
  }

  getAirline(airline:any)
  {
    let res:Airlines = {
      Id : airline.id,
      Name : airline.name,
      AirlineCode: airline.airlineCode,
      ContactNumber: airline.contactNumber,
      ContactAddress: airline.contactAddress,
      TotalSeats: airline.totalSeats,
      TotalBCSeats: airline.totalBCSeats,
      TotalNBCSeats: airline.totalNBCSeats,
      BCTicketCost: airline.bcTicketCost,
      NBCTicketCost: airline.nbcTicketCost,
      IsActive: airline.isActive ?? false,
      CreatedOn: airline.createdOn,
      ModifiedOn: airline.modifiedOn
    }
    return res;
  }

  loadAirlines(){
    this.airlineService.getAirlines(null, this.headerInfo).subscribe(
      (result) =>{
        this.getAirlinesList(result);
        if(((this.airlines ?? null) == null) || ((this.airlines?.length ?? 0) <= 0) )
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
          //this.alert = {type : "danger", message : "unauthorized"};
          //console.log(error);
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
              this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.getAirlines(null, this.headerInfo).subscribe(
                (result) =>{
                  this.getAirlinesList(result);
                  if(((this.airlines ?? null) == null) || ((this.airlines?.length ?? 0) <= 0) )
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

  addAirlinePopup()
  {
    const addAirlinePopupRef = this.matDialog.open(AddOrEditAirlineComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:false,airlineId:0},
      "autoFocus": false
    });
    addAirlinePopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    addAirlinePopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    addAirlinePopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          else
            this.loadAirlines();
        }
      }
    );
  }

  editAirlinePopup(airlineId:number){
    const editAirlinePopupRef = this.matDialog.open(AddOrEditAirlineComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:true,airlineId:airlineId},
      "autoFocus": false
    });
    editAirlinePopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    editAirlinePopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    editAirlinePopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          else
            this.loadAirlines();
        }
      }
    );
  }

  deleteAirlinePopup(airlineId:number)
  {
    const deletePopupRef = this.matDialog.open(DeleteConfirmationComponent, {
      "width": '400px',
      "maxHeight": '90vh',
      "data": {title:"Airline",itemName:this.airlines?.find(x => x.Id == airlineId)?.Name},
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
          this.airlineService.deleteAirline(airlineId, this.headerInfo).subscribe(
            (result) => {
              if(((result ?? null) != null) && result?.res == true)
              {
                //alert(`airline ${this.airlines?.find(x => x.Id == airlineId)?.Name} deleted successfully`);
                this.alert = {type : "success", message : `airline ${this.airlines?.find(x => x.Id == airlineId)?.Name} deleted successfully`};
                //remove element
                this.airlines = this.airlines?.filter(x => x.Id !== airlineId);
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
                  this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
                  //retry
                  this.airlineService.deleteAirline(airlineId, this.headerInfo).subscribe(
                  (result) =>{
                    if(((result ?? null) != null) && result?.res == true)
                    {
                      //alert(`airline ${this.airlines?.find(x => x.Id == airlineId)?.Name} deleted successfully`);
                      this.alert = {type : "success", message : `airline ${this.airlines?.find(x => x.Id == airlineId)?.Name} deleted successfully`};
                      //remove element
                      this.airlines = this.airlines?.filter(x => x.Id !== airlineId);
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

  logout()
  {
    localStorage.clear();
    this.router.navigateByUrl("users/login");
  }

  closeAlert() {
    this.alert = null;
  }



}
