import { Pipe, PipeTransform } from '@angular/core';
import { IUser } from '../../services/user.service';

@Pipe({
  name: 'filterUser'
})
export class FilterUserPipe implements PipeTransform {

  transform(value: IUser[], valor: string): any {
    if (!value||!valor)
      return value;
    return value.filter(item => {

      valor = valor.toLowerCase();
      let itemNew = Object.assign({}, item);

      if (itemNew.email !== null){
        itemNew.email = item.email.toLowerCase();
        if (itemNew.email.indexOf(valor) !== -1)
          return item
      }
      if (itemNew.phone !== null){
        itemNew.phone = item.phone.toLowerCase();
        if (itemNew.phone.indexOf(valor) !== -1)
          return item
        }
      if (itemNew.companyName !== null) {
        itemNew.companyName = item.companyName.toLowerCase();
        if (itemNew.companyName.indexOf(valor) !== -1)
          return item
      }
    });
  }

}
