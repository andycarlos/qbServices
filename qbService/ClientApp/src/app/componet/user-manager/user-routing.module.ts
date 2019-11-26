import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserManagerComponent } from './listUser/user-manager.component';
import { AddUserComponent } from './add-user/add-user.component';
import { AuthGuardService } from '../../services/auth-guard.service';

const routes: Routes = [
    { path: "", component: UserManagerComponent, canActivate: [AuthGuardService] },
    { path: "add", component: AddUserComponent, canActivate: [AuthGuardService] },
    { path: "edit/:id", component: AddUserComponent, canActivate: [AuthGuardService] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserRoutingModule { }
