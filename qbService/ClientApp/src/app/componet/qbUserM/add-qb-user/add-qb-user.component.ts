import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IUser } from '../../../services/user.service';
import { QbUserService} from '../qbUser.service';
import { QbService, IVendors, IEmployee, IOtherName, Customer  } from '../../../services/qb.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-qb-user',
  templateUrl: './add-qb-user.component.html',
  styleUrls: ['./add-qb-user.component.css']
})
export class AddQbUserComponent implements OnInit {
    formGroup: FormGroup;
    user: IUser;
    userID: string;
    type: string[] = ["Employees", "Vendors", "Customers", "Oder Name"];
    lista;
    userSelect: { listID: string; name: string; email: string; };
    vendors: IVendors[] = [];
    employee: IEmployee[] = [];
    otherName: IOtherName[] = [];
    customers: Customer[] = [];
    get Type(): string{
        return this.type[this.formGroup.get('type').value];
    }
    get ListID(): string {
        return this.formGroup.get('listID').value;
    }
    constructor(private _fb: FormBuilder,
        private qbUserService: QbUserService,
        private qbService: QbService,
        private _router: Router) {
        this.formGroup = _fb.group({
            type: new FormControl('', Validators.required),
            listID: new FormControl('', Validators.required),
        });
    }
    a: boolean = true;
    b: boolean = true;
    c: boolean = true;
    d: boolean = true;
    ngOnInit() {

        this.formGroup.disable();
        this.qbService.getAllVendors().subscribe(x => {
            this.vendors = x;
            this.a = false
        }, null, () => this.loadFin());

        this.qbService.getAllEmployee().subscribe(x => {
            this.employee = x;
            this.b = false
        }, null, () => this.loadFin());

        this.qbService.getAllOtherName().subscribe(x => {
            this.otherName = x;
            this.c = false
        }, null, () => this.loadFin());

        this.qbService.getAllCustomers().subscribe(x => {
            this.customers = x;
            this.d = false
        }, null, () => this.loadFin());
    }
    loadFin() {
        if (!this.a && !this.b && !this.c && !this.d) {
            this.formGroup.enable();
            if (this.type.length != 0) {
                this.formGroup.get('type').setValue(0);
                this.onChangeType();
            }
        }
    }
    onChangeType() {
        this.lista = [];
        this.userSelect = undefined;
        this.formGroup.get('listID').setValue("");
        if (this.Type == "Vendors") {
            this.lista = this.vendors.filter(x => x.email!="");
        }

        if (this.Type == "Employees") {
            this.lista = this.employee.filter(x => x.email != "");
        }

        if (this.Type == "Oder Name") {
            this.lista = this.otherName.filter(x => x.email != "");
        }

        if (this.Type == "Customers") {
            this.lista = this.customers.filter(x => x.email != "");
        }
    }
    onChangeName(listID: string) {
        (this.lista as IEmployee[]).forEach(x => {
            if (x.listID == listID) {
                this.userSelect = {
                    listID: x.listID,
                    email: x.email,
                    name: x.name
                };
            }
        });
    }
    save() {
        let user: IUser = {
            block: true,
            companyName: null,
            email: this.userSelect.email,
            id: null,
            password: "holam",
            phone: null,
            roles: null,
            typeUser: this.Type,
            name: this.userSelect.name,
            listID: this.userSelect.listID
        };
        
        this.formGroup.disable();
        this.qbUserService.create(user).subscribe(x => this._router.navigate(['/qbuser']), e => this.formGroup.enable());
    }
    goBack() {
        this._router.navigate(['/qbuser']);
    }
}

