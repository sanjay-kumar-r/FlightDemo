import { WeekDay } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { delay } from 'rxjs';
import { Airlines, AirlineSchedules } from 'src/Models/Airlines';
import { BookingDetails, Bookings } from 'src/Models/Bookings';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { BookingService } from 'src/Services/booking.service';
import { CommonService } from 'src/Services/common.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';
import { DeleteConfirmationComponent } from '../delete-confirmation/delete-confirmation.component';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.css']
})
export class BookingsComponent implements OnInit {

  @Input() data :any;
  //@ViewChild(AddOrEditAirlineComponent, { static : true }) addEditAirlineComp!: AddOrEditAirlineComponent;

  isAddEditError:boolean = false;
  isAddEditCancel:boolean = false;
  isAddEditSaved:boolean = false;
  confirmDelete:boolean = false;
  airlineSchedules?:AirlineSchedules[]|null;
  bookingDetails?:BookingDetails[]|null;

  isScheduleLoadingComplete:boolean = false;

  alert?:{type:string|null, message:string|null}|null;

  bookings?:Bookings[]|null;
  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(private bookingService:BookingService,
    private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router,
    private dynamicComponent: DynamicComponentService,
    private matDialog: MatDialog) { 
    //console.log("data: ", this.data);
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
    this.alert = null;
    this.airlineSchedules = [];
    this.bookingDetails = [];
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

  getBookingList(bookingsList:any[]|null)
  {
    if(((bookingsList ?? null) != null) && ((bookingsList?.length ?? 0) > 0) )
    {
      this.bookings = [];
      bookingsList?.forEach(x => 
      {
        this.bookings?.push(this.getBooking(x));
      });
    }
  }

  getBooking(booking:any)
  {
    let res:Bookings = {
      Id : booking.id,
      UserId : booking.userId,
      ScheduleId: booking.scheduleId,
      DateBookedFor: booking.dateBookedFor,
      BCSeats: booking.bcSeats ?? 0,
      NBCSeats: booking.nbcSeats ?? 0,
      ActualPaidAmount: booking.actualPaidAmount,
      BookingStatusId: booking.bookingStatusId,
      BookingStatus: {
        Id : booking.bookingStatus.id,
        Status: booking.bookingStatus.status,
        Description : booking.bookingStatus.description
      },
      PNR: booking.pnr,
      IsRefunded: booking.isRefunded ?? false,
      CreatedOn: booking.createdOn,
      CanceledOn: booking.canceledOn
    }
    return res;
  }

  getAirline(airlineResult:any)
  {
    let res:Airlines = {
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
    return res;
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
    this.airlineSchedules?.push(res);
    return res;
  }

  getBookingDetailsList(schedule:AirlineSchedules){
    let bookingFiltered = this.bookings?.filter(x => x.ScheduleId == schedule.Id);
    bookingFiltered?.forEach(x => {
      let res:BookingDetails = {
        booking : x,
        ScheduleDetail: schedule
      }
      this.bookingDetails?.push(res);
    });
  }

  ngOnInit(): void {
    this.alert = null;
    //console.log("data: ", this.data);
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();
    this.airlineSchedules = [];
    this.bookingDetails = [];

    if(this.userDetails == null || this.authResponse == null)
    {
      this.logout();
      return;
    }
    this.loadBookings();
  }

  loadSchedule(scheduleId:number){
      //get airline
    this.airlineService.getSchedules(scheduleId, this.headerInfo)
    .subscribe(
      (result) => {
        let res = this.getAirlineSchedule(result[0]);
        this.getBookingDetailsList(res);
        if((this.airlineSchedules?.length ?? 0) == (this.bookings?.length ?? 0))
        {
          this.isScheduleLoadingComplete = true;
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
              this.airlineService.getSchedules(scheduleId, this.headerInfo).subscribe(
                (result) =>{
                  let res = this.getAirlineSchedule(result[0]);
                  this.getBookingDetailsList(res);
                  if((this.airlineSchedules?.length ?? 0) == (this.bookings?.length ?? 0))
                  {
                    this.isScheduleLoadingComplete = true;
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
              this.alert = {type : "danger", message : "internal server error"};
            }
          );
        }
        else
        {
          console.error(error);
          this.alert = {type : "danger", message : "internal server error"};
        }
      }
    );
  }

  loadBookings(){
    this.bookingService.getBookings(null, this.headerInfo).subscribe(
      (result) =>{
        this.getBookingList(result);
        if(((this.bookings ?? null) == null) || ((this.bookings?.length ?? 0) <= 0) )
        {
          //this.alert = {type : "danger", message : "internal server error"};
        }
        else
        {
          //success loaded
          // let ids:number[] =[];
          // this.bookings?.forEach(x => {
          //    if((x.ScheduleId ?? 0) > 0)
          //       ids.push(x.ScheduleId ?? 0);
          // });
          // let scheduleIds = [...new Set(ids)];
          // if((scheduleIds?.length ?? 0) > 0)
          // {
          //   this.bookings?.forEach(x => {

          //     this.loadSchedule(x.ScheduleId ?? 0);
          //   });
          // }

          while(!this.cancelBooking)
          {
            //wait
            delay(1000);
          }
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
              this.bookingService.getBookings(null, this.headerInfo).subscribe(
                (result) =>{
                  this.getBookingList(result);
                  if(((this.bookings ?? null) == null) || ((this.bookings?.length ?? 0) <= 0) )
                  {
                    //this.alert = {type : "danger", message : "internal server error"};
                  }
                  else
                  {
                    //success loaded
                    // this.bookings?.forEach(x => {

                    //   this.loadSchedule(x.ScheduleId ?? 0);
                    // });
          
                    // while(!this.cancelBooking)
                    // {
                    //   //wait
                    //   delay(1000);
                    // }
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

  cancelBooking(bookingId:number)
  {
    const deletePopupRef = this.matDialog.open(DeleteConfirmationComponent, {
      "width": '400px',
      "maxHeight": '90vh',
      "data": {title:"Booking",itemName:this.bookings?.find(x => x.Id == bookingId)?.PNR},
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
          this.bookingService.cancelBooking(bookingId, this.headerInfo).subscribe(
            (result) => {
              if(((result ?? null) != null) && result == true)
              {
                this.alert = {type : "success", message : `booking for pnr :${this.bookings?.find(x => x.Id == bookingId)?.PNR} canceled successfully`};
                //remove element
                this.loadBookings();
                //this.bookings = this.bookings?.filter(x => x.Id !== bookingId);
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
                  this.bookingService.cancelBooking(bookingId, this.headerInfo).subscribe(
                  (result) =>{
                    if(((result ?? null) != null) && result == true)
                    {
                      this.alert = {type : "success", message : `booking for pnr :${this.bookings?.find(x => x.Id == bookingId)?.PNR} canceled successfully`};
                      this.loadBookings();
                      //remove element
                      //this.airlines = this.airlines?.filter(x => x.Id !== airlineId);
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

  cancelBookingPopUp(id:number)
  {

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
