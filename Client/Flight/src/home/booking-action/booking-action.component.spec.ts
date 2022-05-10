import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookingActionComponent } from './booking-action.component';

describe('BookingActionComponent', () => {
  let component: BookingActionComponent;
  let fixture: ComponentFixture<BookingActionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BookingActionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BookingActionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
