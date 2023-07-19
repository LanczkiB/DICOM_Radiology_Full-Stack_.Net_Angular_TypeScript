import { Component, OnInit } from '@angular/core';
import { IMatchingAttr } from '../model/IMatchingAttr';
import { Router} from '@angular/router';
import { ConfigService } from '../services/config.service';
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { SafeUrl } from '@angular/platform-browser';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../error-dialog/error-dialog.component';

@Component({
  templateUrl: './dicom-web.component.html',
  styleUrls: ['./dicom-web.component.css']
})

export class DicomWebComponent implements OnInit {
  server:string="Sample";
  servers:string[]=[];
  allAttr:IMatchingAttr[]=[];
  urlParams:string="noMatchingAttributes";
  settings: boolean=false;
  url:string="https://localhost:5001/";
  sub!: Subscription;
  attrsub!: Subscription;
  errorText:string="Error: No server configured";
  serverErrorText:string="Error occured during server configuration!";
  errorTextDev:string="Check if backend server is running on localhost:5000";
  serverError:boolean=true
  attributesError:boolean=false;
  
  constructor(private router: Router,private configService:ConfigService,private dialog:MatDialog) { 
      this.sub=configService.getHardCodedAttributes().subscribe(
      (attributes)=>{
        attributes.forEach(attribute=>{attribute.selected=true; attribute.attrValue="";})
        this.allAttr=attributes}
    );
  }


  ngOnInit(): void {
    this.sub=this.configService.getServers(this.url).subscribe(
      (response)=>{
        this.serverError=false;
        this.servers=response;
        this.attrsub=this.configService.getAttributes(this.url).subscribe(
          (response)=> {
            response.forEach(attr=>attr.attrValue="");
            response.forEach(attr=>attr.selected=false);
            this.allAttr.push(...response);
          },(error:HttpErrorResponse)=>{
            if(!this.serverError){this.attributesError=true;
              this.errorTextDev=error.error.message;
              this.errorText=error.error.userMessage;
          }
        });
      },
      (error:HttpErrorResponse)=>{
        this.serverError=true;
        if(error.error.userMessage!=undefined){
          this.errorTextDev=error.error.message;
          this.errorText=error.error.userMessage;
        }
      }
    );
  }

  setAttribute(attribute:string,value:boolean):void{
    this.allAttr.forEach((a:IMatchingAttr) => {
      if(a.attrName==attribute) { a.selected=value;return;}
    });
  }

  OpenErrorDialog():void{
    this.dialog.open(ErrorDialogComponent,
      {
        data: [this.errorText,"Failure!",this.errorTextDev]
      })
  }

  onSettingClick(): void{
    this.settings=!this.settings;
  }

  onSearch(): void{
    this.allAttr.forEach((a:IMatchingAttr) => {
      if(a.selected && a.attrValue!="") { 
        if(this.urlParams=="noMatchingAttributes") this.urlParams=a.attrName.replace(" ","")+"="+a.attrValue;
        else this.urlParams+="&"+a.attrName.replace(" ","")+"="+a.attrValue;
      }});
    console.log(this.helpAttributes());
    this.router.navigate(['dicom-web/studies',this.url+this.server,this.urlParams,this.helpAttributes()]);
  }

  helpAttributes():string{
    let output="study"
    this.allAttr.forEach(attr=>{if(attr.selected==true && attr.type=="series" && (attr.attrValue!="")) output="series"});
    return output;
  }

  getSeriesAttributes():IMatchingAttr[]{
    return this.allAttr.filter(attr=>attr.type=="series");
  }

  getStudyAttributes():IMatchingAttr[]{
    return this.allAttr.filter(attr=>attr.type=="study");
  }

  getSelectedAttributes():IMatchingAttr[]{
    return this.allAttr.filter(attr=>attr.selected);
  }

  onCancelClick(e:any):void{
    this.allAttr.forEach((a:IMatchingAttr) => {
       a.attrValue="";
    });
    this.server="";
  }

  ngOnDestroy(){
    this.sub.unsubscribe();
    this.attrsub.unsubscribe();
  }
}
