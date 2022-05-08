import { Component, OnInit, OnDestroy, ViewChild, ViewContainerRef, ComponentRef  } from '@angular/core';
import { AuthResponse, Users } from 'src/Models/Users';
import { Router } from '@angular/router';
import { ComponentLoaderDirective } from '../component-loader.directive';
import { DynamicComponentService } from 'src/Services/dynamic-component.service';
import { subscribeOn } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  @ViewChild(ComponentLoaderDirective, { static : true }) componentLoaderDirective!: ComponentLoaderDirective;


  readonly userDetails:Users;
  readonly dynamicComponent:DynamicComponentService;
  //readonly viewContainerRef:ViewContainerRef;
  selectedTab:string;
  selectedSubTab:string;


  readonly adminSubTabComponentImports:{name:string, value:any}[];
  readonly bookingSubTabComponentImports:{name:string, value:any}[];

  router:Router

  constructor(router:Router,
    dynamicComponent: DynamicComponentService
    // ,
    // private viewContainerRef: ViewContainerRef
    ) {
    //let user = JSON.parse(localStorage.getItem("userDetails") ?? '{}');
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
    // }
    this.userDetails =  JSON.parse(localStorage.getItem("userDetails") ?? '{}');
    //console.log(this.userDetails);

    this.selectedTab = "administration";
    this.selectedSubTab = "Airlines";

    this.dynamicComponent = dynamicComponent;
    //viewContainerRef.clear();
    //this.viewContainerRef = viewContainerRef;
    //this.componentLoaderDirective.viewContainerRef = viewContainerRef;
    //this.viewContainerRef = this.componentLoaderDirective.viewContainerRef;

    // const viewContainerRef = this.componentLoaderDirective.viewContainerRef;
    // viewContainerRef.clear();

    this.adminSubTabComponentImports = [
      {
        name:"airlines",
        value: () => import('../airlines/airlines.component').then(
          m => m.AirlinesComponent
        )
      },
      {
        name:"discounts",
        value: () => import('../discounts/discounts.component').then(
          m => m.DiscountsComponent
        )
      }
    ]
    this.bookingSubTabComponentImports = [
      {
        name:"BookTicket",
        value: () => import('../airlines/airlines.component').then(
          m => m.AirlinesComponent
        )
      },
      {
        name:"BookingHistory",
        value: () => import('../airlines/airlines.component').then(
          m => m.AirlinesComponent
        )
      }
  ]
    this.router = router;
    // this.loadSubTab(this.selectedSubTab);
  }

  ngOnInit(): void {
    //check for session variable 
    //if not then logout
    if(((localStorage.getItem("userId") ?? null) == null)
    || ((localStorage.getItem("authResponse") ?? null) == null)
    || ((JSON.parse(localStorage.getItem("authResponse") ?? '{}')?.Token ?? null) == null ))
    {
      this.logout();
      return;
    }
    const viewContainerRef = this.componentLoaderDirective.viewContainerRef;
    viewContainerRef.clear();
    this.loadSubTab(this.selectedSubTab);
  }

  loadComponent(component:any, data:any)
  {
    const viewContainerRef = this.componentLoaderDirective.viewContainerRef;
    viewContainerRef.clear();
    this.dynamicComponent.loadComponents(viewContainerRef, 
      { loadChildren: component }, data)
      .subscribe((componentref:any) => 
      {
        componentref.instance.data = data;
      });
  }

  loadSubTab(subTab:string)
  {
    switch(subTab.toLowerCase())
    {
      case "Airlines".toLowerCase() :{
        //update selected subtabs
        //call airlines component
        this.selectedSubTab = subTab.toLowerCase();
        let airlineComponent = this.adminSubTabComponentImports.find(x => 
          x.name.toLowerCase() == subTab.toLowerCase())?.value;
        let data = {value:this.selectedSubTab};
        this.loadComponent(airlineComponent, data);
        break;
      }
      case "Discounts".toLowerCase() :{
        //update selected subtabs
        //call Discounts component
        this.selectedSubTab = subTab.toLowerCase();
        let discountComponent = this.adminSubTabComponentImports.find(x => 
          x.name.toLowerCase() == subTab.toLowerCase())?.value;
        let data = {value:this.selectedSubTab};
        this.loadComponent(discountComponent, data);
        break;
      }
      case "BookTicket".toLowerCase() :{
        //update selected subtabs
        //call BookTicket component
        break;
      }
      case "BookingHistory".toLowerCase() :{
        //update selected subtabs
        //call BookingHistory component
        break;
      }
    }
  }

  loadMainTab(navTab:string){
    switch(navTab.toLowerCase()){
      case "Administration":{
        this.selectedTab = navTab.toLowerCase();
        this.selectedSubTab = this.adminSubTabComponentImports[0].name.toLowerCase();
        this.loadSubTab(this.selectedSubTab);
        break;
      }
      case "Bookings":{
        this.selectedTab = navTab.toLowerCase();
        this.selectedSubTab = this.bookingSubTabComponentImports[0].name.toLowerCase();
        this.loadSubTab(this.selectedSubTab);
        break;
      }
    }
  }

  logout()
  {
    localStorage.clear();
    this.router.navigateByUrl("users/login");
  }


}
