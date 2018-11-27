import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';


import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

//import { Hero } from './hero';
//import { HttpErrorHandler, HandleError } from '../http-error-handler.service';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json',
    'Authorization': 'my-auth-token'
  })
};

@Injectable()
export class AppService {
  dataUrl = 'api/test';  // URL to web api
  
  constructor( private http: HttpClient ) { }

  /** GET heroes from the server */
  getData (): Observable<any> {
    return this.http.get<any>(this.dataUrl);
  }
}