import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AirlineDiscountTagMappingComponent } from './airline-discount-tag-mapping.component';

describe('AirlineDiscountTagMappingComponent', () => {
  let component: AirlineDiscountTagMappingComponent;
  let fixture: ComponentFixture<AirlineDiscountTagMappingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AirlineDiscountTagMappingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AirlineDiscountTagMappingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
