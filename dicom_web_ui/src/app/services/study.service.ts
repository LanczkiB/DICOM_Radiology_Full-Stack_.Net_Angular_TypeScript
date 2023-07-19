import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable, Subject, tap } from "rxjs";
import { IStudy } from "../model/IStudy";

@Injectable({
    providedIn: 'root'
})
export class StudyService{
    constructor(private http: HttpClient){}

    //search study by url
    getStudies(url:string): Observable<IStudy[]>{
        return this.http.get<IStudy[]>(url).pipe(
            tap(studies=> studies.forEach(study=>{
                study.selected=false;
                study.active=false;
            })
            ));
    }

    //retrieve study by url
    retrieveStudy(url:string){
        console.log(url);
        return this.http.get(url);
    }
}