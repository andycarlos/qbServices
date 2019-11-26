import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';
import { UserService, IUser } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

    constructor(private _userService: UserService,
        private _fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute) {

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
        this._userService.login(loginInfo).subscribe(x => {
            if (x["token"] !== undefined) {

                this._userService.userEmail = loginInfo.email;
                this._userService.tokenUserExpiration = x.expiration;
                this._userService.token = x.token;
                localStorage.setItem("User", loginInfo.email);
                localStorage.setItem("Pass", loginInfo.password);///

                this._userService.access(x.roles);
                this._userService.loginNow = true;

                //if (this._userService.rolUser || this._userService.rolAdmin) {
                //    this._router.navigate(['/home']);
                //    return;
                //}

                //if (this._userService.rolFileAdd || this._userService.rolFileDel || this._userService.rolFileDownload) {
                //    this._router.navigate(['/files']);
                //    return;
                //}

                this._router.navigate(['/home']);
            }
            else {
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

}
