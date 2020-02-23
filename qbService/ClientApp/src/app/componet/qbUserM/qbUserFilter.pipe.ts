import { Pipe, PipeTransform } from '@angular/core';
import { IUser } from '../../services/user.service';

@Pipe({
  name: 'qbfilterUser'
})
export class QbFilterUserPipe implements PipeTransform {

  transform(value: IUser[], valor: string): any {
      if (!value||!valor)
        return value;
      return value.filter(item => {

          valor = valor.toLowerCase();
          let itemNew = Object.assign({}, item);

          if (itemNew.email !== null) {
              itemNew.email = item.email.toLowerCase();
              if (itemNew.email.indexOf(valor) !== -1)
                  return item
          }
          if (itemNew.name !== null) {
              itemNew.name = item.name.toLowerCase();
              if (itemNew.name.indexOf(valor) !== -1)
                  return item
          }
          if (itemNew.typeUser !== null) {
              itemNew.typeUser = item.typeUser.toLowerCase();
              if (itemNew.typeUser.indexOf(valor) !== -1)
                  return item
          }
      });
  }

}
