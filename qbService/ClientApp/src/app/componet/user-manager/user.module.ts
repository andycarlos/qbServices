import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserRoutingModule } from './user-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
//componet
import { UserManagerComponent } from './listUser/user-manager.component';
import { AddUserComponent } from './add-user/add-user.component';
import { RolUserComponent } from './rol-user/rol-user.component';
//ng-boostrap
import { NgbModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
//pipe
import { FilterUserPipe } from './userFilter.pipe';
@NgModule({
    declarations: [
        UserManagerComponent,
        AddUserComponent,
        FilterUserPipe,
        RolUserComponent
    ],
  imports: [
      CommonModule,
      UserRoutingModule,
      ReactiveFormsModule,
      FormsModule,
      NgbModule,
      NgbPaginationModule
    ],
    entryComponents:[RolUserComponent],
})
export class UserModule { }
