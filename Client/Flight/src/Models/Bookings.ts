import { AirlineSchedules } from "./Airlines";

export interface Bookings {
    Id?:number,
    UserId?:number,
    ScheduleId?:number,
    DateBookedFor?:Date,
    BCSeats?:number,
    NBCSeats?:number,
    ActualPaidAmount?:number,
    BookingStatusId?:number,
    BookingStatus?:AccountStatus,
    PNR?:string,
    IsRefunded?:boolean,
    CreatedOn?:Date,
    CanceledOn?:Date
}

export interface AccountStatus {
    Id?:number,
    Status?:string,
    Description?:string
}

export interface BookingDetails {
    booking?:Bookings
    ScheduleDetail?:AirlineSchedules
}

export interface BookingResponse {
    booking?:Bookings
    ScheduleDetail?:AirlineSchedules
}

