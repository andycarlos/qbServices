import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '../../services/auth-guard.service';
import { SaleOrderComponent } from './sale-order/sale-order.component';
import { SaleOrderConfigComponent } from './sale-order-config/sale-order-config.component';

const routes: Routes = [
    { path: "", component: SaleOrderComponent, canActivate: [AuthGuardService] },
    { path: "config", component: SaleOrderConfigComponent, canActivate: [AuthGuardService] },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SaleOrderRoutingModule { }
