import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QbUserRoutingModule } from './qbUser-routing.module';
import { QbUserComponent } from './qbuser/qbuser.component';
import { AddQbUserComponent } from './add-qb-user/add-qb-user.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
//componet
import { QbRolUserComponent } from './modalView/rol-user/rol-user.component';
//ng-boostrap
import { NgbPaginationModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
//pipe
import { QbFilterUserPipe } from './qbUserFilter.pipe';
@NgModule({
    declarations: [
        QbUserComponent,
        AddQbUserComponent,
        QbFilterUserPipe,
        QbRolUserComponent,
        ],
  imports: [
      CommonModule,
      QbUserRoutingModule,
      FormsModule,
      ReactiveFormsModule,
      NgbPaginationModule,
      NgbModule
    ],
    entryComponents: [QbRolUserComponent],
})
export class QbUserModule { }
