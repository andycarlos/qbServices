import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaleOrderRoutingModule } from './saleOrder-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
//componet
import { SaleOrderComponent } from './sale-order/sale-order.component';
import { SaleOrderConfigComponent } from './sale-order-config/sale-order-config.component';
//pipe
import { DescByItemPipe, AmountByItemPipe, AmountTotalPipe, AmountTotalByInvocePipe } from './saleOrder.pipe';
//ngboostrap
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
@NgModule({
    declarations: [SaleOrderComponent,
        DescByItemPipe,
        AmountByItemPipe,
        AmountTotalPipe,
        AmountTotalByInvocePipe,
        SaleOrderConfigComponent
    ],
  imports: [
      CommonModule,
      SaleOrderRoutingModule,
      FormsModule,
      ReactiveFormsModule,
      NgbModule
    ],
    entryComponents:[]
})
export class SaleOrderMModule { }
