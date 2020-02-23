import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '../../services/auth-guard.service';
import { QbUserComponent } from './qbuser/qbuser.component';
import { AddQbUserComponent } from './add-qb-user/add-qb-user.component';

const routes: Routes = [
    { path: "", component: QbUserComponent, canActivate: [AuthGuardService] },
    { path: "add", component: AddQbUserComponent, canActivate: [AuthGuardService] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class QbUserRoutingModule { }
