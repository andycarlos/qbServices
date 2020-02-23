import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-aler',
  templateUrl: './aler.component.html',
  styleUrls: ['./aler.component.css']
})
export class AlerComponent implements OnInit {
    @Input() title;
    @Input() body;
    @Input() canClose: boolean = true;
    constructor(public activeModal: NgbActiveModal) { }

    ngOnInit() {
    }

    close() {
        if(this.canClose)
          this.activeModal.close();
    }

}
