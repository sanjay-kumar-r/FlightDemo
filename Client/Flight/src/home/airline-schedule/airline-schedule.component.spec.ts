import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AirlineScheduleComponent } from './airline-schedule.component';

describe('AirlineScheduleComponent', () => {
  let component: AirlineScheduleComponent;
  let fixture: ComponentFixture<AirlineScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AirlineScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AirlineScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
