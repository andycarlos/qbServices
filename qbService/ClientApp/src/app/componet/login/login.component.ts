import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';
import { UserService, IUser, IEmail } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, of } from 'rxjs';
import { QbService } from '../../services/qb.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

    constructor(private _userService: UserService,
        private _fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute,
        private _qbService: QbService) {

        this.formGroup = _fb.group({
            email: ['', Validators.required, this.emalDuplication()],
            password: ['', [Validators.required, Validators.minLength(5)]]
        });
    }

    emalDuplication(): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/.test(control.value) == false) {
                return of({ 'isValidEmail': true });
            }
            else {
                this.error = null;
                return of(null);
            }
        }
    }

    formGroup: FormGroup;
    error;
    submit() {

        let loginInfo = this.formGroup.value as IUser;
        this.formGroup.disable();
        this._userService.login(loginInfo).subscribe(x => {
            if (x["token"] !== undefined) {
                this._userService.userEmail = loginInfo.email;
                this._userService.tokenUserExpiration = x.expiration;
                this._userService.token = x.token;
                this._userService.type = x.type;
                this._userService.userListID = x.listID;
                localStorage.setItem("User", loginInfo.email);
                localStorage.setItem("Pass", loginInfo.password);///
                this._userService.access(x.roles);

                if (this._userService.type !="Customers" && this._userService.userListID != null) {
                    this._qbService.getAllSalesRep().subscribe(rp => {
                        rp.forEach(rpx => {
                            if (rpx.userListID == this._userService.userListID)
                                this._userService.SaleRepListID = rpx.saleRepListID;
                        });
                    }, null, () => {
                            this._userService.loginNow = true;
                            this._router.navigate(['/home']);
                    });
                }
                else {
                    this._userService.SaleRepListID = null
                    this._userService.loginNow = true;
                    this._router.navigate(['/home']);
                }
            }
            else {
                this.formGroup.enable();
                this.error = x;
            }
        });
    }
    ngOnInit() {
        this._route.queryParams.subscribe(paras => {
            let email = (paras['email']) ? paras['email'] : localStorage.getItem("User");
            let pass = localStorage.getItem("Pass");///

            if (email)
                this.formGroup.get('email').setValue(email);
            if (pass)
                this.formGroup.get('password').setValue(pass);///
        });
    }
    forgodtpassword() {
        let email: IEmail = { email: this.formGroup.get('email').value };
        if (this.formGroup.get("email").valid) {
            this._userService.forgotPassword(email).subscribe(x => {
                this.error = "Cheque you Email for continue";
            });
        }
        else
            this.error = "Required correct Email.";
    }

}
