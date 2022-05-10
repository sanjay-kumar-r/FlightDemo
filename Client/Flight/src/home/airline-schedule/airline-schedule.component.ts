import { WeekDay } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Airlines, AirlineSchedules } from 'src/Models/Airlines';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';
import { AddOrEditAirlineScheduleComponent } from '../add-or-edit-airline-schedule/add-or-edit-airline-schedule.component';
import { DeleteConfirmationComponent } from '../delete-confirmation/delete-confirmation.component';

@Component({
  selector: 'app-airline-schedule',
  templateUrl: './airline-schedule.component.html',
  styleUrls: ['./airline-schedule.component.css']
})
export class AirlineScheduleComponent implements OnInit {

  @Input() data :any;
  
  isAddEditError:boolean = false;
  isAddEditCancel:boolean = false;
  isAddEditSaved:boolean = false;
  confirmDelete:boolean = false;

  alert?:{type:string|null, message:string|null}|null;

  airlines?:Airlines[]|null;
  airlineSchedules?:AirlineSchedules[]|null;
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
      this.userDetails = user;
    }
  }

  getAuthResponse(authResponse:any)
  {
    if(((authResponse ?? null) != null) && ((authResponse?.Token ?? null) != null)
      && ((authResponse?.RefreshToken ?? null) != null))
    {
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
    this.loadAirlineSchedules();
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

  getAirlineSchedulesList(airlineSchedulesList:any[]|null)
  {
    if(((airlineSchedulesList ?? null) != null) && ((airlineSchedulesList?.length ?? 0) > 0) )
    {
      this.airlineSchedules = [];
      airlineSchedulesList?.forEach(x => 
      {
        this.airlineSchedules?.push(this.getAirlineSchedule(x));
      });
    }
  }

  getAirlineSchedule(airlineSchedule:any)
  {
    let res:AirlineSchedules = {
      Id : airlineSchedule.id,
      AirlineId : airlineSchedule.airlineId,
      Airline: this.getAirline(airlineSchedule.airline),
      From: airlineSchedule.from,
      To: airlineSchedule.to,
      IsRegular: airlineSchedule?.isRegular ?? false,
      DepartureDay: ((airlineSchedule?.departureDay ?? null) != null) ? WeekDay[airlineSchedule.departureDay] as unknown as WeekDay : undefined,
      DepartureDate: airlineSchedule.departureDate,
      DepartureTime: airlineSchedule.departureTime,
      ArrivalDay: ((airlineSchedule?.arrivalDay ?? null) != null) ? WeekDay[airlineSchedule.arrivalDay] as unknown as WeekDay : undefined,
      ArrivalDate: airlineSchedule.arrivalDate,
      ArrivalTime: airlineSchedule.arrivalTime,
      CreatedOn: airlineSchedule.createdOn,
      ModifiedOn: airlineSchedule.modifiedOn
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

  loadAirlineSchedules(){
    this.airlineService.getSchedules(null, this.headerInfo).subscribe(
      (result) =>{
        this.getAirlineSchedulesList(result);
        if(((this.airlineSchedules ?? null) == null) || ((this.airlineSchedules?.length ?? 0) <= 0) )
        {
          this.alert = {type : "danger", message : "internal server error"};
        }
        else
        {
          //success loaded
        }
      },
      (error) => {
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
              this.airlineService.getAirlines(null, this.headerInfo).subscribe(
                (result) =>{
                  this.getAirlinesList(result);
                  if(((this.airlineSchedules ?? null) == null) || ((this.airlineSchedules?.length ?? 0) <= 0) )
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

  addAirlineSchedulePopup()
  {
    const addAirlineSchedulePopupRef = this.matDialog.open(AddOrEditAirlineScheduleComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:false,scheduleId:0},
      "autoFocus": false
    });
    addAirlineSchedulePopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    addAirlineSchedulePopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    addAirlineSchedulePopupRef.componentInstance.isSaveTrigger.subscribe(
      isSaved => this.isAddEditSaved = isSaved
    );
    addAirlineSchedulePopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditSaved)
          {
            this.loadAirlineSchedules();
          }
          else if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          // else
          //   this.loadAirlineSchedules();
        }
      }
    );
  }

  editAirlineSchedulePopup(scheduleId:number){
    const editAirlineSchedulePopupRef = this.matDialog.open(AddOrEditAirlineScheduleComponent, {
      "width": '6000px',
      "maxHeight": '90vh',
      "data": {isEdit:true,scheduleId:scheduleId},
      "autoFocus": false
    });
    editAirlineSchedulePopupRef.componentInstance.isErrorOutput.subscribe(
      iserror => this.isAddEditError = iserror
    );
    editAirlineSchedulePopupRef.componentInstance.isCancel.subscribe(
      isCancel => this.isAddEditCancel = isCancel
    );
    editAirlineSchedulePopupRef.componentInstance.isSaveTrigger.subscribe(
      isSaved => this.isAddEditSaved = isSaved
    );
    editAirlineSchedulePopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          if(this.isAddEditSaved)
          {
            this.loadAirlineSchedules();
          }
          else if(this.isAddEditError)
          {
            this.alert = {type : "danger", message : "internal server error"};
          }
          // else
          //   this.loadAirlineSchedules();
        }
      }
    );
  }

  deleteAirlineSchedulePopup(scheduleId:number)
  {
    const deleteAirlineSchedulePopupRef = this.matDialog.open(DeleteConfirmationComponent, {
      "width": '400px',
      "maxHeight": '90vh',
      "data": {title:"Airline Schedule",itemName:this.airlineSchedules?.find(x => x.Id == scheduleId)?.Airline?.Name},
      "autoFocus": false
    });
    deleteAirlineSchedulePopupRef.componentInstance.isYes.subscribe(
      isYes => this.confirmDelete = isYes
    );
    deleteAirlineSchedulePopupRef.afterClosed().subscribe(
      (result) => {
        if(this.confirmDelete)
        {
          //delete api //and delete row
          this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
          this.getHeader();
          this.airlineService.deleteAirlineSchedule(scheduleId, this.headerInfo).subscribe(
            (result) => {
              if(((result ?? null) != null) && result == true)
              {
                this.alert = {type : "success", message : `airline schedule deleted successfully`};
                //remove element
                this.airlineSchedules = this.airlineSchedules?.filter(x => x.Id !== scheduleId);
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
                  this.airlineService.deleteAirlineSchedule(scheduleId, this.headerInfo).subscribe(
                  (result) =>{
                    if(((result ?? null) != null) && result == true)
                    {
                      this.alert = {type : "success", message : `airline schedule deleted successfully`};
                      //remove element
                      this.airlineSchedules = this.airlineSchedules?.filter(x => x.Id !== scheduleId);
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
