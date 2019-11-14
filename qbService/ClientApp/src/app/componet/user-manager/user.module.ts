import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManagerComponent } from './listUser/user-manager.component';
import { UserRoutingModule } from './user-routing.module';
import { AddUserComponent } from './add-user/add-user.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [
        UserManagerComponent,
        AddUserComponent
    ],
  imports: [
      CommonModule,
      UserRoutingModule,
      ReactiveFormsModule,
    ],
})
export class UserModule { }
