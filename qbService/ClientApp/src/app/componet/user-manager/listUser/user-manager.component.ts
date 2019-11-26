import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { IUser, IRole } from '../../../services/user.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RolUserComponent } from '../rol-user/rol-user.component';
import { UserManagerService } from '../user-manager.service';

@Component({
  selector: 'app-user-manager',
  templateUrl: './user-manager.component.html',
  styleUrls: ['./user-manager.component.css']
})
export class UserManagerComponent implements OnInit {

    userList: IUser[] = [];
    //sectionList: ISection[] = [];
    RolesList: IRole[] = [];
    load: boolean = false;
    formGroupPass: FormGroup;
    filter: string = "";
    pageSize: number = 10;
    page: number = 1;

    constructor(private _userService: UserManagerService,
        private _router: Router,
        private modalService: NgbModal) { }

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

}
