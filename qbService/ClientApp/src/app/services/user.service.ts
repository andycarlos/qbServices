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

  token: string='';
  tokenUserExpiration: string='';
  loginNow: boolean = false;
  roles: string[] = [];
  userEmail: string = ''; //for navbar

  //
  rolAdmin: boolean = false;
  rolUser: boolean = false;
  rolFileAdd: boolean = false;
  rolFileDel: boolean = false;
  rolFileDownload: boolean = false;
  access(rolesTemp: string[]) {
    this.roles = rolesTemp;
    if (this.roles.some(x => x == "Admin")) {
      this.rolAdmin = true;
    } else { this.rolAdmin = false; }

    if (this.roles.some(x => x == "User")) {
      this.rolUser = true;
    } else { this.rolUser = false; }

    if (this.roles.some(x => x == "File_Add")) {
      this.rolFileAdd = true;
    } else { this.rolFileAdd = false; }

    if (this.roles.some(x => x == "File_Del")) {
      this.rolFileDel = true;
    } else { this.rolFileDel = false; }

    if (this.roles.some(x => x == "File_DownLoad")) {
      this.rolFileDownload = true;
    } else { this.rolFileDownload = false; }
  }

  //Get all Users
  forgotPassword(Email: IEmail): Observable<any> {
    return this.http.post(this.baseUrl + "/ForgotPassword", Email);
  }
  forgotPasswordValid(ForgotPassword: IForgotPassword): Observable<any> {
    return this.http.post(this.baseUrl + "/ForgotPasswordValid", ForgotPassword);
  }

  login(loginInfo: IUser): Observable<any>
  {
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

  obtenerToken(): string{
    return this.token;
  }
  obtenerTokenExpriacion(): string {
    return this.tokenUserExpiration;
  }
}


export interface IEmail {
  email: string;
}
export interface IUser{
  id: string;
  companyName: string;
  email: string;
  phone: string;
  password: string;
  roles: string[];
  block: boolean;
}
export interface IPassWord {
  id: string;
  OldPass: string;
  NewPass: string;
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
