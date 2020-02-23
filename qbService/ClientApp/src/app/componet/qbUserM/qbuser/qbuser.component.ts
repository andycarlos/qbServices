import { Component, OnInit } from '@angular/core';
import { IUser, UserService } from '../../../services/user.service';
import { ConfimationsComponent } from '../../../modalView/confimation/confimation.component';
import { FormGroup } from '@angular/forms';
import { UserManagerService } from '../../userManagerM/user-manager.service';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DomSanitizer } from '@angular/platform-browser';
import { QbRolUserComponent } from '../modalView/rol-user/rol-user.component';
import { ChangePassUserComponent } from '../../../modalView/change-pass-user/change-pass-user.component';

@Component({
  selector: 'app-qbuser',
  templateUrl: './qbuser.component.html',
  styleUrls: ['./qbuser.component.css']
})
export class QbUserComponent implements OnInit {

    userList: IUser[] = [];
    //RolesList: IRole[] = [];
    load: boolean = false;
    formGroupPass: FormGroup;
    filter: string = "";
    pageSize: number = 10;
    page: number = 1;
    messageSuccess: boolean = true;
    constructor(private _userManagerService: UserManagerService,
        private _userService: UserService,
        private _router: Router,
        private modalService: NgbModal,
        private _sanitized: DomSanitizer) { }

    ngOnInit() {
        this.load = true;
        this._userService.getAllUser().subscribe(x => {
            this.userList = x;
            this.load = false;
            this.ordenByUser("email")
        });
    }

    timerOut;
    linkAddUser() {
        if (this.userList.length <= 4) {
            this._router.navigate(['/qbuser/add']);
        } else {
            clearTimeout(this.timerOut);
            this.messageSuccess = false;
            this.timerOut = setTimeout(() => {
                console.log(1);
                this.messageSuccess = true;
            }, 5000);
        }
    }
    alertClose() {
        clearTimeout(this.timerOut);
        this.messageSuccess = true;
    }

    ordernEmail: string;
    ordernName: string;
    ordernType: string;
    ordenAsed: string;
    ordenByUser(userProperty: string) {
        if (this.ordenAsed !== userProperty) {
            this.ordernEmail = "";
            this.ordernName = "";
            this.ordernType = "";
            switch (userProperty) {
                case "name":
                    this.ordernName = "↑";
                    break;
                case "email":
                    this.ordernEmail = "↑";
                    break;
                case "typeUser":
                    this.ordernType = "↑";
                    break;
                default:
            }
            this.userList.sort((a: IUser, b: IUser) => (a[userProperty].toUpperCase() > b[userProperty].toUpperCase()) ? 1 : (a[userProperty].toUpperCase() < b[userProperty].toUpperCase()) ? -1 : 0);
            this.ordenAsed = userProperty;
        }
        else {
            this.ordernEmail = "";
            this.ordernName = "";
            this.ordernType = "";
            switch (userProperty) {
                case "name":
                    this.ordernName = "↓";
                    break;
                case "email":
                    this.ordernEmail = "↓";
                    break;
                case "typeUser":
                    this.ordernType = "↓";
                    break;
                default:
            }
            this.userList.sort((a: IUser, b: IUser) => (a[userProperty].toUpperCase() < b[userProperty].toUpperCase()) ? 1 : (a[userProperty].toUpperCase() > b[userProperty].toUpperCase()) ? -1 : 0);
            this.ordenAsed = "";
        }
    }

    editRolUser(user: IUser) {
        const modalRef = this.modalService.open(QbRolUserComponent);
        modalRef.componentInstance.name = user.email;
        modalRef.componentInstance.userSelect = user;
    }

    delect(user: IUser) {
        const modalRef = this.modalService.open(ConfimationsComponent);
        modalRef.componentInstance.title = "Profile deletion!";
        modalRef.componentInstance.body = this._sanitized.bypassSecurityTrustHtml(`<strong>Are you sure you want to delete <span class="text-danger">"${user.email}"</span> profile?</strong>`);
        modalRef.result.then(() => {
            this._userManagerService.delect(user).subscribe(x => {
                this.userList = this.userList.filter(u => u.email != user.email);
            });
        }, () => { });
    }

    editPasswordUser(user: IUser) {
        const modalRef = this.modalService.open(ChangePassUserComponent);
        modalRef.componentInstance.userSelect = user;
    }

}
