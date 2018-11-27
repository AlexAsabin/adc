import { Component, OnInit } from '@angular/core';
import { AppService } from './app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{ 
  title = 'Advanced Data Convertor';
  data: string;

  constructor(private appService: AppService){ }
  
  ngOnInit() {
    this.getData();
  }

  getData(): void {
    this.appService.getData()
      .subscribe(data => this.data = data.name as string);
  }  
}
