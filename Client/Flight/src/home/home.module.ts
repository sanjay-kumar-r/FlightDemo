import { NgModule, ViewContainerRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home/home.component';
import { AirlinesComponent } from './airlines/airlines.component';
import { DiscountsComponent } from './discounts/discounts.component';
import { ComponentLoaderDirective } from './component-loader.directive';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgbModule, NgbPaginationModule, NgbAlertModule, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { AddOrEditAirlineComponent } from './add-or-edit-airline/add-or-edit-airline.component';
import { DeleteConfirmationComponent } from './delete-confirmation/delete-confirmation.component';
import { AddOrEditDiscountTagComponent } from './add-or-edit-discount-tag/add-or-edit-discount-tag.component';


@NgModule({
  declarations: [
    HomeComponent,
    AirlinesComponent,
    DiscountsComponent,
    ComponentLoaderDirective,
    AddOrEditAirlineComponent,
    DeleteConfirmationComponent,
    AddOrEditDiscountTagComponent
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    NgbModule,
    NgbPaginationModule,
    NgbAlertModule,
    ReactiveFormsModule,
    FormsModule
  ],
  entryComponents: [ComponentLoaderDirective],
})
export class HomeModule { }
