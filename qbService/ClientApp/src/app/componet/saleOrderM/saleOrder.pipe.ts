import { Pipe, PipeTransform } from '@angular/core';
import { item, Invoce } from '../../services/qb.service';
import { AbstractControl } from '@angular/forms';

@Pipe({
    name: 'DescByItem'
})
export class DescByItemPipe implements PipeTransform {

    transform(items: item[], ItemRefListID: string): any {
        if (!ItemRefListID || !items)
            return "";
        return items.find(x => x.listID == ItemRefListID).salesDesc;
    }
}

@Pipe({
    name: 'AmountByItem'
})
export class AmountByItemPipe implements PipeTransform {

    transform(items: item[], ItemRefListID: string, quantityt: number): any {
        if (!ItemRefListID || !items || !quantityt)
            return "0";
        let price: number = items.find(x => x.listID == ItemRefListID).salesPrice;

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
            if (y.get('ItemRefListID').value != "") {
                let price: number = items.find(x => x.listID == y.get('ItemRefListID').value).salesPrice;
                let quantityt = y.get('Quantity').value;
                if (!isNaN(quantityt))
                    amountTotal += price * quantityt;
            }
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
