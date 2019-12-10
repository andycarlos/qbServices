import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { Data } from '@angular/router';
@Component({
  selector: 'app-prueba',
  templateUrl: './prueba.component.html',
  styleUrls: ['./prueba.component.css']
})
export class PruebaComponent implements OnInit {

    baseUrl;
    constructor(private http: HttpClient,
        location: Location) {
        this.baseUrl = (location as any)._platformLocation._doc.baseURI + "api/Hub";
    }
    customers: Customer[] = [];
    invoces: Invoce[] = [];
    customer: string;

    loadCustomer: boolean = false;
    ngOnInit() {
        let params: HttpParams = new HttpParams().set("funcion", "getAllCustomer");
        this.loadCustomer = true;
        this.http.get<Customer[]>(this.baseUrl + "/GetLisCustomer", { params: params }).subscribe(x => {
            this.loadCustomer = false;
            if (x) {
                console.log(x);
                this.customers = x;
            }
            else
                console.log("User Destok no Conect");
        });
        
    }

    loadInvoces: boolean = false;
    run = 0;
    amoutTotal: number;
    balanceTotal: number;
    onChange() {
        if (this.customer) {
            this.invoces = [];
            let header: HttpHeaders = new HttpHeaders().set("id", this.customer);
            let params1: HttpParams = new HttpParams().set("funcion", "getAllInvoceByUserID");

            this.amoutTotal = 0;
            this.balanceTotal = 0;

            this.loadInvoces = true;
            this.http.get<any>(this.baseUrl + "/GetLisCustomer", { params: params1, headers: header }).subscribe(x => {
                this.loadInvoces = false;
                if (x) {
                    if (x.invoces) {
                        if (x.id == this.customer) {
                            this.invoces = x.invoces;
                            
                            this.invoces.forEach(value => {
                                if (value.isPaid == false) {
                                  this.amoutTotal += value.subtotal;
                                  this.balanceTotal += value.balanceRemaining;
                                }
                            });
                        }
                        else
                            this.onChange();
                        }
                        if (x.error)
                            console.log(x.error)
                    }
                    else {
                        console.log("User Destok no Conect");
                    }
                });
            //}
            //else {
            //    this.run = 2;
            //}
        }
        
    }

}
interface Customer {
    listID: string;
    name: string;
    fullName: string;
}
interface Invoce {
    txnID: string;
    txnDate: Data;
    dueDate: Data;
    balanceRemaining: number;
    isPaid: boolean;
    refNumber: number;
    subtotal: number;
}
