import { WeekDay } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Airlines, AirlineSchedules, AirlineScheduleTracker, AirlinesSearchRequest, AirlinesSearchResponse, DiscountTags } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';

@Component({
  selector: 'app-book-ticket',
  templateUrl: './book-ticket.component.html',
  styleUrls: ['./book-ticket.component.css']
})
export class BookTicketComponent implements OnInit {

  @Input() data :any;
  //@ViewChild(AddOrEditAirlineComponent, { static : true }) addEditAirlineComp!: AddOrEditAirlineComponent;

  isAddEditError:boolean = false;
  isAddEditCancel:boolean = false;
  isAddEditSaved:boolean = false;
  confirmDelete:boolean = false;
  

  alert?:{type:string|null, message:string|null}|null;

  airlines?:Airlines[]|null;
  airlinesSearchResponse?:AirlinesSearchResponse[]|null;
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
    this.airlinesSearchResponse = [];
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

  // getAirlineSchedulesList(airlineSchedulesList:any[]|null)
  // {
  //   if(((airlineSchedulesList ?? null) != null) && ((airlineSchedulesList?.length ?? 0) > 0) )
  //   {
  //     this.airlineSchedules = [];
  //     airlineSchedulesList?.forEach(x => 
  //     {
  //       this.airlineSchedules?.push(this.getAirlineSchedule(x));
  //     });
  //   }
  // }

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

  getDiscountTagList(discountTags:any[]|null)
  {
    let discountTagsRes:DiscountTags[] = [];
    if(((discountTags ?? null) != null) && ((discountTags?.length ?? 0) > 0) )
    {
      discountTags?.forEach(x => 
      {
        discountTagsRes.push(this.getDiscountTag(x));
      });
    }
    return discountTagsRes;
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

  getAvailableAirlinesList(availableAirlines:any[]){
    this.airlinesSearchResponse = [];
    if(((availableAirlines ?? null) != null) && ((availableAirlines?.length ?? 0) > 0) )
    {
      availableAirlines?.forEach(x => 
      {
        this.airlinesSearchResponse?.push(this.getAvailableAirlines(x));
      });
    }
  }

  getAvailableAirlines(availableAirline:any)
  {
    
    let res:AirlinesSearchResponse = {
      AirlineSchedules: this.getAirlineSchedule(availableAirline.airlineSchedules),
      ActualDepartureDate : availableAirline.actualDepartureDate,
      ActualArrivalDate : availableAirline.actualArrivalDate,
      DiscountTags : this.getDiscountTagList(availableAirline.discountTags),
      BCSeatsAvailable : availableAirline.bcSeatsAvailable,
      NBCSeatsAvailable: availableAirline.nbcSeatsAvailable
    }
    return res;
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
  }

  prepareAirlineSearchRequest(from?:string, to?:string, departureDate?:string){
    let res:AirlinesSearchRequest ={
      From: from,
      To:to
    };
    let d = new Date(departureDate ?? '');
    res.DepartureDate = d;
    return res;
  }

  getAvailableAirlineSearch(from:string, to:string, departureDate:string){
    let request = this.prepareAirlineSearchRequest(from, to, departureDate);
    this.airlineService.getAvailableAirlines(request, this.headerInfo).subscribe(
      (result) =>{
        this.getAvailableAirlinesList(result);
        if(((this.airlines ?? null) == null) || ((this.airlines?.length ?? 0) <= 0) )
        {
          //this.alert = {type : "danger", message : "internal server error"};
        }
        else
        {
          //success loaded
        }
      },
      (error) => {
        if(error.status == 401)
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
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.getAvailableAirlines(request, this.headerInfo).subscribe(
                (result) =>{
                  this.getAvailableAirlinesList(result);
                  if(((this.airlines ?? null) == null) || ((this.airlines?.length ?? 0) <= 0) )
                  {
                    //this.alert = {type : "danger", message : "internal server error"};
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

  bookTicket(scheduleId?:number, dateBookedFor?:Date, discount?:string ){
    // console.log("scheduleId : " , scheduleId);
    // console.log("dateBookedFor : ", dateBookedFor);
    // console.log("discount : ", discount);
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
    // addAirlinePopupRef.componentInstance.isCancel.subscribe(
    //   isSaved => this.isAddEditSaved = isSaved
    // );
    addAirlinePopupRef.afterClosed().subscribe(
      (result) => {
        if(!this.isAddEditCancel)
        {
          // if(this.isAddEditSaved)
          // {
          //   this.loadAirlines();
          // }
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

  logout()
  {
    localStorage.clear();
    this.router.navigateByUrl("users/login");
  }

  closeAlert() {
    this.alert = null;
  }

}
