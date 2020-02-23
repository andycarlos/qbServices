import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { IUser,  IRole } from '../../services/user.service';

@Injectable({
    providedIn: 'root'
})
export class UserManagerService {

    baseUrl;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Account";
    }
    
    getUserByID(ID: string): Observable<IUser> {
        let head: HttpHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post<IUser>(this.baseUrl + "/GetUserByID", '"' + ID + '"', { headers: head });
    }
    AnyUserByEmail(email: string): Observable<boolean> {
        let param = new HttpParams().set("Email", email);
        return this.http.get<boolean>(this.baseUrl + "/AnyUserByEmail", { params: param });
    }
    AnyCompanyName(companyName: string): Observable<boolean> {
        let param = new HttpParams().set("CompanyName", companyName);
        return this.http.get<boolean>(this.baseUrl + "/AnyCompanyName", { params: param });
    }
    setPassWord(passInfo: IPassWord): Observable<any> {
        return this.http.post(this.baseUrl + "/SetPassword", passInfo);
    }
    blockUser(user: IUser): Observable<any> {
        return this.http.post<any>(this.baseUrl + "/Block", user);
    }

    create(loginInfo: IUser): Observable<any> {
        return this.http.post<any>(this.baseUrl + "/Create", loginInfo);
    }
    updateUser(loginInfo: IUser): Observable<any> {
        return this.http.post<any>(this.baseUrl + "/UpdateUser", loginInfo);
    }
    delect(user: IUser): Observable<IUser> {
        return this.http.post<IUser>(this.baseUrl + "/Remove", user);
    }


}
export interface IPassWord {
    id: string;
    OldPass: string;
    NewPass: string;
}

