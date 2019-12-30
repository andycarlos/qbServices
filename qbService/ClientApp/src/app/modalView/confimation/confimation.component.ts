import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confimation',
  templateUrl: './confimation.component.html',
  styleUrls: ['./confimation.component.css']
})
export class ConfimationComponent implements OnInit {
    @Input() title;
    @Input() body;
    constructor(public activeModal: NgbActiveModal) { }

    ngOnInit() {
  }

}
