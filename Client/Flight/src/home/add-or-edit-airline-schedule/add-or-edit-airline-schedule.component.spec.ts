import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrEditAirlineScheduleComponent } from './add-or-edit-airline-schedule.component';

describe('AddOrEditAirlineScheduleComponent', () => {
  let component: AddOrEditAirlineScheduleComponent;
  let fixture: ComponentFixture<AddOrEditAirlineScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddOrEditAirlineScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrEditAirlineScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
