import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrEditDiscountTagComponent } from './add-or-edit-discount-tag.component';

describe('AddOrEditDiscountTagComponent', () => {
  let component: AddOrEditDiscountTagComponent;
  let fixture: ComponentFixture<AddOrEditDiscountTagComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddOrEditDiscountTagComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrEditDiscountTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
