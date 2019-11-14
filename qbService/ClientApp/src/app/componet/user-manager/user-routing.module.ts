import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserManagerComponent } from './listUser/user-manager.component';
import { AddUserComponent } from './add-user/add-user.component';

const routes: Routes = [
    { path: "", component: UserManagerComponent },
    { path: "add", component: AddUserComponent },
    { path: "edit/:id", component: AddUserComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserRoutingModule { }
