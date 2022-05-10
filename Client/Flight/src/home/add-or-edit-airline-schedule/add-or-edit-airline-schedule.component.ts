import { Time, WeekDay } from '@angular/common';
import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Airlines, AirlineSchedules } from 'src/Models/Airlines';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';
import { AirlineScheduleComponent } from '../airline-schedule/airline-schedule.component';

@Component({
  selector: 'app-add-or-edit-airline-schedule',
  templateUrl: './add-or-edit-airline-schedule.component.html',
  styleUrls: ['./add-or-edit-airline-schedule.component.css']
})
export class AddOrEditAirlineScheduleComponent implements OnInit {

  @Output() airlineSaveResult : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isErrorOutput : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isCancel : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isSaveTrigger : EventEmitter<boolean> = new EventEmitter<boolean>();

  isEdit:boolean = false;
  scheduleId?:number|null;
  airline?:Airlines|null;
  airlineSchedule?:AirlineSchedules|null;

  isFormSubmitted:boolean = false;
  isDuplicate:boolean = false;
  invalidSelection:boolean = false;
  invalidDate:boolean = false;
  isCanceled:boolean = false;
  isSavedTriggered:boolean = false;
  weekDays:{key:string,value:number}[];
  currentDate:Date;

  departureTimeDisplay:string;
  arrivalTimeDisplay:string;

  airlines?:Airlines[]|null;
  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(@Inject(MAT_DIALOG_DATA) public addEditAirlineScheduleDate:any,
    private addEditAirlineScheduleCompRef: MatDialogRef<AddOrEditAirlineScheduleComponent>,
    private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router
    ) 
  {
    this.scheduleId = 0;
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

    this.departureTimeDisplay = "";
    this.arrivalTimeDisplay = "";

    this.weekDays = [];
    for(let day in WeekDay)
    {
      if (!isNaN(Number(WeekDay[day]))) {
        this.weekDays.push({key:day, value:Number(WeekDay[day])});
      }
      // this.weekDays.push({key:day, value:Number(WeekDay[day])});
    }
    //this.weekDays.find(x => x.key="");
    // (this.weekDays.find(x => x.key==(this.airlineSchedule?.DepartureDay ?? ''))?.value ?? -1)
    this.currentDate = new Date();
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
    return this.airline;
  }

