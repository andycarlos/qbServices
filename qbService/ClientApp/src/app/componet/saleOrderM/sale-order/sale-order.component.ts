import { Component, OnInit, ViewChild } from '@angular/core';
import { Customer, QbService, Invoce, InvoceFilter, InvocePaidStatus, item, UserEmail, ShipToAddress } from '../../../services/qb.service';
import { FormGroup, FormBuilder, FormControl, FormArray, Validators, ValidatorFn, AbstractControl, EmailValidator } from '@angular/forms';
import { ISaleOrder, SaleOrderService } from '../saleOrder.service';
import { Router, Data } from '@angular/router';
import { NgbModal, NgbModalConfig, NgbTypeahead } from '@ng-bootstrap/ng-bootstrap';
import { Observable, Subject, merge, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, catchError } from 'rxjs/operators';

import { UserService } from '../../../services/user.service';
import { AlerComponent } from '../../../modalView/aler/aler.component';

@Component({
  selector: 'app-sale-order',
  templateUrl: './sale-order.component.html',
  styleUrls: ['./sale-order.component.css']
})
export class SaleOrderComponent implements OnInit {

    customers: Customer[] = [];
    customerSelect: Customer;
    items: item[] = [];
    formGroup: FormGroup;

    //customer fill typea
    focusCustomer$ = new Subject<string>();
    formatterCustomer = (state: Customer) => state.fullName;
    formatterItems = (iten: item) => iten.fullName;
    
    get SalesOrderLineAdd():FormArray {
       return (this.formGroup.get('SalesOrderLineAdd')as FormArray) ;
    }
    
    get Shiping():ShipToAddress {
        return this.formGroup.get('Shiping').value ;
     }
  

    search = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe( debounceTime(200), distinctUntilChanged());
        const inputFocus$ = this.focusCustomer$;

