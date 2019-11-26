import { Component, OnInit, Input, AfterViewChecked} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IUser, IRole } from '../../../services/user.service';
import { UserManagerService } from '../user-manager.service';

@Component({
  selector: 'app-rol-user',
  templateUrl: './rol-user.component.html',
  styleUrls: ['./rol-user.component.css']
})
export class RolUserComponent implements OnInit, AfterViewChecked {


    @Input() name;
    @Input() userSelect: IUser;
    RolesList: IRole[] = [];
    sectionExpire = '';
    constructor(public activeModal: NgbActiveModal,
        private _userService: UserManagerService) { }

    ngOnInit() {
        this.RolesList = [];
        
        this._userService.GetListRoles().subscribe(x => {
            this.sectionExpire = '-';
            this.RolesList = x;
            this.RolesList.forEach(x => {
                if (this.userSelect.roles.some(usRol => usRol == x.name)) {
                    x.activ = true;
                }
                else {
                    x.activ = false;
                }
            });
        }, null, () => { if (this.sectionExpire === '') this.sectionExpire='close'; }); 
    }

    ngAfterViewChecked(): void {
        if (this.sectionExpire==='close')
          this.activeModal.close();
    }

    editRolUserUpDate(rol: IRole) {
        if (rol.activ) {
            this._userService.RemoveRolByUser(this.userSelect, rol).subscribe();
            this.userSelect.roles = this.userSelect.roles.filter(x => x != rol.name);
        }
        else {
            this._userService.AddRolByUser(this.userSelect, rol).subscribe();
            this.userSelect.roles.push(rol.name);
        }
        this.RolesList.forEach(x => {
            if (this.userSelect.roles.some(usRol => usRol == x.name)) {
                x.activ = true;
            }
            else {
                x.activ = false;
            }
        });
    }
}
