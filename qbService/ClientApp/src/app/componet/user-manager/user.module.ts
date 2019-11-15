import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManagerComponent } from './listUser/user-manager.component';
import { UserRoutingModule } from './user-routing.module';
import { AddUserComponent } from './add-user/add-user.component';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
//ng-boostrap
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
//pipe
import { FilterUserPipe } from './userFilter.pipe';
@NgModule({
    declarations: [
        UserManagerComponent,
        AddUserComponent,
        FilterUserPipe
    ],
  imports: [
      CommonModule,
      UserRoutingModule,
      ReactiveFormsModule,
      FormsModule,
      NgbPaginationModule

    ],
})
export class UserModule { }
