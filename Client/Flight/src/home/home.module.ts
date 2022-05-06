import { NgModule, ViewContainerRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home/home.component';
import { AirlinesComponent } from './airlines/airlines.component';
import { DiscountsComponent } from './discounts/discounts.component';
import { ComponentLoaderDirective } from './component-loader.directive';

import { NgbModule, NgbPaginationModule, NgbAlertModule, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { AddOrEditAirlineComponent } from './add-or-edit-airline/add-or-edit-airline.component';


@NgModule({
  declarations: [
    HomeComponent,
    AirlinesComponent,
    DiscountsComponent,
    ComponentLoaderDirective,
    AddOrEditAirlineComponent
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    NgbModule,
    NgbPaginationModule,
    NgbAlertModule
  ],
  entryComponents: [ComponentLoaderDirective],
})
export class HomeModule { }
