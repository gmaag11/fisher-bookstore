import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs'; 
import { catchError, map, tap } from 'rxjs/operators';
import { Book } from './book';

@Injectable({
  providedIn: 'root'
})
export class BooksService {

  private booksUri = 'https://localhost:5001/api/books';
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }

  constructor(private http: HttpClient) { }

  getBooks(){
    return this.http.get<Book[]>(this.booksUri).pipe(
      tap(_ => console.log('fetched books')),
      catchError(this.handleError<Book[]>('getBooks', []))
    );
  }
}
