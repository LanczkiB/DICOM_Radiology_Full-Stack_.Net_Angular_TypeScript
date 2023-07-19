import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable,of,tap, throwError } from "rxjs";
import { IMatchingAttr } from "../model/IMatchingAttr";
import { map } from 'rxjs/operators';
import { DomSanitizer, SafeUrl } from "@angular/platform-browser";

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  constructor(private http: HttpClient,private readonly sanitizer: DomSanitizer) { }

  //retrieve instance image by url
  get_file(url:string):Observable<any>{
    
    return this.http.get<any>(url).pipe(map((response)=>{
       return this.sanitizer.bypassSecurityTrustUrl('assets/' + response.url.toString());
    }
    ))
  }

  //Search instances by url
  getInstanceUIDs(url:string): Observable<any[]>{
    return this.http.get<string[]>(url).pipe(tap(data=>data.toString()));
  }
}
