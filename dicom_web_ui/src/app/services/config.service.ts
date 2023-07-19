import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable,of,tap, throwError } from "rxjs";
import { IMatchingAttr } from "../model/IMatchingAttr";

@Injectable({
    providedIn: 'root'
})
export class ConfigService{
    private attributesUrl = 'assets/attributes.json';

    constructor(private http: HttpClient){}

    getServers(url:string): Observable<string[]>{
        return this.http.get<string[]>(url + "Server");
    }

    getAttributes(url:string): Observable<IMatchingAttr[]>{
        return this.http.get<IMatchingAttr[]>(url+"Attributes");
    }

    getHardCodedAttributes(): Observable<IMatchingAttr[]> {
        return this.http.get<IMatchingAttr[]>(this.attributesUrl);
      }
}