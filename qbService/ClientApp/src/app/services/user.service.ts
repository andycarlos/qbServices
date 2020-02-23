import { Injectable } from '@angular/core';
import { HttpClient,  HttpParams } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    baseUrl;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Account";
    }

    token: string = '';
    tokenUserExpiration: string = '';
    loginNow: boolean = false;
    roles: string[] = [];
    type: string = '';
    userListID: string = '';
    SaleRepListID: string = null;
    userEmail: string = ''; //for navbar


    //
    rolAdmin: boolean = false;
    rolQbAdmin: boolean = false;
    rolCreateSO: boolean = false;
    rolConfigSO: boolean = false;
    //rolFileDownload: boolean = false;
    access(rolesTemp: string[]) {
        this.roles = rolesTemp;
        if (this.roles.some(x => x == "Admin")) {
            this.rolAdmin = true;
        } else { this.rolAdmin = false; }

        if (this.roles.some(x => x == "QbAdmin")) {
            this.rolQbAdmin = true;
        } else { this.rolQbAdmin = false; }

        if (this.roles.some(x => x == "CreateSO")) {
            this.rolCreateSO = true;
        } else { this.rolCreateSO = false; }

        if (this.roles.some(x => x == "ConfigSO")) {
            this.rolConfigSO = true;
        } else { this.rolConfigSO = false; }

        //if (this.roles.some(x => x == "File_DownLoad")) {
        //    this.rolFileDownload = true;
        //} else { this.rolFileDownload = false; }
    }

    //Get all Users
    getAllUser(): Observable<IUser[]> {
        return this.http.get<IUser[]>(this.baseUrl + "/GetAllUser");
    }

    forgotPassword(Email: IEmail): Observable<any> {
        return this.http.post(this.baseUrl + "/ForgotPassword", Email);
    }
    forgotPasswordValid(ForgotPassword: IForgotPassword): Observable<any> {
        return this.http.post(this.baseUrl + "/ForgotPasswordValid", ForgotPassword);
    }

    login(loginInfo: IUser): Observable<any> {
        return this.http.post<any>(this.baseUrl + "/Login", loginInfo);
    }
    logout() {
        this.token = '';
        this.tokenUserExpiration = '';
        this.loginNow = false;
    }
    isLogin(): boolean {

        let exp = this.tokenUserExpiration;
        if (!exp) {
            this.logout();
            return false;
        }
        let now = new Date().getTime();
        let dateExp = new Date(exp);

        if (now > dateExp.getTime()) {
            //ya expiro
            this.logout();
            return false;
        }
        else {
            this.loginNow = true;
            return true;
        }
    }

    obtenerToken(): string {
        return this.token;
    }
    obtenerTokenExpriacion(): string {
        return this.tokenUserExpiration;
    }

    //roles
    GetListRoles(): Observable<IRole[]> {
        return this.http.get<IRole[]>(this.baseUrl + "/GetAllRoles");
    }
    GetAllRolesUser(): Observable<IRole[]> {
        return this.http.get<IRole[]>(this.baseUrl + "/GetAllRolesUser");
    }
    AddRolByUser(user: IUser, rol: IRole) {
        return this.http.post(this.baseUrl + "/AddRolByUser/" + user.id, rol);
    }
    RemoveRolByUser(user: IUser, rol: IRole) {
        return this.http.post(this.baseUrl + "/RemoveRolByUser/" + user.id, rol);
    }
}


export interface IEmail {
  email: string;
}
export interface IUser {
    id: string;
    companyName: string;
    email: string;
    phone: string;
    password: string;
    roles: string[];
    block: boolean;
    typeUser: string;
    name: string;
    listID: string;
}
export interface IForgotPassword {
  email: string;
  token: string;
  passwrod: string;
}
export interface IRole {
  id: string;
  name: string;
  normalizedName: string;
  concurrencyStamp: string;
  activ: boolean;
}
