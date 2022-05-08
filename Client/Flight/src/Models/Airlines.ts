import { WeekDay } from '@angular/common'

export interface Airlines {
    Id?:number,
    Name?:string,
    AirlineCode?:string,
    ContactNumber?:string,
    ContactAddress?:string,
    TotalSeats:number,
    TotalBCSeats:number,
    TotalNBCSeats:number,
    BCTicketCost:number,
    NBCTicketCost:number,
    IsActive?:boolean,
    CreatedOn?:Date,
    ModifiedOn?:Date
}

export interface DiscountTags {
    Id?:number,
    Name?:string,
    DiscountCode?:string,
    Description?:string,
    Discount:number,
    IsByRate?:boolean,
    IsActive?:boolean,
    CreatedOn?:Date,
    ModifiedOn?:Date
}

export interface AirlineDiscountTagMappingDetails {
    Airline?:Airlines;
    DiscountTags?:DiscountTags[];
}

export interface RemapAirlineDiscountTagsDetails {
    AirlineId?:number;
    AddedDiscountTagIds?:number[];
    RemovedDiscountTagIds?:number[];
}

export interface AirlineSchedules {
    Id?:number;
    AirlineId?:number;
    Airline?:Airlines;
    From?:string;
    To?:string;
    IsRegular?:boolean;
    DepartureDay?:WeekDay;
    DepartureDate?:Date;
    DepartureTime?:Date;
    ArrivalDay?:WeekDay;
    ArrivalDate?:Date;
    ArrivalTime?:Date;
    CreatedOn?:Date;
    ModifiedOn?:Date;
}

export interface AirlineScheduleTracker {
    Id?:number;
    ScheduleId?:number;
    AirlineSchedule?:AirlineSchedules;
    ActualDepartureDate?:Date;
    ActualArrivalDate?:Date;
    BCSeatsRemaining?:number;
    NBCSeatsRemaining?:number;
}