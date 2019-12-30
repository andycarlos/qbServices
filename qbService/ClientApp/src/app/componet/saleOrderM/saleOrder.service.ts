import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class SaleOrderService {

    baseUrl;
    baseUrlSaleOrderConfigModels;
    saleOrderConfig: ISaleOrderConfig;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Qb";
        this.baseUrlSaleOrderConfigModels = (location as any)._platformLocation._doc.baseURI + "api/SaleOrderConfigModels";
    }
    createSaleOrder(body: ISaleOrder) {
        return this.http.post(this.baseUrl + "/CreateSaleOrder", body);
    }
    getCode(nameCustomer: string): Observable<ICode> {
        let paras: HttpParams = new HttpParams().set('name', nameCustomer)
        return this.http.get<ICode>(this.baseUrl + "/getCode", { params: paras });
    }

    saveSaleOrderConfig(body: ISaleOrderConfig) {
        return this.http.post(this.baseUrlSaleOrderConfigModels, body);
    }
    getSaleOrderConfig(): Observable<ISaleOrderConfig> {
        return this.http.get<ISaleOrderConfig>(this.baseUrlSaleOrderConfigModels);
    }
}
export interface ISaleOrder {
    CustomerRefListID: string;
    SalesOrderLineAdd: ISalesOrderLineAdd[];
}
export interface ISalesOrderLineAdd {
    ItemRefListID: string;
    Quantity: number;
}
export interface ICode {
    code: string;
}
export interface ISaleOrderConfig {
    invocesDueDate: number;
    daysNextDueDate: number;
    creditLimit: boolean;
}

