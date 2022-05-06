import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrEditAirlineComponent } from './add-or-edit-airline.component';

describe('AddOrEditAirlineComponent', () => {
  let component: AddOrEditAirlineComponent;
  let fixture: ComponentFixture<AddOrEditAirlineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddOrEditAirlineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrEditAirlineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
