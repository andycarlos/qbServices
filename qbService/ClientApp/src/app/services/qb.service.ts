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
    //Get all Vendors
    getAllVendors(): Observable<IVendors[]> {
        return this.http.get<IVendors[]>(this.baseUrl + "/getAllVendors");
    }
    //Get all Employee
    getAllEmployee(): Observable<IEmployee[]> {
        return this.http.get<IEmployee[]>(this.baseUrl + "/getAllEmployee");
    }
    //Get all OtherName
    getAllOtherName(): Observable<IOtherName[]> {
        return this.http.get<IOtherName[]>(this.baseUrl + "/getAllOtherName");
    }
    //Get all SaleRep
    getAllSalesRep(): Observable<ISalesRep[]> {
        return this.http.get<ISalesRep[]>(this.baseUrl + "/getAllSalesRep");
    }
    //Send Email
    sentEmail(email: UserEmail) {
        return this.http.post(this.baseUrl + "/Email", email);
    }


}
export interface Customer {
    listID: string;
    name: string;
    fullName: string;
    creditLimit: number;
    email: string;
    saleRepListID: string;
    shipToAddress: ShipToAddress[];
}
export interface ShipToAddress
{
    name: string;
    addr1: string;
    addr2: string;
    addr3: string;
    addr4: string;
    addr5: string;
    city: string;
    state: string;
    postalCode: string;
    country: string;
    note: string;
    defaultShipTo: string;
}
export interface IVendors {
    listID: string;
    name: string;
    email: string;
}
export interface IEmployee {
    listID: string;
    name: string;
    email: string;
}
export interface IOtherName {
    listID: string;
    name: string;
    email: string;
}

export interface ISalesRep {
    userListID: string;
    saleRepListID: string;
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
    items: InvoiceLineItem[];
}
export interface InvoceFilter {
    customerID: string;
    paidStatus: InvocePaidStatus;
    overdue: boolean;
    includeLineItems: boolean;
}
export enum InvocePaidStatus {
    All = 'All',
    PaidOnly = 'PaidOnly',
    NotPaidOnly = 'NotPaidOnly'
}
interface InvoiceLineItem {
    listID: string;
    quantity: Number;
    rate: Number;
    amount: Number;
}
export interface item {
    listID: string;
    name: string;
    fullName: string;
    salesDesc: string;
    salesPrice: number;
    type: string;
}

export interface UserEmail {
    userEmail: string;
    subject: string;
    body: string;
}



