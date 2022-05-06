import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { right } from '@popperjs/core';
import { Airlines } from 'src/Models/Airlines';
import { CustomErrorCode } from 'src/Models/Commons';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';

@Component({
  selector: 'app-airlines',
  templateUrl: './airlines.component.html',
  styleUrls: ['./airlines.component.css']
})
export class AirlinesComponent implements OnInit {

  @Input() data :any;

  alert?:{type:string|null, message:string|null}|null;

  airlines?:Airlines[]|null;
  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(private airlineService:AirlineService,
    private router:Router,
    private dynamicComponent: DynamicComponentService) { 
    //console.log("data: ", this.data);
    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
    this.alert = null;
  }

  getUserDetails(user:any)
  {
    if(((user ?? null) != null) && ((user?.id ?? null) != null) && (user.id != 0))
    {
      this.userDetails = {
        Id : user.id,
        FirstName : user.firstName,
        LastName : user.lastName,
        EmailId : user.emailId,
        Password : user.password,
        AccountStatus : user.accountStatus.status,
        IsSuperAdmin : user.isSuperAdmin,
        CreatedOn : new Date(user.createdOn ?? ""),
        ModifiedOn : new Date(user.modifiedOn ?? "")
      };
    }
  }

  getAuthResponse(authResponse:any)
  {
    if(((authResponse ?? null) != null) && ((authResponse?.token ?? null) != null)
      && ((authResponse?.refreshToken ?? null) != null))
    {
      this.authResponse = {
        Token : authResponse.token,
        RefreshToken : authResponse.refreshToken
      };
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
    console.log("data: ", this.data);
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader()

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
            this.alert = {type : "danger", message : "unauthorized"};
            console.log(error);
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
    
  }

  logout()
  {
    localStorage.clear();
    this.router.navigateByUrl("users/login");
  }

}
