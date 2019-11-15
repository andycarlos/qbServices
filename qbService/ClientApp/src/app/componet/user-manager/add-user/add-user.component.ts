import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators, AsyncValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IUser, UserService } from '../../../services/user.service';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.css']
})
export class AddUserComponent implements OnInit {

    formGroup: FormGroup;
    constructor(private _fb: FormBuilder,
        private _router: Router,
        private _userService: UserService) {
        this.formGroup = _fb.group({
            companyName: new FormControl('', Validators.required),
            email: new FormControl('', [Validators.required], this.emalDuplication()),
            phone: new FormControl('', Validators.required),
        });
    }
    validation
    emalDuplication(): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/.test(control.value)) {
                return this._userService.AnyUserByEmail(control.value).pipe(
                    map(user => {
                        return (user) ? { 'anyEmail': true } : null;
                    })
                );
            }
            else {
                return of({ 'isValidEmail': true });
            }
        }
    }

    ngOnInit() {
    }
    load: boolean = false;
    save() {
        let loginInfo = this.formGroup.value as IUser;
        loginInfo.password = "holam"
        this.load = true;
        this.formGroup.disable();
        this._userService.create(loginInfo).subscribe(null, err => console.log(err), () => {
            this.goBack();
            this.load = false;
        });
    }
    goBack() {
        this._router.navigate(['/user']);
    }

}