        return merge(debouncedText$, inputFocus$).pipe(
            map(term => (term === '' ? this.customers
                : this.customers.filter(v => v.fullName.toLowerCase().indexOf(term.toLowerCase()) > -1)))
        );
    }
    searchItems = (text$: Observable<string>) => {
        return text$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
          map(term => this.items.filter(v => {
            if (v.fullName != null && v.fullName != undefined && v.salesDesc != null && v.salesDesc != undefined)
                    return v.fullName.toLowerCase().indexOf(term.toLowerCase()) > -1 || v.salesDesc.toLowerCase().indexOf(term.toLowerCase()) > -1
                return false;
          }))
        );
    }
    focusInput(e: Event) {
        e.stopPropagation();
        setTimeout(() => {
            const inputEvent: Event = new Event('input');
            e.target.dispatchEvent(inputEvent);
        }, 0);
    }
    typeaheadKeydown(event: NgbTypeahead) {
        if (event.isPopupOpen()) {
            setTimeout(() => {
                const popup = document.getElementById(event.popupId);
                const activeElements = popup.getElementsByClassName('active');
                if (activeElements.length === 1) {
                    const elem = (activeElements[0] as any);
                    if (typeof elem.scrollIntoViewIfNeeded === 'function') {
                        // non standard function, but works (in chrome)...
                        this.scrollIntoViewIfNeededPolyfill(elem as HTMLElement);
                        //elem.scrollIntoViewIfNeeded();
                    } else {
                        //do custom scroll calculation or use jQuery Plugin or ...
                        this.scrollIntoViewIfNeededPolyfill(elem as HTMLElement);
                    }
                }
            });
        }
    }
    private scrollIntoViewIfNeededPolyfill(elem: HTMLElement, centerIfNeeded = true) {
        var parent = elem.parentElement,
            parentComputedStyle = window.getComputedStyle(parent, null),
            parentBorderTopWidth = parseInt(parentComputedStyle.getPropertyValue('border-top-width')),
            parentBorderLeftWidth = parseInt(parentComputedStyle.getPropertyValue('border-left-width')),
            overTop = elem.offsetTop - parent.offsetTop < parent.scrollTop,
            overBottom = (elem.offsetTop - parent.offsetTop + elem.clientHeight - parentBorderTopWidth) > (parent.scrollTop + parent.clientHeight),
            overLeft = elem.offsetLeft - parent.offsetLeft < parent.scrollLeft,
            overRight = (elem.offsetLeft - parent.offsetLeft + elem.clientWidth - parentBorderLeftWidth) > (parent.scrollLeft + parent.clientWidth),
            alignWithTop = overTop && !overBottom;

        if ((overTop || overBottom) && centerIfNeeded) {
            parent.scrollTop = elem.offsetTop - parent.offsetTop - parent.clientHeight / 2 - parentBorderTopWidth + elem.clientHeight / 2;
        }

        if ((overLeft || overRight) && centerIfNeeded) {
            parent.scrollLeft = elem.offsetLeft - parent.offsetLeft - parent.clientWidth / 2 - parentBorderLeftWidth + elem.clientWidth / 2;
        }

        if ((overTop || overBottom || overLeft || overRight) && !centerIfNeeded) {
            elem.scrollIntoView(alignWithTop);
        }
    }

    constructor(private _qbServices: QbService,
        public _saleOrderService: SaleOrderService,
        public _userService: UserService,
        private _fb: FormBuilder,
        private _router: Router,
        private _modalService: NgbModal,
        private config: NgbModalConfig) {

        config.backdrop = 'static';
        config.keyboard = false;

        this.formGroup = this._fb.group({
            CustomerRefListID: new FormControl('', Validators.required),
            SalesOrderLineAdd: this._fb.array([], Validators.required),
            Shiping: new FormControl()
        });
        //this.formGroup.valueChanges.subscribe(x => console.log(x));
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
            //user type distinct to customers
            if (this._userService.type != "Customers" && this._userService.SaleRepListID != null)
                this.customers = x.filter(y => y.saleRepListID == this._userService.SaleRepListID);
            //user type customers
            if (this._userService.type == "Customers") {
                this.customers = x.filter(y => y.listID == this._userService.userListID);
                this.formGroup.get('CustomerRefListID').setValue(this.customers[0].name);
            }

            ////select first element
            //if (this.customers.length != 0) {
            //    this.formGroup.get('CustomerRefListID').setValue(this.customers[0].listID);
            //    this.onChange(null);
            //}

            //fill items list
            this._qbServices.getAllItems().subscribe(y => {
                this.items = y.filter(x => ( x.type == "Inventory"));//|| (x.type == "Service")x.salesPrice !== 0 &&
            }, e => this.loadCustomers = false, () => this.loadCustomers = false);

        },
        error => {
            console.log(error);
            this.loadCustomers = false;
            this.formGroup.enable();
            if (this._userService.type == "Customers")
                this.formGroup.get('CustomerRefListID').disable();
        },
        () => {
            this.formGroup.enable();
            if (this._userService.type == "Customers")
                this.formGroup.get('CustomerRefListID').disable();
            
        });
    }

    removeSalesOrderLineAdd(i) {
        
        (this.formGroup.get('SalesOrderLineAdd') as FormArray).removeAt(i);
    }
    addSalesOrderLineAdd() {
        
        let array = this.formGroup.get('SalesOrderLineAdd') as FormArray;
        let grup = this._fb.group({
            ItemRefListID: new FormControl('', [Validators.required, this.validItemValid()]),
            Quantity: new FormControl(0, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")]),
            Rate: new FormControl(0, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")]),
            Amount: new FormControl(0, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")])
        });
        let leng = this.itemFullName.length;
        grup.get('ItemRefListID').valueChanges.subscribe(() => this.updateAmount(grup, leng));
        grup.get('Quantity').valueChanges.subscribe(() => this.updateAmount(grup, -1));
        grup.get('Rate').valueChanges.subscribe(() => this.updateAmount(grup, -1));
        array.push(grup);
        this.itemFullName.push('');
    }
    itemFullName: string[] = [];
    updateAmount(grup: FormGroup, leng: number) {
       
        let ItemFullName;
        if (typeof grup.get('ItemRefListID').value == "string")
            ItemFullName = grup.get('ItemRefListID').value;
        else
            ItemFullName = ((grup.get('ItemRefListID').value as item))?(grup.get('ItemRefListID').value as item).fullName:'';

        let quantityt = grup.get('Quantity').value;

        if (leng==-1 || ItemFullName != this.itemFullName[leng]) {
            if (!ItemFullName || !this.items || !quantityt)
                grup.get('Amount').setValue(0);

            let tempItem: item = this.items.find(x => x.fullName == ItemFullName);

            if (ItemFullName != this.itemFullName[leng]) {
                if (leng != -1) {
                    this.itemFullName[leng] = ItemFullName;
                    grup.get('Rate').setValue((tempItem != undefined) ? tempItem.salesPrice : 0);
                    return;
                }
            }

            if (isNaN(quantityt))
                grup.get('Amount').setValue(0)
            else
                grup.get('Amount').setValue(+(grup.get('Rate').value * quantityt).toFixed(2));
        }
    }
    updateRate(grup: FormGroup, event: Event) {
        if (grup.valid == false) {
            event.stopPropagation();
            event.preventDefault();
        }
        if ( event.type == "input") {
            let amout = grup.get('Amount').value;
            let quantityt = grup.get('Quantity').value;
            if (amout > 0 && quantityt>0)
                grup.get('Rate').setValue(amout / quantityt);
            else
                grup.get('Rate').setValue(0);
        }
    }
    validItemValid(): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            let valor;
            if (typeof control.value == "string")
                valor = control.value;
            else
                valor = (control.value as item).fullName;
            const forbidden = this.items.some(z => z.fullName == valor);
            return !forbidden ? { 'ItemInvalid': { value: control.value } } : null;
        };
    }

    invoces: Invoce[] = [];
    amoutTotal: number;
    onChangeCustomerRefListID(value: string) {
        //inicializacion de variables
        //console.log(value);
        if (this.customers.some(x => x.name == value)) {

            this.customerSelect = this.customers.find(x => x.name == value);
            if(this.customerSelect.shipToAddress!=null && this.customerSelect.shipToAddress.length>0)
                this.formGroup.get('Shiping').setValue(this.customerSelect.shipToAddress[0]);
            else
                this.formGroup.get('Shiping').setValue(null);

           // console.log(this.Shiping);

            (this.formGroup.get('SalesOrderLineAdd') as FormArray).clear();
            this.code = undefined;
            this.creditLimite = false;
            this.invocesDueDate = false;
            this.daysNextDueDate = false;

            this.formGroup.disable();
            let filter: InvoceFilter = {
                customerID: this.customerSelect.listID,
                paidStatus: InvocePaidStatus.NotPaidOnly,
                overdue: false,
                includeLineItems: false
            }
            this._qbServices.getInvoices(filter).subscribe(x => {
                //console.log(x);
                if (x.id == this.customerSelect.listID) {
                    this.invoces = x.invoces;

                    let dateNow: Date = new Date(x.dateNow);
                    this.amoutTotal = 0;
                    this.invoces.forEach(x => {
                        this.amoutTotal += x.balanceRemaining;
                    });
                    this.invoces = this.invoces.filter(x => (x.dueDate.getTime() < dateNow.getTime()));

                    this.checkEnableSaleOrder(dateNow);
                }
                this.formGroup.enable();
                if (this._userService.type == "Customers")
                    this.formGroup.get('CustomerRefListID').disable();
            },
                error => {
                    console.log(error);
                    this.formGroup.enable();
                    if (this._userService.type == "Customers")
                        this.formGroup.get('CustomerRefListID').disable();
                    //this.loadInvoce = false;
                });
        }
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
            let Amount = y.get('Amount').value;
            if (!isNaN(Amount))
                amountTotal += Amount;
        });
        return amountTotal;
    }
    save() {
        
        if (this.formGroup.status == 'VALID') {
            //valida CREDIT LIMIT
            if (this.creditLimite != true && this._saleOrderService.saleOrderConfig.creditLimit) {
                let amout: number = this.amountActual() + this.amoutTotal;
                let creditLimite = this.customerSelect.creditLimit;// this.customers.find(x => x.listID == this.customerSelect.listID).creditLimit;
                if (creditLimite > 0 && creditLimite < amout) {
                    this.enableSaleOrder = false;
                    this.creditLimite = true;
                    return;
                }
            }

            //Crea sale ordern
            let saleOrder: ISaleOrder = {
                CustomerRefListID: this.customerSelect.listID,
                SalesOrderLineAdd: []
            };

            (this.formGroup.controls['SalesOrderLineAdd'] as FormArray).controls.forEach(x => {
                let valor;
                if (typeof x.get('ItemRefListID').value == "string")
                    valor = x.get('ItemRefListID').value;
                else
                    valor = (x.get('ItemRefListID').value as item).fullName;
                saleOrder.SalesOrderLineAdd.push({
                    ItemRefListID: this.items.find(z => z.fullName == valor).listID,
                    Quantity: x.get('Quantity').value,
                    Amount: x.get('Amount').value
                });
            });

            const modalRef = this._modalService.open(AlerComponent);
            modalRef.componentInstance.canClose = false;
            modalRef.componentInstance.title = 'Wait, please';
            modalRef.componentInstance.body = 'Processing the data.';

            this._saleOrderService.createSaleOrder(saleOrder).subscribe(x => {
                modalRef.componentInstance.title = 'Good job';
                modalRef.componentInstance.body = 'The Sale Order was created successfully.';
                modalRef.result.then(() => this._router.navigate(['/home']));
                modalRef.componentInstance.canClose = true;

                let email: UserEmail = {
                    userEmail: null,
                    subject: "Sale Order -> " + this.customerSelect.fullName + " | Made by -> " + this._userService.userEmail,
                    body: this.createBody()
                }
                //console.log(email);
                this._qbServices.sentEmail(email).subscribe();

            }, error => {
                console.log(error);
                modalRef.componentInstance.title = 'Bad job';
                modalRef.componentInstance.body = 'The Sale Order error. ' + ((error.error == undefined) ? "" : error.error.error);
                modalRef.componentInstance.canClose = true;
            });
        }
    }

    private createBody(): string {
        let body = `<html xmlns:v="urn:schemas-microsoft-com:vml"
xmlns:o="urn:schemas-microsoft-com:office:office"
xmlns:w="urn:schemas-microsoft-com:office:word"
xmlns:m="http://schemas.microsoft.com/office/2004/12/omml"
xmlns="http://www.w3.org/TR/REC-html40">

<head>
<meta http-equiv=Content-Type content="text/html; charset=windows-1252">
<meta name=ProgId content=Word.Document>
<meta name=Generator content="Microsoft Word 15">
<meta name=Originator content="Microsoft Word 15">
<link rel=File-List href="Untitled_files/filelist.xml">
<link rel=Edit-Time-Data href="Untitled_files/editdata.mso">
<!--[if gte mso 9]><xml>
 <o:OfficeDocumentSettings>
  <o:AllowPNG/>
 </o:OfficeDocumentSettings>
</xml><![endif]-->
<link rel=themeData href="Untitled_files/themedata.thmx">
<link rel=colorSchemeMapping href="Untitled_files/colorschememapping.xml">
<!--[if gte mso 9]><xml>
 <w:WordDocument>
  <w:TrackMoves/>
  <w:TrackFormatting/>
  <w:PunctuationKerning/>
  <w:ValidateAgainstSchemas/>
  <w:SaveIfXMLInvalid>false</w:SaveIfXMLInvalid>
  <w:IgnoreMixedContent>false</w:IgnoreMixedContent>
  <w:AlwaysShowPlaceholderText>false</w:AlwaysShowPlaceholderText>
  <w:DoNotPromoteQF/>
  <w:LidThemeOther>EN-US</w:LidThemeOther>
  <w:LidThemeAsian>X-NONE</w:LidThemeAsian>
  <w:LidThemeComplexScript>X-NONE</w:LidThemeComplexScript>
  <w:Compatibility>
   <w:BreakWrappedTables/>
   <w:SnapToGridInCell/>
   <w:WrapTextWithPunct/>
   <w:UseAsianBreakRules/>
   <w:DontGrowAutofit/>
   <w:SplitPgBreakAndParaMark/>
   <w:EnableOpenTypeKerning/>
   <w:DontFlipMirrorIndents/>
   <w:OverrideTableStyleHps/>
  </w:Compatibility>
  <m:mathPr>
   <m:mathFont m:val="Cambria Math"/>
   <m:brkBin m:val="before"/>
   <m:brkBinSub m:val="&#45;-"/>
   <m:smallFrac m:val="off"/>
   <m:dispDef/>
   <m:lMargin m:val="0"/>
   <m:rMargin m:val="0"/>
   <m:defJc m:val="centerGroup"/>
   <m:wrapIndent m:val="1440"/>
   <m:intLim m:val="subSup"/>
   <m:naryLim m:val="undOvr"/>
  </m:mathPr></w:WordDocument>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <w:LatentStyles DefLockedState="false" DefUnhideWhenUsed="false"
  DefSemiHidden="false" DefQFormat="false" DefPriority="99"
  LatentStyleCount="371">
  <w:LsdException Locked="false" Priority="0" QFormat="true" Name="Normal"/>
  <w:LsdException Locked="false" Priority="9" QFormat="true" Name="heading 1"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 2"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 3"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 4"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 5"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 6"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 7"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 8"/>
  <w:LsdException Locked="false" Priority="9" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="heading 9"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 6"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 7"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 8"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index 9"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 1"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 2"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 3"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 4"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 5"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 6"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 7"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 8"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" Name="toc 9"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Normal Indent"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="footnote text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="annotation text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="header"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="footer"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="index heading"/>
  <w:LsdException Locked="false" Priority="35" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="caption"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="table of figures"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="envelope address"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="envelope return"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="footnote reference"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="annotation reference"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="line number"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="page number"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="endnote reference"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="endnote text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="table of authorities"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="macro"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="toa heading"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Bullet"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Number"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Bullet 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Bullet 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Bullet 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Bullet 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Number 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Number 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Number 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Number 5"/>
  <w:LsdException Locked="false" Priority="10" QFormat="true" Name="Title"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Closing"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Signature"/>
  <w:LsdException Locked="false" Priority="1" SemiHidden="true"
   UnhideWhenUsed="true" Name="Default Paragraph Font"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text Indent"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Continue"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Continue 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Continue 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Continue 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="List Continue 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Message Header"/>
  <w:LsdException Locked="false" Priority="11" QFormat="true" Name="Subtitle"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Salutation"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Date"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text First Indent"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text First Indent 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Note Heading"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text Indent 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Body Text Indent 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Block Text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Hyperlink"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="FollowedHyperlink"/>
  <w:LsdException Locked="false" Priority="22" QFormat="true" Name="Strong"/>
  <w:LsdException Locked="false" Priority="20" QFormat="true" Name="Emphasis"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Document Map"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Plain Text"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="E-mail Signature"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Top of Form"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Bottom of Form"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Normal (Web)"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Acronym"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Address"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Cite"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Code"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Definition"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Keyboard"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Preformatted"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Sample"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Typewriter"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="HTML Variable"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Normal Table"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="annotation subject"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="No List"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Outline List 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Outline List 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Outline List 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Simple 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Simple 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Simple 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Classic 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Classic 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Classic 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Classic 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Colorful 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Colorful 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Colorful 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Columns 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Columns 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Columns 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Columns 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Columns 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 6"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 7"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Grid 8"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 4"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 5"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 6"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 7"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table List 8"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table 3D effects 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table 3D effects 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table 3D effects 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Contemporary"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Elegant"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Professional"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Subtle 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Subtle 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Web 1"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Web 2"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Web 3"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Balloon Text"/>
  <w:LsdException Locked="false" Priority="39" Name="Table Grid"/>
  <w:LsdException Locked="false" SemiHidden="true" UnhideWhenUsed="true"
   Name="Table Theme"/>
  <w:LsdException Locked="false" SemiHidden="true" Name="Placeholder Text"/>
  <w:LsdException Locked="false" Priority="1" QFormat="true" Name="No Spacing"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 1"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 1"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 1"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 1"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 1"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 1"/>
  <w:LsdException Locked="false" SemiHidden="true" Name="Revision"/>
  <w:LsdException Locked="false" Priority="34" QFormat="true"
   Name="List Paragraph"/>
  <w:LsdException Locked="false" Priority="29" QFormat="true" Name="Quote"/>
  <w:LsdException Locked="false" Priority="30" QFormat="true"
   Name="Intense Quote"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 1"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 1"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 1"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 1"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 1"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 1"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 1"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 1"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 2"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 2"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 2"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 2"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 2"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 2"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 2"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 2"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 2"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 2"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 2"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 2"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 2"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 2"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 3"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 3"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 3"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 3"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 3"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 3"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 3"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 3"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 3"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 3"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 3"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 3"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 3"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 3"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 4"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 4"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 4"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 4"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 4"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 4"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 4"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 4"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 4"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 4"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 4"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 4"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 4"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 4"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 5"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 5"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 5"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 5"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 5"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 5"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 5"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 5"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 5"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 5"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 5"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 5"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 5"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 5"/>
  <w:LsdException Locked="false" Priority="60" Name="Light Shading Accent 6"/>
  <w:LsdException Locked="false" Priority="61" Name="Light List Accent 6"/>
  <w:LsdException Locked="false" Priority="62" Name="Light Grid Accent 6"/>
  <w:LsdException Locked="false" Priority="63" Name="Medium Shading 1 Accent 6"/>
  <w:LsdException Locked="false" Priority="64" Name="Medium Shading 2 Accent 6"/>
  <w:LsdException Locked="false" Priority="65" Name="Medium List 1 Accent 6"/>
  <w:LsdException Locked="false" Priority="66" Name="Medium List 2 Accent 6"/>
  <w:LsdException Locked="false" Priority="67" Name="Medium Grid 1 Accent 6"/>
  <w:LsdException Locked="false" Priority="68" Name="Medium Grid 2 Accent 6"/>
  <w:LsdException Locked="false" Priority="69" Name="Medium Grid 3 Accent 6"/>
  <w:LsdException Locked="false" Priority="70" Name="Dark List Accent 6"/>
  <w:LsdException Locked="false" Priority="71" Name="Colorful Shading Accent 6"/>
  <w:LsdException Locked="false" Priority="72" Name="Colorful List Accent 6"/>
  <w:LsdException Locked="false" Priority="73" Name="Colorful Grid Accent 6"/>
  <w:LsdException Locked="false" Priority="19" QFormat="true"
   Name="Subtle Emphasis"/>
  <w:LsdException Locked="false" Priority="21" QFormat="true"
   Name="Intense Emphasis"/>
  <w:LsdException Locked="false" Priority="31" QFormat="true"
   Name="Subtle Reference"/>
  <w:LsdException Locked="false" Priority="32" QFormat="true"
   Name="Intense Reference"/>
  <w:LsdException Locked="false" Priority="33" QFormat="true" Name="Book Title"/>
  <w:LsdException Locked="false" Priority="37" SemiHidden="true"
   UnhideWhenUsed="true" Name="Bibliography"/>
  <w:LsdException Locked="false" Priority="39" SemiHidden="true"
   UnhideWhenUsed="true" QFormat="true" Name="TOC Heading"/>
  <w:LsdException Locked="false" Priority="41" Name="Plain Table 1"/>
  <w:LsdException Locked="false" Priority="42" Name="Plain Table 2"/>
  <w:LsdException Locked="false" Priority="43" Name="Plain Table 3"/>
  <w:LsdException Locked="false" Priority="44" Name="Plain Table 4"/>
  <w:LsdException Locked="false" Priority="45" Name="Plain Table 5"/>
  <w:LsdException Locked="false" Priority="40" Name="Grid Table Light"/>
  <w:LsdException Locked="false" Priority="46" Name="Grid Table 1 Light"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark"/>
  <w:LsdException Locked="false" Priority="51" Name="Grid Table 6 Colorful"/>
  <w:LsdException Locked="false" Priority="52" Name="Grid Table 7 Colorful"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 1"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 1"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 1"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 1"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 1"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 1"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 1"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 2"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 2"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 2"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 2"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 2"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 2"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 2"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 3"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 3"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 3"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 3"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 3"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 3"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 3"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 4"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 4"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 4"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 4"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 4"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 4"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 4"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 5"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 5"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 5"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 5"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 5"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 5"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 5"/>
  <w:LsdException Locked="false" Priority="46"
   Name="Grid Table 1 Light Accent 6"/>
  <w:LsdException Locked="false" Priority="47" Name="Grid Table 2 Accent 6"/>
  <w:LsdException Locked="false" Priority="48" Name="Grid Table 3 Accent 6"/>
  <w:LsdException Locked="false" Priority="49" Name="Grid Table 4 Accent 6"/>
  <w:LsdException Locked="false" Priority="50" Name="Grid Table 5 Dark Accent 6"/>
  <w:LsdException Locked="false" Priority="51"
   Name="Grid Table 6 Colorful Accent 6"/>
  <w:LsdException Locked="false" Priority="52"
   Name="Grid Table 7 Colorful Accent 6"/>
  <w:LsdException Locked="false" Priority="46" Name="List Table 1 Light"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark"/>
  <w:LsdException Locked="false" Priority="51" Name="List Table 6 Colorful"/>
  <w:LsdException Locked="false" Priority="52" Name="List Table 7 Colorful"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 1"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 1"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 1"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 1"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 1"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 1"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 1"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 2"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 2"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 2"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 2"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 2"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 2"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 2"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 3"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 3"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 3"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 3"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 3"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 3"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 3"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 4"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 4"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 4"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 4"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 4"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 4"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 4"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 5"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 5"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 5"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 5"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 5"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 5"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 5"/>
  <w:LsdException Locked="false" Priority="46"
   Name="List Table 1 Light Accent 6"/>
  <w:LsdException Locked="false" Priority="47" Name="List Table 2 Accent 6"/>
  <w:LsdException Locked="false" Priority="48" Name="List Table 3 Accent 6"/>
  <w:LsdException Locked="false" Priority="49" Name="List Table 4 Accent 6"/>
  <w:LsdException Locked="false" Priority="50" Name="List Table 5 Dark Accent 6"/>
  <w:LsdException Locked="false" Priority="51"
   Name="List Table 6 Colorful Accent 6"/>
  <w:LsdException Locked="false" Priority="52"
   Name="List Table 7 Colorful Accent 6"/>
 </w:LatentStyles>
</xml><![endif]-->
<style>
<!--
 /* Font Definitions */
 @font-face
	{font-family:"Cambria Math";
	panose-1:2 4 5 3 5 4 6 3 2 4;
	mso-font-charset:1;
	mso-generic-font-family:roman;
	mso-font-pitch:variable;
	mso-font-signature:0 0 0 0 0 0;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;
	mso-font-charset:0;
	mso-generic-font-family:swiss;
	mso-font-pitch:variable;
	mso-font-signature:-536870145 1073786111 1 0 415 0;}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{mso-style-unhide:no;
	mso-style-qformat:yes;
	mso-style-parent:"";
	margin:0in;
	margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:"Times New Roman";
	mso-bidi-theme-font:minor-bidi;}
a:link, span.MsoHyperlink
	{mso-style-noshow:yes;
	mso-style-priority:99;
	color:#0563C1;
	mso-themecolor:hyperlink;
	text-decoration:underline;
	text-underline:single;}
a:visited, span.MsoHyperlinkFollowed
	{mso-style-noshow:yes;
	mso-style-priority:99;
	color:#954F72;
	mso-themecolor:followedhyperlink;
	text-decoration:underline;
	text-underline:single;}
span.EmailStyle17
	{mso-style-type:personal-compose;
	mso-style-noshow:yes;
	mso-style-unhide:no;
	mso-ansi-font-size:11.0pt;
	mso-bidi-font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:"Times New Roman";
	mso-bidi-theme-font:minor-bidi;
	color:windowtext;}
.MsoChpDefault
	{mso-style-type:export-only;
	mso-default-props:yes;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:"Times New Roman";
	mso-bidi-theme-font:minor-bidi;}
@page WordSection1
	{size:8.5in 11.0in;
	margin:1.0in 1.0in 1.0in 1.0in;
	mso-header-margin:.5in;
	mso-footer-margin:.5in;
	mso-paper-source:0;}
div.WordSection1
	{page:WordSection1;}
-->
</style>
<!--[if gte mso 10]>
<style>
 /* Style Definitions */
 table.MsoNormalTable
	{mso-style-name:"Table Normal";
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-parent:"";
	mso-padding-alt:0in 5.4pt 0in 5.4pt;
	mso-para-margin:0in;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;}
table.MsoTableGrid
	{mso-style-name:"Table Grid";
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-priority:39;
	mso-style-unhide:no;
	border:solid windowtext 1.0pt;
	mso-border-alt:solid windowtext .5pt;
	mso-padding-alt:0in 5.4pt 0in 5.4pt;
	mso-border-insideh:.5pt solid windowtext;
	mso-border-insidev:.5pt solid windowtext;
	mso-para-margin:0in;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;}
table.MsoTable15Grid4Accent5
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-tstyle-rowband-size:1;
	mso-tstyle-colband-size:1;
	mso-style-priority:49;
	mso-style-unhide:no;
	border:solid #8EAADB 1.0pt;
	mso-border-themecolor:accent5;
	mso-border-themetint:153;
	mso-border-alt:solid #8EAADB .5pt;
	mso-border-themecolor:accent5;
	mso-border-themetint:153;
	mso-padding-alt:0in 5.4pt 0in 5.4pt;
	mso-border-insideh:.5pt solid #8EAADB;
	mso-border-insideh-themecolor:accent5;
	mso-border-insideh-themetint:153;
	mso-border-insidev:.5pt solid #8EAADB;
	mso-border-insidev-themecolor:accent5;
	mso-border-insidev-themetint:153;
	mso-para-margin:0in;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;}
table.MsoTable15Grid4Accent5FirstRow
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:first-row;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-tstyle-shading:#4472C4;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-border-top:.5pt solid #4472C4;
	mso-tstyle-border-top-themecolor:accent5;
	mso-tstyle-border-left:.5pt solid #4472C4;
	mso-tstyle-border-left-themecolor:accent5;
	mso-tstyle-border-bottom:.5pt solid #4472C4;
	mso-tstyle-border-bottom-themecolor:accent5;
	mso-tstyle-border-right:.5pt solid #4472C4;
	mso-tstyle-border-right-themecolor:accent5;
	mso-tstyle-border-insideh:cell-none;
	mso-tstyle-border-insidev:cell-none;
	color:white;
	mso-themecolor:background1;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid4Accent5LastRow
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:last-row;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-tstyle-border-top:1.5pt double #4472C4;
	mso-tstyle-border-top-themecolor:accent5;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid4Accent5FirstCol
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:first-column;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid4Accent5LastCol
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:last-column;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid4Accent5OddColumn
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:odd-column;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-tstyle-shading:#D9E2F3;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-shading-themetint:51;}
table.MsoTable15Grid4Accent5OddRow
	{mso-style-name:"Grid Table 4 - Accent 5";
	mso-table-condition:odd-row;
	mso-style-priority:49;
	mso-style-unhide:no;
	mso-tstyle-shading:#D9E2F3;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-shading-themetint:51;}
table.MsoTable15Grid5DarkAccent5
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-tstyle-rowband-size:1;
	mso-tstyle-colband-size:1;
	mso-style-priority:50;
	mso-style-unhide:no;
	border:solid white 1.0pt;
	mso-border-themecolor:background1;
	mso-border-alt:solid white .5pt;
	mso-border-themecolor:background1;
	mso-padding-alt:0in 5.4pt 0in 5.4pt;
	mso-border-insideh:.5pt solid white;
	mso-border-insideh-themecolor:background1;
	mso-border-insidev:.5pt solid white;
	mso-border-insidev-themecolor:background1;
	mso-tstyle-shading:#D9E2F3;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-shading-themetint:51;
	mso-para-margin:0in;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;}
table.MsoTable15Grid5DarkAccent5FirstRow
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:first-row;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#4472C4;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-border-top:.5pt solid white;
	mso-tstyle-border-top-themecolor:background1;
	mso-tstyle-border-left:.5pt solid white;
	mso-tstyle-border-left-themecolor:background1;
	mso-tstyle-border-right:.5pt solid white;
	mso-tstyle-border-right-themecolor:background1;
	mso-tstyle-border-insideh:cell-none;
	mso-tstyle-border-insidev:cell-none;
	color:white;
	mso-themecolor:background1;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid5DarkAccent5LastRow
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:last-row;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#4472C4;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-border-left:.5pt solid white;
	mso-tstyle-border-left-themecolor:background1;
	mso-tstyle-border-bottom:.5pt solid white;
	mso-tstyle-border-bottom-themecolor:background1;
	mso-tstyle-border-right:.5pt solid white;
	mso-tstyle-border-right-themecolor:background1;
	mso-tstyle-border-insideh:cell-none;
	mso-tstyle-border-insidev:cell-none;
	color:white;
	mso-themecolor:background1;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid5DarkAccent5FirstCol
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:first-column;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#4472C4;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-border-top:.5pt solid white;
	mso-tstyle-border-top-themecolor:background1;
	mso-tstyle-border-left:.5pt solid white;
	mso-tstyle-border-left-themecolor:background1;
	mso-tstyle-border-bottom:.5pt solid white;
	mso-tstyle-border-bottom-themecolor:background1;
	mso-tstyle-border-insidev:cell-none;
	color:white;
	mso-themecolor:background1;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid5DarkAccent5LastCol
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:last-column;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#4472C4;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-border-top:.5pt solid white;
	mso-tstyle-border-top-themecolor:background1;
	mso-tstyle-border-bottom:.5pt solid white;
	mso-tstyle-border-bottom-themecolor:background1;
	mso-tstyle-border-right:.5pt solid white;
	mso-tstyle-border-right-themecolor:background1;
	mso-tstyle-border-insidev:cell-none;
	color:white;
	mso-themecolor:background1;
	mso-ansi-font-weight:bold;
	mso-bidi-font-weight:bold;}
table.MsoTable15Grid5DarkAccent5OddColumn
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:odd-column;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#B4C6E7;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-shading-themetint:102;}
table.MsoTable15Grid5DarkAccent5OddRow
	{mso-style-name:"Grid Table 5 Dark - Accent 5";
	mso-table-condition:odd-row;
	mso-style-priority:50;
	mso-style-unhide:no;
	mso-tstyle-shading:#B4C6E7;
	mso-tstyle-shading-themecolor:accent5;
	mso-tstyle-shading-themetint:102;}
</style>
<![endif]--><!--[if gte mso 9]><xml>
 <o:shapedefaults v:ext="edit" spidmax="1026"/>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <o:shapelayout v:ext="edit">
  <o:idmap v:ext="edit" data="1"/>
 </o:shapelayout></xml><![endif]-->
</head>

<body lang=EN-US link="#0563C1" vlink="#954F72" style='tab-interval:.5in'>

<div class=WordSection1>

<p class=MsoNormal style='mso-layout-grid-align:none;text-autospace:none'><span
style='mso-ascii-font-family:Calibri;mso-hansi-font-family:Calibri;mso-bidi-font-family:
Calibri;color:black'><o:p>&nbsp;</o:p></span></p>

<p class=MsoNormal><o:p>&nbsp;</o:p></p>

<table class=MsoTable15Grid5DarkAccent5 border=1 cellspacing=0 cellpadding=0
 style='border-collapse:collapse;border:none;mso-border-alt:solid white .5pt;
 mso-border-themecolor:background1;mso-yfti-tbllook:1248;mso-padding-alt:0in 5.4pt 0in 5.4pt'>
 <tr style='mso-yfti-irow:-1;mso-yfti-firstrow:yes;mso-yfti-lastfirstrow:yes;
  height:13.45pt'>
  <td valign=top style='border:solid white 1.0pt;mso-border-themecolor:background1;
  border-right:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-left-alt:solid white .5pt;mso-border-left-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:5'><a name="_MailAutoSig"><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>#<o:p></o:p></span></b></a></p>
  </td>
   
  <td valign=top style='border-top:solid white 1.0pt;mso-border-top-themecolor:
  background1;border-left:none;border-bottom:solid white 1.0pt;mso-border-bottom-themecolor:
  background1;border-right:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:1'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>ITEMS<o:p></o:p></span></b></span></p>
  </td>
   
  <td valign=top style='border-top:solid white 1.0pt;mso-border-top-themecolor:
  background1;border-left:none;border-bottom:solid white 1.0pt;mso-border-bottom-themecolor:
  background1;border-right:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:1'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>DESCRIPCTION<o:p></o:p></span></b></span></p>
  </td>
   
  <td valign=top style='border-top:solid white 1.0pt;mso-border-top-themecolor:
  background1;border-left:none;border-bottom:solid white 1.0pt;mso-border-bottom-themecolor:
  background1;border-right:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:1'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>ORDERED<o:p></o:p></span></b></span></p>
  </td>
   
  <td valign=top style='border-top:solid white 1.0pt;mso-border-top-themecolor:
  background1;border-left:none;border-bottom:solid white 1.0pt;mso-border-bottom-themecolor:
  background1;border-right:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:1'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>RATE<o:p></o:p></span></b></span></p>
  </td>
   
  <td valign=top style='border:solid white 1.0pt;mso-border-themecolor:background1;
  border-left:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-bottom-alt:solid white .5pt;mso-border-bottom-themecolor:
  background1;mso-border-right-alt:solid white .5pt;mso-border-right-themecolor:
  background1;background:#4472C4;mso-background-themecolor:accent5;padding:
  0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:1'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>AMOUT<o:p></o:p></span></b></span></p>
  </td>
   
 </tr>`;
        let total: number = 0;
        let cont: number = 1;
        (this.formGroup.controls['SalesOrderLineAdd'] as FormArray).controls.forEach(x => {
            let itemLine: item;
            if (typeof x.get('ItemRefListID').value == "string")
                itemLine = this.items.find(z => z.fullName == x.get('ItemRefListID').value) ;
            else
                itemLine = (x.get('ItemRefListID').value as item);
            body = body + `<tr>
 <td valign=top style='border:solid white 1.0pt;mso-border-themecolor:background1;
  border-top:none;mso-border-top-alt:solid white .5pt;mso-border-top-themecolor:
  background1;mso-border-alt:solid white .5pt;mso-border-themecolor:background1;
  background:#4472C4;mso-background-themecolor:accent5;padding:0in 5.4pt 0in 5.4pt;
  height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:68'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>`+ cont+`<o:p></o:p></span></b></span></p>
  </td>

<td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-left-alt:solid white .5pt;
  mso-border-left-themecolor:background1;mso-border-alt:solid white .5pt;
  mso-border-themecolor:background1;background:#B4C6E7;mso-background-themecolor:
  accent5;mso-background-themetint:102;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:64'><span style='mso-bookmark:_MailAutoSig'><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:black;mso-themecolor:text1;mso-no-proof:yes'>`+ itemLine.fullName + `<o:p></o:p></span></span></p>
  </td>

 <td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-left-alt:solid white .5pt;
  mso-border-left-themecolor:background1;mso-border-alt:solid white .5pt;
  mso-border-themecolor:background1;background:#B4C6E7;mso-background-themecolor:
  accent5;mso-background-themetint:102;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:64'><span style='mso-bookmark:_MailAutoSig'><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:black;mso-themecolor:text1;mso-no-proof:yes'>`+ itemLine.salesDesc + `<o:p></o:p></span></span></p>
  </td>

<td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-left-alt:solid white .5pt;
  mso-border-left-themecolor:background1;mso-border-alt:solid white .5pt;
  mso-border-themecolor:background1;background:#B4C6E7;mso-background-themecolor:
  accent5;mso-background-themetint:102;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:64'><span style='mso-bookmark:_MailAutoSig'><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:black;mso-themecolor:text1;mso-no-proof:yes'>`+ x.get('Quantity').value +`<o:p></o:p></span></span></p>
  </td>

  <td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-left-alt:solid white .5pt;
  mso-border-left-themecolor:background1;mso-border-alt:solid white .5pt;
  mso-border-themecolor:background1;background:#B4C6E7;mso-background-themecolor:
  accent5;mso-background-themetint:102;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:64'><span style='mso-bookmark:_MailAutoSig'><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:black;mso-themecolor:text1;mso-no-proof:yes'>`+ x.get('Rate').value +`<o:p></o:p></span></span></p>
  </td>

<td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-left-alt:solid white .5pt;
  mso-border-left-themecolor:background1;mso-border-alt:solid white .5pt;
  mso-border-themecolor:background1;background:#B4C6E7;mso-background-themecolor:
  accent5;mso-background-themetint:102;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:64'><span style='mso-bookmark:_MailAutoSig'><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:black;mso-themecolor:text1;mso-no-proof:yes'>`+ x.get('Amount').value +`<o:p></o:p></span></span></p>
  </td>
  `;
            cont++;
            total += x.get('Amount').value;
        });
body = body + `
 <tr style='mso-yfti-irow:1;mso-yfti-lastrow:yes;height:13.45pt'>
  <td valign=top style='border-top:none;border-left:solid white 1.0pt;
  mso-border-left-themecolor:background1;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:none;mso-border-top-alt:
  solid white .5pt;mso-border-top-themecolor:background1;mso-border-top-alt:
  solid white .5pt;mso-border-top-themecolor:background1;mso-border-left-alt:
  solid white .5pt;mso-border-left-themecolor:background1;mso-border-bottom-alt:
  solid white .5pt;mso-border-bottom-themecolor:background1;background:#4472C4;
  mso-background-themecolor:accent5;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'><span
  style='mso-bookmark:_MailAutoSig'></span>
  <p class=MsoNormal style='mso-yfti-cnfc:6'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'><o:p>&nbsp;</o:p></span></b></span></p>
  </td>
   
  <td colspan=4 valign=top style='border:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-bottom-alt:solid white .5pt;
  mso-border-bottom-themecolor:background1;background:#4472C4;mso-background-themecolor:
  accent5;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:2'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>TOTAL<o:p></o:p></span></b></span></p>
  </td>
   
  <td valign=top style='border-top:none;border-left:none;border-bottom:solid white 1.0pt;
  mso-border-bottom-themecolor:background1;border-right:solid white 1.0pt;
  mso-border-right-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-top-alt:solid white .5pt;
  mso-border-top-themecolor:background1;mso-border-bottom-alt:solid white .5pt;
  mso-border-bottom-themecolor:background1;mso-border-right-alt:solid white .5pt;
  mso-border-right-themecolor:background1;background:#4472C4;mso-background-themecolor:
  accent5;padding:0in 5.4pt 0in 5.4pt;height:13.45pt'>
  <p class=MsoNormal style='mso-yfti-cnfc:2'><span style='mso-bookmark:_MailAutoSig'><b><span
  style='mso-fareast-font-family:"Times New Roman";mso-fareast-theme-font:minor-fareast;
  color:white;mso-themecolor:background1;mso-no-proof:yes'>`+ total+`<o:p></o:p></span></b></span></p>
  </td>
   
 </tr>
</table>
<p class=MsoNormal><o:p>&nbsp;</o:p></p>
</div>
</body>
</html>
`;
        return body
    }

    refil() {
        let filter: InvoceFilter = {
            customerID: this.customerSelect.listID,
            paidStatus: InvocePaidStatus.All,
            overdue: false,
            includeLineItems: true
        }
        this.formGroup.disable();
        this._qbServices.getInvoices(filter).subscribe(x => {
            //console.log(x);
            this.formGroup.enable();
            if (this._userService.type == "Customers")
                this.formGroup.get('CustomerRefListID').disable();

            let array = this.formGroup.get('SalesOrderLineAdd') as FormArray;
            array.clear();
            if (x.invoces.length != 0) {
                x.invoces[x.invoces.length - 1].items.forEach(x => {
                    let grup = this._fb.group({
                        ItemRefListID: new FormControl( this.items.find(z => z.listID == x.listID), Validators.required),
                        Quantity: new FormControl(x.quantity, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")]),
                        Rate: new FormControl(x.rate, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")]),
                        Amount: new FormControl(x.amount, [Validators.required, Validators.pattern("[0-9]+([,\.][0-9]+)?")])
                    });
                    let leng = this.itemFullName.length;
                    grup.get('ItemRefListID').valueChanges.subscribe(() => this.updateAmount(grup, leng));
                    grup.get('Quantity').valueChanges.subscribe(() => this.updateAmount(grup, -1));
                    grup.get('Rate').valueChanges.subscribe(() => this.updateAmount(grup, -1));
                    array.push(grup);
                    this.itemFullName.push('');
                });
            }
        }, e => {
                this.formGroup.enable();
                if (this._userService.type == "Customers")
                    this.formGroup.get('CustomerRefListID').disable();
        });
    }

    goBack() {
        this._router.navigate(['/home']);
    }

    code: string;
    getCode() {
        let nameCustomer: string = this.customerSelect.fullName
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


