import { Component, Inject, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Airlines, AirlinesSearchResponse, DiscountTags } from 'src/Models/Airlines';
import { Bookings } from 'src/Models/Bookings';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { BookingService } from 'src/Services/booking.service';
import { CommonService } from 'src/Services/common.service';

@Component({
  selector: 'app-booking-action',
  templateUrl: './booking-action.component.html',
  styleUrls: ['./booking-action.component.css']
})
export class BookingActionComponent implements OnInit {

  @Output() bookingSaveResult : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isErrorOutput : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isCancel : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isSaveTrigger : EventEmitter<boolean> = new EventEmitter<boolean>(); 

  // scheduleId?:number;
  // dateBookedFor?:Date;
  // discountTags?:DiscountTags[];
  // airline?:Airlines;
  airlineSearchResponse?:AirlinesSearchResponse;
  dynamicBCAvailableSeats?:number;
  dynamicNBCAvailableSeats?:number;
  actualPaidAmount?:number;


  isFormSubmitted:boolean = false;
  isDuplicate:boolean = false;
  isSeatsInvalid:boolean = false;
  isCanceled:boolean = false;
  isSavedTriggered:boolean = false;
  

  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(@Inject(MAT_DIALOG_DATA) public bookingInputData:any,
    private bookingActionPopupRef: MatDialogRef<BookingActionComponent>,
    private bookingService:BookingService,
    private commonService:CommonService,
    private router:Router
    ) 
  {
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
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

  prepareBookingRequest(scheduleId?:number, DateBookedFor?:Date, bc?:number, nbc?:number,
     actualPaidAmount?:number){
    let res: Bookings ={
      ScheduleId:scheduleId,
      DateBookedFor:DateBookedFor,
      BCSeats:bc,
      NBCSeats:nbc,
      ActualPaidAmount:actualPaidAmount
    }
    return res;
  }

  ngOnInit(): void {
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();

    this.airlineSearchResponse = this.bookingInputData?.airlineSearchResponse;
    this.dynamicBCAvailableSeats = this.airlineSearchResponse?.BCSeatsAvailable ?? 0;
    this.dynamicNBCAvailableSeats = this.airlineSearchResponse?.NBCSeatsAvailable ?? 0;

    // this.scheduleId = this.bookingInputData?.scheduleId ?? 0;
    // this.actualPaidAmount = this.bookingInputData?.actualPaidAmount ?? 0;
    // this.dateBookedFor = this.bookingInputData?.dateBookedFor;
   
     }

  validateBCMaxAndMin($event:any, bcSeats?:string, nbcSeats?:string, discountId?:string){
    let val = Number($event.target.value);
    if(!isNaN(val) && val <= 0)
    {
      $event.target.value = 0;
    }
    // else if(!isNaN(val) && val >= (this.airlineSearchResponse?.BCSeatsAvailable ?? 0))
    // {
    //   $event.target.value = (this.airlineSearchResponse?.BCSeatsAvailable ?? 0);
    // }
    this.claculateActualAmount($event.target.value, nbcSeats, discountId);
  }

  validateNBCMaxAndMin($event:any, bcSeats?:string, nbcSeats?:string, discountId?:string){
    let val = Number($event.target.value);
    if(!isNaN(val) && val <= 0)
    {
      $event.target.value = 0;
    }
    // else if(!isNaN(val) && val >= (this.airlineSearchResponse?.NBCSeatsAvailable ?? 0))
    // {
    //   $event.target.value = (this.airlineSearchResponse?.NBCSeatsAvailable ?? 0);
    // }
    this.claculateActualAmount(bcSeats, $event.target.value, discountId);
  }

  claculateActualAmount(bcSeats?:string, nbcSeats?:string, discountId?:string)
  {
    let bc:number = 0;
    let nbc:number = 0;
    if(((bcSeats ?? null) != null) &&(bcSeats !== ""))
      bc = Number(bcSeats);
    if(((nbcSeats ?? null) != null) &&(nbcSeats !== ""))
      nbc = Number(nbcSeats);
    
    let bcCost:number = this.airlineSearchResponse?.AirlineSchedules?.Airline?.BCTicketCost ?? 0;
    let nbcCost:number = this.airlineSearchResponse?.AirlineSchedules?.Airline?.NBCTicketCost ?? 0;

    let discountedBCAmount:number = bcCost;
    let discountedNBCAmount:number = nbcCost;

    let discount:DiscountTags|null = null;
    if(((discountId ?? null) != null)
     && this.airlineSearchResponse?.DiscountTags?.some(x => x.Id == discountId))
    {
      discount = this.airlineSearchResponse.DiscountTags.find(x => x.Id == discountId) ?? null;
    }
    if((discount ?? null) != null)
    {
      if((discount?.IsByRate ?? false) == true)
      {
        discountedBCAmount -=(((discount?.Discount ?? 0) * bcCost)/ 100 );
        discountedNBCAmount -=(((discount?.Discount ?? 0) * nbcCost)/ 100 );
      }
      else
      {
        discountedBCAmount -= (discount?.Discount ?? 0);
        discountedNBCAmount -=(discount?.Discount ?? 0);
      }
    }
    this.actualPaidAmount = (bc * discountedBCAmount) + (nbc * discountedNBCAmount);

    this.dynamicBCAvailableSeats = Math.max(0, 
      (this.airlineSearchResponse?.BCSeatsAvailable ?? 0) - (bc));
    this.dynamicNBCAvailableSeats =  Math.max(0,  
      (this.airlineSearchResponse?.NBCSeatsAvailable ?? 0) - (nbc));
  }

  bookTicket(bcSeats?:string, nbcSeats?:string, discountId?:string)
  {
    let bc:number = 0;
    let nbc:number = 0;
    if(((bcSeats ?? null) != null) &&(bcSeats !== ""))
      bc = Number(bcSeats);
    if(((nbcSeats ?? null) != null) &&(nbcSeats !== ""))
      nbc = Number(nbcSeats);
    if(bc + nbc <= 0)
    {
      alert("please select seats");
      return;
    }
    this.claculateActualAmount(bcSeats, nbcSeats, discountId);

    let request = this.prepareBookingRequest(this.airlineSearchResponse?.AirlineSchedules?.Id,
     this.airlineSearchResponse?.ActualDepartureDate,
     bc, nbc, this.actualPaidAmount);
     this.bookingService.bookTicket(request, this.headerInfo).subscribe(
      (result) =>{
        if(((result ?? null) != null))
        {
          //success
          let status = result?.bookingStatus ?? '';
          alert(`tickek is booked with status : ${status}`);
          this.saveAndClose();
        }
        else
        {
          //failed
          this.emitError();
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
              this.bookingService.bookTicket(request, this.headerInfo).subscribe(
                (result) =>{
                  if(((result ?? null) != null) )
                  {
                    //success
                    let status = result?.bookingStatus ?? '';
                    alert(`tickek is booked with status : ${status}`);
                    this.saveAndClose();
                  }
                  else
                  {
                    //failed
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

  emitError()
  {
    this.isErrorOutput.emit(true);
    this.closePopup();
  }

  closePopup(){
    this.bookingActionPopupRef.close();
  }

  saveAndClose(){
    this.isSavedTriggered = true;
    this.isSaveTrigger.emit(true);
    this.closePopup();
  }

  cancel()
  {
    this.isCanceled = true;
    this.isCancel.emit(true);
    this.closePopup();
  }


}
