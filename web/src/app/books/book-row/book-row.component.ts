import { Component, OnInit, Input } from '@angular/core';

import { Book } from '../book'; 

@Component({
  selector: 'app-book-row',
  templateUrl: './book-row.component.html',
  styleUrls: ['./book-row.component.css']
})
export class BookRowComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  @Input() book: any; 

}
