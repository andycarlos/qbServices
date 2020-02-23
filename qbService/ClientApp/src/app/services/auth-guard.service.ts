import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { UserService } from './user.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

    constructor(private _userService: UserService,
        private router: Router) { }
    rolNull: string[] = ["/home", "/login", "/about"];
    rolQbAdmin: string[] = ["/qbuser", "/qbuser/add"];
    rolCreateSO: string[] = ["/saleorder"];
    rolConfigSO: string[] = ["/saleorder/config"];
    //rolFileDel: string[] = ["/files"];
    //rolFileDownload: string[] = ["/files"];
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | import("rxjs").Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        //return true;

        if (this._userService.isLogin()) {
            if (this._userService.rolAdmin) {
                return true;
            }

            if (this.rolNull.some(x => x == state.url)) {
                return true;
            }
            if (this._userService.rolQbAdmin && this.rolQbAdmin.some(x => x == state.url)) {
                return true;
            }
            if (this._userService.rolCreateSO && this.rolCreateSO.some(x => x == state.url)) {
              return true;
            }
            if (this._userService.rolConfigSO && this.rolConfigSO.some(x => x == state.url)) {
              return true;
            }
            //if (this._userService.rolFileDownload && this.rolFileDownload.some(x => x == state.url)) {
            //  return true;
            //}

            return false;
        }
        else {
            this.router.navigate(['/login']);
            return false;
        }
    }
}
