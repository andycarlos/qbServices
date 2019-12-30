import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService, ISaleOrderConfig } from '../saleOrder.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-sale-order-config',
  templateUrl: './sale-order-config.component.html',
  styleUrls: ['./sale-order-config.component.css']
})
export class SaleOrderConfigComponent implements OnInit {
    formGroup: FormGroup;
    constructor(private _fb: FormBuilder,
        private _saleOrderService: SaleOrderService,
        private _location: Location) {

        this.formGroup = this._fb.group({
            invocesDueDate: new FormControl(0, Validators.required),
            daysNextDueDate: new FormControl(0, Validators.required),
            creditLimit: new FormControl(false, Validators.required),
        });
    }

    ngOnInit() {
        this.formGroup.disable();
        this._saleOrderService.getSaleOrderConfig().subscribe(x  => {
            this.formGroup.setValue({
                invocesDueDate : x.invocesDueDate,
                daysNextDueDate: x.daysNextDueDate,
                creditLimit: x.creditLimit
            });
        }, null, () => this.formGroup.enable());
    }
    save() {
        this._saleOrderService.saveSaleOrderConfig(this.formGroup.value as ISaleOrderConfig).subscribe(x => {
            this._saleOrderService.saleOrderConfig = this.formGroup.value as ISaleOrderConfig;
            this._location.back();
        });
    }

}
