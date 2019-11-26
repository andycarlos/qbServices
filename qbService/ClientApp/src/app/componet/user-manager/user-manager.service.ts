import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { IUser, IPassWord, IRole } from '../../services/user.service';

@Injectable({
    providedIn: 'root'
})
export class UserManagerService {

    baseUrl;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Account";
    }
    //Get all Users
    getAllUser(): Observable<IUser[]> {
        return this.http.get<IUser[]>(this.baseUrl + "/GetAllUser");
    }
    getUserByID(ID: string): Observable<IUser> {
        let head: HttpHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post<IUser>(this.baseUrl + "/GetUserByID", '"' + ID + '"', { headers: head });
    }
    AnyUserByEmail(email: string): Observable<boolean> {
        let param = new HttpParams().set("Email", email);
        return this.http.get<boolean>(this.baseUrl + "/AnyUserByEmail", { params: param });
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

    GetListRoles(): Observable<IRole[]> {
        return this.http.get<IRole[]>(this.baseUrl + "/GetAllRoles");
    }
    AddRolByUser(user: IUser, rol: IRole) {
        return this.http.put(this.baseUrl + "/AddRolByUser/" + user.id, rol);
    }
    RemoveRolByUser(user: IUser, rol: IRole) {
        return this.http.put(this.baseUrl + "/RemoveRolByUser/" + user.id, rol);
    }
}

