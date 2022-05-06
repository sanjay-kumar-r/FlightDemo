import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-discounts',
  templateUrl: './discounts.component.html',
  styleUrls: ['./discounts.component.css']
})
export class DiscountsComponent implements OnInit {

  @Input() data :any;
  constructor() { 
    console.log("data: ", this.data);
  }

  ngOnInit(): void {
    console.log("data: ", this.data);
  }

}
