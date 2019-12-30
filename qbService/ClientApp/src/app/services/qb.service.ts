import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class QbService {

    baseUrl;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Qb";
    }
    getAllCustomers(): Observable<Customer[]> {
        return this.http.get<Customer[]>(this.baseUrl + "/getAllCustomers");
    }
    getInvoices(body: InvoceFilter): Observable<invoceResponse> {
        return this.http.post<invoceResponse>(this.baseUrl + "/getInvoces", body).pipe(
             map(x => {
                 x.invoces.forEach(d => {
                     d.dueDate = new Date(d.dueDate);
                     d.txnDate = new Date(d.txnDate);
               });
               return x;
            })
        );
    }
    getAllItems(): Observable<item[]> {
        return this.http.get<item[]>(this.baseUrl + "/getItems");
    }
}
export interface Customer {
    listID: string;
    name: string;
    fullName: string;
    creditLimit: number;
}
export interface invoceResponse {
    id: string;
    dateNow: Date;
    invoces: Invoce[];
}
export interface Invoce {
    txnID: string;
    txnDate: Date;
    dueDate: Date;
    balanceRemaining: number;
    refNumber: number;
    subtotal: number;
}
export interface InvoceFilter {
    customerID: string;
    paidStatus: InvocePaidStatus;
    overdue: boolean;
}
export enum InvocePaidStatus {
    All = 'All',
    PaidOnly = 'PaidOnly',
    NotPaidOnly = 'NotPaidOnly'
}
export interface item {
    listID: string;
    name: string;
    fullName: string;
    salesDesc: string;
    salesPrice: number;
    type: string;
}
