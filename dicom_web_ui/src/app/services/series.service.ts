import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Observable, catchError, tap, throwError, filter, map, Subject, Subscription } from "rxjs";
import { IStudy } from "../model/IStudy";
import { ISerie } from "../model/ISerie";

@Injectable({
    providedIn: 'root'
})
export class SeriesService{
    constructor(private http: HttpClient){}
    //Search series by url
    getSeries(url:string): Observable<ISerie[]>{
        return this.http.get<ISerie[]>(url).pipe(
            tap(series=> series.forEach(serie=>serie.selected=false)));
    }

    //Retrieve series by url
    RetrieveSerie(url:string){
        return this.http.get(url);
    }
}