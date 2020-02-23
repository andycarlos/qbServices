
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { IUser,  IRole } from '../../services/user.service';

@Injectable({
    providedIn: 'root'
})
export class QbUserService {

    baseUrl;
    baseUrlUser;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Qb";
        this.baseUrlUser = (location as any)._platformLocation._doc.baseURI + "api/Account";
    }
    create(loginInfo: IUser): Observable<any> {
        return this.http.post<any>(this.baseUrlUser + "/Create", loginInfo);
    }

}




