import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { IUser, IRole } from '../../../services/user.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserManagerService } from '../user-manager.service';
import { RolUserComponent } from '../modalView/rol-user/rol-user.component';
import { ConfimationComponent } from '../../../modalview/confimation/confimation.component';
import { DomSanitizer } from '@angular/platform-browser';
import { ChangePassUserComponent } from '../modalview/change-pass-user/change-pass-user.component';

@Component({
  selector: 'app-user-manager',
  templateUrl: './user-manager.component.html',
  styleUrls: ['./user-manager.component.css']
})
export class UserManagerComponent implements OnInit {

    userList: IUser[] = [];
    //RolesList: IRole[] = [];
    load: boolean = false;
    formGroupPass: FormGroup;
    filter: string = "";
    pageSize: number = 10;
    page: number = 1;

    constructor(private _userService: UserManagerService,
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
    linkAddUser() {
        this._router.navigate(['/user/add']);
    }

    ordernEmail: string;
    ordernCompanyName: string;
    ordernPhone: string;
    ordenAsed: string;
    ordenByUser(userProperty: string) {
        if (this.ordenAsed !== userProperty) {
            this.ordernEmail = "";
            this.ordernCompanyName = "";
            this.ordernPhone = "";
            switch (userProperty) {
                case "companyName":
                    this.ordernCompanyName = "↑";
                    break;
                case "email":
                    this.ordernEmail = "↑";
                    break;
                case "phone":
                    this.ordernPhone = "↑";
                    break;
                default:
            }
            this.userList.sort((a: IUser, b: IUser) => (a[userProperty].toUpperCase() > b[userProperty].toUpperCase()) ? 1 : (a[userProperty].toUpperCase() < b[userProperty].toUpperCase()) ? -1 : 0);
            this.ordenAsed = userProperty;
        }
        else {
            this.ordernEmail = "";
            this.ordernCompanyName = "";
            this.ordernPhone = "";
            switch (userProperty) {
                case "companyName":
                    this.ordernCompanyName = "↓";
                    break;
                case "email":
                    this.ordernEmail = "↓";
                    break;
                case "phone":
                    this.ordernPhone = "↓";
                    break;
                default:
            }
            this.userList.sort((a: IUser, b: IUser) => (a[userProperty].toUpperCase() < b[userProperty].toUpperCase()) ? 1 : (a[userProperty].toUpperCase() > b[userProperty].toUpperCase()) ? -1 : 0);
            this.ordenAsed = "";
        }

    }

    editRolUser(user: IUser) {
        const modalRef = this.modalService.open(RolUserComponent);
        modalRef.componentInstance.name = user.email;
        modalRef.componentInstance.userSelect = user;
    }
    blockUser(user: IUser, event: boolean) {
        user.block = event;
        this._userService.blockUser(user).subscribe();
    }

    editUser(user: IUser) {
        this._router.navigate(['/user/edit', user.id])
    }

    delect(user:IUser) {
        const modalRef = this.modalService.open(ConfimationComponent);
        modalRef.componentInstance.title = "Profile deletion!";
        modalRef.componentInstance.body = this._sanitized.bypassSecurityTrustHtml(`<strong>Are you sure you want to delete <span class="text-danger">"${user.email}"</span> profile?</strong>`);
        modalRef.result.then(() => {
            this._userService.delect(user).subscribe(x => {
                this.userList = this.userList.filter(u => u.email != user.email);
            });
        }, () => { });
    }
    editPasswordUser(user: IUser) {
        const modalRef = this.modalService.open(ChangePassUserComponent);
        modalRef.componentInstance.userSelect = user;
    }

}
