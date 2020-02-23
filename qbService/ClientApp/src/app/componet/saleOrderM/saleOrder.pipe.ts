import { Pipe, PipeTransform, Type } from '@angular/core';
import { item, Invoce } from '../../services/qb.service';
import { AbstractControl } from '@angular/forms';

@Pipe({
    name: 'DescByItem'
})
export class DescByItemPipe implements PipeTransform {

    transform(items: item[], ItemFullName: string): any {
        if (!ItemFullName || !items)
            return "";
        
        let tempItem: item = items.find(x => x.fullName == ItemFullName);
        return (tempItem != undefined) ? tempItem.salesDesc:'';
    }
}

@Pipe({
    name: 'AmountByItem'
})
export class AmountByItemPipe implements PipeTransform {

    transform(items: item[], ItemFullName: string, quantityt: number): any {
        if (!ItemFullName || !items || !quantityt)
            return "0";
        let tempItem: item = items.find(x => x.fullName == ItemFullName);
        let price: number = (tempItem != undefined) ? tempItem.salesPrice:0;

        if (isNaN(quantityt))
            return '0';
        else
            return (price * quantityt).toFixed(2);
    }
}

@Pipe({
    name: 'AmountTotal',
    pure: false
})
export class AmountTotalPipe implements PipeTransform {
    
    transform(items: item[], controls: AbstractControl[]): any {
        
        let amountTotal = 0;
        if (!items || !controls)
            return amountTotal.toFixed(2);
        controls.forEach(y => {
            //console.log(y.get('ItemRefListID').value)
            let Amount = y.get('Amount').value;
            if (!isNaN(Amount))
                amountTotal += Amount;
        });
        return amountTotal.toFixed(2);
    }
}

//for the modal
@Pipe({
    name: 'AmountTotalByInvoce',
    pure: false
})
export class AmountTotalByInvocePipe implements PipeTransform {

    transform(items: Invoce[]): any {
        let amountTotal = 0;
        if (!items)
            return amountTotal.toFixed(2);
        items.forEach(y => amountTotal += y.balanceRemaining);
        return amountTotal.toFixed(2);
    }
}