  getAirlineSchedule(airlineSchedule:any)
  {
    this.airlineSchedule = {
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

  }

  prepareAirlineScheduleBodyFromForm(airlineScheduleForm:any)
  {
    let airlineScheduleRequest:AirlineSchedules = {
      AirlineId : airlineScheduleForm.airlineSelect,
      From: airlineScheduleForm.from,
      To: airlineScheduleForm.to,
      IsRegular: airlineScheduleForm.isRegular ?? false,
      DepartureDay: undefined,
      DepartureDate: undefined,
      DepartureTime: (airlineScheduleForm.departureTime ?? ''),
      ArrivalDay: undefined,
      ArrivalDate: undefined,
      ArrivalTime: (airlineScheduleForm.arrivalTime ?? '')
    };
    if(airlineScheduleRequest.IsRegular)
    {  
      airlineScheduleRequest.DepartureDay = (((airlineScheduleForm?.departureDay ?? null) != null) && airlineScheduleForm?.departureDay >= 0 ) ? airlineScheduleForm.departureDay : undefined;
    }
    else
    {
      airlineScheduleRequest.DepartureDate = airlineScheduleForm.departureDate;
    }
    if(airlineScheduleRequest.IsRegular)
    {
      airlineScheduleRequest.ArrivalDay = (((airlineScheduleForm?.arrivalDay ?? null) != null) && airlineScheduleForm?.arrivalDay >= 0 ) ? airlineScheduleForm.arrivalDay : undefined;
    }
    else
    {
      airlineScheduleRequest.ArrivalDate = airlineScheduleForm.arrivalDate;
    }
    //let t1:Time = airlineScheduleForm.departureTime;
    let d1:Date = new Date();
    d1.setHours(airlineScheduleForm.departureTime.split(":")[0]);
    d1.setMinutes(airlineScheduleForm.departureTime.split(":")[1]);
    d1.setSeconds(0);
    airlineScheduleRequest.DepartureTime = d1;

    //let t2:Time = airlineScheduleForm.arrivalTime;
    let d2:Date = new Date();
    d2.setHours(airlineScheduleForm.arrivalTime.split(":")[0]);
    d2.setMinutes(airlineScheduleForm.arrivalTime.split(":")[0]);
    d2.setSeconds(0);
    airlineScheduleRequest.ArrivalTime = d2;

    if(this.isEdit)
    {
      airlineScheduleRequest.Id = this.scheduleId ?? 0;
    }
    else
    {

    }
    this.airlineSchedule = airlineScheduleRequest;
  }

  getAirlinesList(airlineList:any[]|null)
  {
    if(((airlineList ?? null) != null) && ((airlineList?.length ?? 0) > 0) )
    {
      this.airlines = [];
      airlineList?.forEach(x => 
      {
        this.airlines?.push(this.getAirlineRes(x));
      });
    }
  }

  getAirlineRes(airline:any)
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

  ngOnInit(): void {
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();
    if((this.addEditAirlineScheduleDate ?? null) != null)
    {
      this.isEdit = this.addEditAirlineScheduleDate?.isEdit ?? false;
      this.scheduleId = this.addEditAirlineScheduleDate?.scheduleId ?? 0;
      if(!this.isEdit)
      {
        this.airline = {
          TotalSeats:0,
          TotalBCSeats:0,
          TotalNBCSeats:0,
          BCTicketCost:0,
          NBCTicketCost:0,
        };
        //get airline list
        this.loadAirlines();
      }
      else
      {
        //edit
        if((this.scheduleId ?? 0) >= 0)
        {
          //get airline
          this.airlineService.getSchedules(this.scheduleId ?? 0, this.headerInfo)
          .subscribe(
            (result) => {
              this.getAirlineSchedule(result[0]);
              if(((this.airlineSchedule ?? null) != null) && ((this.airlineSchedule?.Id ?? 0) == this.scheduleId))
              {
                //valid
                this.departureTimeDisplay = this.getDepartureTime();
                this.arrivalTimeDisplay = this.getArrivalTime();
              }
              else
              {
                this.emitError();
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
                    this.airlineService.getSchedules(this.scheduleId ?? 0, this.headerInfo).subscribe(
                      (result) =>{
                        this.getAirlineSchedule(result[0]);
                        if(((this.airlineSchedule ?? null) != null) && ((this.airlineSchedule?.Id ?? 0) == this.scheduleId))
                        {
                          //valid
                          this.departureTimeDisplay = this.getDepartureTime();
                          this.arrivalTimeDisplay = this.getArrivalTime();
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
          console.error("airlineSchedule id is not present");
          this.emitError();
        }
      }
    }
    else
    {
      this.emitError();
    }
  }

  loadAirlines(){
    this.airlineService.getAirlines(null, this.headerInfo).subscribe(
      (result) =>{
        this.getAirlinesList(result);
        if(((this.airlines ?? null) == null) || ((this.airlines?.length ?? 0) <= 0) )
        {
          this.emitError();
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
                    this.emitError();
                  }
                  else
                  {
                    //success loaded
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
          this.emitError();
        }
      }
    );
  }


  saveAirlineSchedule(airlineScheduleForm:any){
    //console.log(airlineForm);
    if(!this.isCanceled)
    {
      if(this.isSavedTriggered)
        return;
      this.isSavedTriggered = true;
      if(airlineScheduleForm.valid)
      {
        this.isFormSubmitted = true;
          this.prepareAirlineScheduleBodyFromForm(airlineScheduleForm.value);
          if(!this.isEdit)
            this.addAirlineSchedule(this.airlineSchedule);
          else
            this.editAirlineSchedule(this.airlineSchedule);
        
      }
      else
      {
        console.error("form is invalid");
      }
    }
  }

  addAirlineSchedule(airlineScheduleRequest:any){
    this.airlineService.addSchedule(airlineScheduleRequest, this.headerInfo).subscribe(
      (result) => {
        if((result ?? null) != null)
        {
          alert("airline schedule details added successfully");
          this.saveAndClose();
        }
        else{
          this.emitError();
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
              this.airlineService.addSchedule(airlineScheduleRequest, this.headerInfo).subscribe(
                (result) =>{
                  if((result ?? null) != null)
                  {
                    alert("airline schedule details added successfully");
                    this.saveAndClose();
                  }
                  else{
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

  editAirlineSchedule(airlineScheduleRequest:any){
    this.airlineService.reSchedulesAirlinesByRange([airlineScheduleRequest], this.headerInfo).subscribe(
      (result) => {
        if(((result ?? null) != null))
        {
          alert("airline schedule details updated successfully");
          this.closePopup();
        }
        else{
          this.emitError();
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
              this.airlineService.reSchedulesAirlinesByRange(airlineScheduleRequest, this.headerInfo).subscribe(
                (result) =>{
                  if(((result ?? null) != null) && result?.res == true)
                  {
                    alert("airline schedule details updated successfully");
                    this.closePopup();
                  }
                  else{
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
    this.addEditAirlineScheduleCompRef.close();
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

  getDepartureDay(){
    return (this.weekDays.find(x => x.key==(this.airlineSchedule?.DepartureDay ?? ''))?.value ?? -1);
  }

  getArrivalDay(){
    return (this.weekDays.find(x => x.key==(this.airlineSchedule?.ArrivalDay ?? ''))?.value ?? -1);
  }

  getDepartureTime(){
    // let y:Date = new Date(); 
    // let hr = y.getHours();
    // let min = y.getMinutes();

    let x:string = this.airlineSchedule?.DepartureTime?.toString() ?? ""; 
    let z:Date = new Date(x); 
    let hr1 = z.getHours();
    let hr2 = hr1.toString();
    if(hr1.toString().length == 1)
      hr2 = "0"+hr1;
    let min1 = z.getMinutes();
    let min2 = min1.toString();
    if(min2.toString().length == 1)
      min2 = "0"+min1;
    return hr2 + ":" + min2;
    //return this.airlineSchedule?.DepartureTime?.getHours() + ":" + this.airlineSchedule?.DepartureTime?.getMinutes();
  }

  getArrivalTime(){
    // let y:Date = new Date(); 
    // let hr = y.getHours();
    // let min = y.getMinutes();

    let x:string = this.airlineSchedule?.ArrivalTime?.toString() ?? ""; 
    let z:Date = new Date(x); 
    let hr1 = z.getHours();
    let hr2 = hr1.toString();
    if(hr1.toString().length == 1)
      hr2 = "0"+hr1;
    let min1 = z.getMinutes();
    let min2 = min1.toString();
    if(min2.toString().length == 1)
      min2 = "0"+min1;
    return hr2 + ":" + min2;

    //return this.airlineSchedule?.ArrivalTime?.getHours() + ":" + this.airlineSchedule?.ArrivalTime?.getMinutes();
  }

}
