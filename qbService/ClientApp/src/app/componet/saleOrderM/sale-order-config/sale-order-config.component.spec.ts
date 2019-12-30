import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderConfigComponent } from './sale-order-config.component';

describe('SaleOrderConfigComponent', () => {
  let component: SaleOrderConfigComponent;
  let fixture: ComponentFixture<SaleOrderConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
