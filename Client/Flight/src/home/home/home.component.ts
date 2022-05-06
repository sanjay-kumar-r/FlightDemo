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


  readonly subTabComponentImports:{name:string, value:any}[];

  router:Router

  constructor(router:Router,
    dynamicComponent: DynamicComponentService
    // ,
    // private viewContainerRef: ViewContainerRef
    ) {
    let user = JSON.parse(localStorage.getItem("userDetails") ?? '{}');
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
    }
    console.log(this.userDetails);

    this.selectedTab = "administration";
    this.selectedSubTab = "Airlines";

    this.dynamicComponent = dynamicComponent;
    //viewContainerRef.clear();
    //this.viewContainerRef = viewContainerRef;
    //this.componentLoaderDirective.viewContainerRef = viewContainerRef;
    //this.viewContainerRef = this.componentLoaderDirective.viewContainerRef;

    // const viewContainerRef = this.componentLoaderDirective.viewContainerRef;
    // viewContainerRef.clear();

    this.subTabComponentImports = [
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

    this.router = router;

    // this.loadSubTab(this.selectedSubTab);
  }

  ngOnInit(): void {
    //check for session variable 
    //if not then logout
    if(((localStorage.getItem("userId") ?? null) == null)
    || ((localStorage.getItem("authResponse") ?? null) == null)
    || ((JSON.parse(localStorage.getItem("authResponse") ?? '{}')?.token ?? null) == null ))
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
    switch(subTab.toLocaleLowerCase())
    {
      case "Airlines".toLocaleLowerCase() :{
        //update selected subtabs
        //call airlines component
        this.selectedSubTab = "subTab";
        let airlineComponent = this.subTabComponentImports.find(x => 
          x.name.toLocaleLowerCase() == subTab.toLocaleLowerCase())?.value;
        let data = {value:this.selectedSubTab};
        this.loadComponent(airlineComponent, data);
        break;
      }
      case "Discounts".toLocaleLowerCase() :{
        //update selected subtabs
        //call Discounts component
        this.selectedSubTab = "subTab";
        let discountComponent = this.subTabComponentImports.find(x => 
          x.name.toLocaleLowerCase() == subTab.toLocaleLowerCase())?.value;
        let data = {value:this.selectedSubTab};
        this.loadComponent(discountComponent, data);
        break;
      }
      case "BookTicket".toLocaleLowerCase() :{
        //update selected subtabs
        //call BookTicket component
        break;
      }
      case "BookingHistory".toLocaleLowerCase() :{
        //update selected subtabs
        //call BookingHistory component
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
