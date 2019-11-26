import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators, AsyncValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IUser } from '../../../services/user.service';
import { UserManagerService } from '../user-manager.service';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.css']
})
export class AddUserComponent implements OnInit {

    formGroup: FormGroup;
    user: IUser;
    userID: string;

    constructor(private _fb: FormBuilder,
        private _router: Router,
        private _userManagerService: UserManagerService,
        private _activateRouter: ActivatedRoute) {
        this.formGroup = _fb.group({
            companyName: new FormControl('', Validators.required),
            email: new FormControl('', [Validators.required], this.emalDuplication()),
            phone: new FormControl('', [Validators.required, Validators.pattern("[0-9]{5,14}")]),
        });
    }

    emalDuplication(): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/.test(control.value)) {
                return this._userManagerService.AnyUserByEmail(control.value).pipe(
                    map(user => {
                        if (this.userID)
                            return (user && control.value !== this.user.email) ? { 'anyEmail': true } : null;
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
        this._activateRouter.params.subscribe(param => {
            if (!param['id']) {
                return;
            }
            this.userID = param['id'];
            this._userManagerService.getUserByID(this.userID).subscribe(user => {
                this.user = user;
                this.formGroup.patchValue({
                    companyName: user.companyName,
                    email: user.email,
                    phone: user.phone
                });
            });
        });
    }
    load: boolean = false;
    save() {
        let loginInfo = this.formGroup.value as IUser;
        loginInfo.password = "holam"
        this.load = true;
        this.formGroup.disable();
        if (!this.userID) {
          this._userManagerService.create(loginInfo).subscribe(null, err => console.log(err), () => {
              this.load = false;
              this.goBack();
          });
        }
        else {
            loginInfo.id = this.userID;
            this._userManagerService.updateUser(loginInfo).subscribe(null, err => console.log(err), () => {
                this.load = false;
                this.goBack();
            });
        }
    }
    goBack() {
        this._router.navigate(['/user']);
    }

    keyPress(event: any) {
        const pattern = /[0-9]/;
        console.log(event.keyCode);

        let inputChar = String.fromCharCode(event.charCode);
        if ((event.keyCode != 8 || event.keyCode != 13) && !pattern.test(inputChar)) {
            event.preventDefault();
        }
    }

}
