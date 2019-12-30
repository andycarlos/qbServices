import { Component, OnInit } from '@angular/core';
import { Customer, QbService, Invoce, InvoceFilter, InvocePaidStatus, item } from '../../../services/qb.service';
import { FormGroup, FormBuilder, FormControl, FormArray, Validators } from '@angular/forms';
import { ISaleOrder, SaleOrderService } from '../saleOrder.service';
import { Router, Data } from '@angular/router';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { AlerComponent } from '../modalview/aler/aler.component';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-sale-order',
  templateUrl: './sale-order.component.html',
  styleUrls: ['./sale-order.component.css']
})
export class SaleOrderComponent implements OnInit {

    customers: Customer[] = [];
    items: item[] = [];
    formGroup: FormGroup;
    get CustomerRefListID(): string {
        return this.formGroup.get('CustomerRefListID').value;
    }
    

    constructor(private _qbServices: QbService,
        public _saleOrderService: SaleOrderService,
        private _fb: FormBuilder,
        private _router: Router,
        private _modalService: NgbModal,
        private config: NgbModalConfig) {

        config.backdrop = 'static';
        config.keyboard = false;

        this.formGroup = this._fb.group({
            CustomerRefListID: new FormControl('', Validators.required),
            SalesOrderLineAdd: this._fb.array([], Validators.required)
        });
    }

    ngOnInit() {
        this.loadInicilaData();
    }
    loadCustomers: boolean = false;
    loadInicilaData() {
        this._saleOrderService.getSaleOrderConfig().subscribe(x => {
            this._saleOrderService.saleOrderConfig = x;
        });
        this.formGroup.disable();
        this.loadCustomers = true;
        this._qbServices.getAllCustomers().subscribe(x => {
            this.customers = x;
            this._qbServices.getAllItems().subscribe(y => {
                this.items = y.filter(x => x.salesPrice !== 0 && x.type == "Inventory");
            });
        },
        error => {
            console.log(error);
            this.loadCustomers = false;
            this.formGroup.enable();
        },
        () => {
            this.loadCustomers = false;
            this.formGroup.enable();
        });
    }

    addSalesOrderLineAdd() {
        let array = this.formGroup.get('SalesOrderLineAdd') as FormArray;
        let grup = this._fb.group({
            ItemRefListID: new FormControl('', Validators.required),
            Quantity: new FormControl(0, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")])
        });
        array.push(grup);
    }
    removeSalesOrderLineAdd(i) {
        (this.formGroup.get('SalesOrderLineAdd') as FormArray).removeAt(i);
    }

    invoces: Invoce[] = [];
    amoutTotal: number;
    onChange(elemet: HTMLSelectElement) {
        //inicializacion de variables
        this.code = undefined;
        this.creditLimite = false;
        this.invocesDueDate = false;
        this.daysNextDueDate = false;

        this.formGroup.disable();
        let filter: InvoceFilter = {
            customerID: this.CustomerRefListID,
            paidStatus: InvocePaidStatus.NotPaidOnly,
            overdue: false
        }
        this._qbServices.getInvoices(filter).subscribe(x => {
            //console.log(x);
            if (x.id == this.formGroup.get('CustomerRefListID').value) {
                this.invoces = x.invoces;

                let dateNow: Date = new Date(x.dateNow);
                this.amoutTotal = 0;
                this.invoces.forEach(x => {
                    this.amoutTotal += x.balanceRemaining;
                });
                this.invoces = this.invoces.filter(x => (x.dueDate.getTime() < dateNow.getTime()));

                this.formGroup.enable();
                this.checkEnableSaleOrder(dateNow);
                elemet.focus();
            }
        },
        error => {
            console.log(error);
            this.formGroup.enable();
            //this.loadInvoce = false;
        });
    }
    //var to modale CheckSaleOrder
    enableSaleOrder: boolean;
    creditLimite: boolean = false;
    invocesDueDate: boolean = false;
    daysNextDueDate: boolean = false;
    checkEnableSaleOrder(dateNow: Date) {
        
        this.enableSaleOrder = true;
        if (this._saleOrderService.saleOrderConfig.invocesDueDate > 0) {
            if (this.invoces.length >= this._saleOrderService.saleOrderConfig.invocesDueDate) {
                this.enableSaleOrder = false;
                this.invocesDueDate = true;
                return;
            }
        }
        if (this._saleOrderService.saleOrderConfig.daysNextDueDate > 0) {
            let invoceTemp: Invoce[] = [];
            this.invoces.forEach(invoc => {
                let tempDueDate: Date = new Date(invoc.dueDate);
                tempDueDate.setDate(tempDueDate.getDate() + this._saleOrderService.saleOrderConfig.daysNextDueDate);

                if (tempDueDate.getTime() <= dateNow.getTime()) {
                    this.enableSaleOrder = false;
                    this.daysNextDueDate = true;
                    invoceTemp.push(invoc);
                }
            });
            this.invoces = invoceTemp;
        }
    }

    private amountActual(): number {
        let amountTotal = 0;
        let controls = (this.formGroup.get('SalesOrderLineAdd') as FormArray).controls;
        if (!this.items || !controls)
            return amountTotal;
        controls.forEach(y => {
            if (y.get('ItemRefListID').value != "") {
                let price: number = this.items.find(x => x.listID == y.get('ItemRefListID').value).salesPrice;
                let quantityt = y.get('Quantity').value;
                if (!isNaN(quantityt))
                    amountTotal += price * quantityt;
            }
        });
        return amountTotal;
    }

    save() {

        if (this.creditLimite!=true && this._saleOrderService.saleOrderConfig.creditLimit) {
            let amout: number = this.amountActual() + this.amoutTotal;
            let creditLimite = this.customers.find(x => x.listID == this.CustomerRefListID).creditLimit;
            if (creditLimite>0 && creditLimite < amout) {
                this.enableSaleOrder = false;
                this.creditLimite = true;
                return;
            }
        }

        let saleOrder = this.formGroup.value as ISaleOrder;
        const modalRef = this._modalService.open(AlerComponent);
        modalRef.componentInstance.canClose = false;
        modalRef.componentInstance.title = 'Wait, please';
        modalRef.componentInstance.body = 'Processing the data.';

        this._saleOrderService.createSaleOrder(saleOrder).subscribe(x => {
            modalRef.componentInstance.title = 'Good job';
            modalRef.componentInstance.body = 'The Sale Order was created successfully.';
            modalRef.result.then(() => this._router.navigate(['/home']));
            modalRef.componentInstance.canClose = true;
            
        }, error => {
                console.log(error);
                modalRef.componentInstance.title = 'Bad job';
                modalRef.componentInstance.body = 'The Sale Order error. ' + ((error.error == undefined) ? "" : error.error.error);
                modalRef.componentInstance.canClose = true;
        });
    }

    goBack() {
        this._router.navigate(['/home']);
    }

    code: string;
    getCode() {
        let nameCustomer: string = this.customers.find(x => x.listID == this.CustomerRefListID).fullName;
        this._saleOrderService.getCode(nameCustomer).subscribe(x => this.code = x.code);
        this.code = "0000000";
    }
    codeValid: boolean = false;
    cheqCode(value: string) {
        if (value == this.code) {
            this.enableSaleOrder = true;
            this.codeValid = false;
            this.code = undefined;
            this.invocesDueDate = false;
            this.daysNextDueDate = false;
            if (this.creditLimite == true)
                this.save();
        } else {
            this.codeValid = true;
        }
    }
}


