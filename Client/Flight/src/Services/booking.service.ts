import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Bookings } from 'src/Models/Bookings';
import { APIRequestType, HeaderInfo } from 'src/Models/HeaderInfo';
import { UserAuthRoutingModule } from 'src/user-auth/user-auth-routing.module';
import { ApiExecutorService } from './api-executor.service';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  baseUrl:string;

  getBookingsUrl:string;
  getBookingsByFilterConditionUrl:string;
  bookTicketUrl:string;
  cancelBookingUrl:string;

  //apiExecutor:ApiExecutorService;
  
  constructor(private apiExecutor:ApiExecutorService){
    this.baseUrl = "http://localhost:9050";

    this.getBookingsUrl = this.baseUrl.trim() + '/' + "api/v1.0/Bookings";
    this.getBookingsByFilterConditionUrl = this.baseUrl.trim() + '/' + "api/v1.0/Bookings/GetBookingsByFiltercondition";
    this.bookTicketUrl = this.baseUrl.trim() + '/' + "api/v1.0/Bookings/BookTicket";
    this.cancelBookingUrl = this.baseUrl.trim() + '/' + "api/v1.0/Bookings/CancelBooking";
  }

  getBookings(booking:Bookings|null, headerInfo:HeaderInfo, id:number = 0) : Observable<any>
  {
    if((booking ?? null) == null)
    {
      let url = this.getBookingsUrl;
      if(id !== 0)
        url = url + "/" + id;
      return this.apiExecutor.CallAPI(APIRequestType.Get, url, headerInfo ,
        null, true);
    }
    else
    {
      return this.apiExecutor.CallAPI(APIRequestType.Post, this.getBookingsByFilterConditionUrl, headerInfo ,
        booking, true);
    }
  }

  bookTicket(booking:Bookings, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.bookTicketUrl, headerInfo ,
      booking, true);
  }

  cancelBooking(id:number, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.cancelBookingUrl, headerInfo ,
      id, true);
  }

}
