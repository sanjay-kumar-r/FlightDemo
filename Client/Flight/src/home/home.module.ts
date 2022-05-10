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
import { AirlineDiscountTagMappingComponent } from './airline-discount-tag-mapping/airline-discount-tag-mapping.component';
import { AirlineScheduleComponent } from './airline-schedule/airline-schedule.component';
import { AddOrEditAirlineScheduleComponent } from './add-or-edit-airline-schedule/add-or-edit-airline-schedule.component';
import { BookingsComponent } from './bookings/bookings.component';
import { BookTicketComponent } from './book-ticket/book-ticket.component';
import { BookingActionComponent } from './booking-action/booking-action.component';


@NgModule({
  declarations: [
    HomeComponent,
    AirlinesComponent,
    DiscountsComponent,
    ComponentLoaderDirective,
    AddOrEditAirlineComponent,
    DeleteConfirmationComponent,
    AddOrEditDiscountTagComponent,
    AirlineDiscountTagMappingComponent,
    AirlineScheduleComponent,
    AddOrEditAirlineScheduleComponent,
    BookingsComponent,
    BookTicketComponent,
    BookingActionComponent
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
