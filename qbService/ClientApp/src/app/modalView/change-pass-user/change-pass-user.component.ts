import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IUser } from '../../services/user.service';
import { UserManagerService, IPassWord } from '../../componet/userManagerM/user-manager.service';

@Component({
  selector: 'app-change-pass-user',
  templateUrl: './change-pass-user.component.html',
  styleUrls: ['./change-pass-user.component.css']
})
export class ChangePassUserComponent implements OnInit {

    formGroupPass: FormGroup;
    userSelect: IUser;
    constructor(private _fb: FormBuilder,
        private _userService: UserManagerService,
        public activeModal: NgbActiveModal) {
        this.formGroupPass = _fb.group({
            password: new FormControl('', [Validators.required, Validators.minLength(5)]),
            passwordConf: new FormControl('', [Validators.required])
        }, { validators: this.checkPasswords });
    }
    checkPasswords(group: FormGroup) { // here we have the 'passwords' group
        let pass = group.get('password').value;
        let confirmPass = group.get('passwordConf').value;
        return pass === confirmPass ? null : { notSame: true }
    }
    savePass() {
        const pass: IPassWord = {
            id: this.userSelect.id,
            NewPass: this.formGroupPass.get('password').value,
            OldPass: null
        };
        this._userService.setPassWord(pass).subscribe(x => this.activeModal.close());
    }

    ngOnInit() {
    }

}
