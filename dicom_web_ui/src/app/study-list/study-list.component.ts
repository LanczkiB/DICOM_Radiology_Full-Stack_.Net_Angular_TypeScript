import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import {  Subscription } from 'rxjs';
import { IStudy} from '../model/IStudy';
import { StudyService } from '../services/study.service';
import { IMatchingAttr } from '../model/IMatchingAttr';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialogComponent } from 'src/app/mat-dialog/mat-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../error-dialog/error-dialog.component';
import { SeriesListComponent } from '../series-list/series-list.component';

@Component({
  templateUrl: './study-list.component.html',
  styleUrls: ['./study-list.component.css']
})
export class StudyListComponent implements OnInit {

  studies: IStudy[]=[];
  studiesSub!:Subscription;
  params:IMatchingAttr[]=[];
  outParams:string="no";
  clicked:string="";
  url:string="";
  selectMode:boolean=false;
  mainCheckBox:boolean=false;
  Filter:string="study";
  errorText:string=""
  errorTextDev:string="";
  errorMessage:string | undefined=undefined;
  studiesSuccess:number=0;
  seriesSuccess:number=0;
  sumSuccess:number=0;
  countError=0;

  @ViewChild(SeriesListComponent)
  private seriesComponent!: SeriesListComponent;

  constructor(private route: ActivatedRoute, private router:Router, private studyService:StudyService, private dialog:MatDialog) {

   }

  onRetrieve(): void{
    this.studiesSuccess=0;
    this.seriesSuccess=0;
    this.countError=0;
    this.studies.filter(study=>study.selected).forEach(retrieveStudy=>{
      this.seriesComponent.Retrieve(retrieveStudy.studyInstanceUID,this.Filter);
    });
  }

  setErrorMessages(error:HttpErrorResponse){
    if(error.error.userMessage!=undefined){
      this.errorTextDev=error.error.message;
      this.errorText=error.error.userMessage;
    }
    else {
      this.errorText=error.message;
      this.errorTextDev="No additional information";
    }
  }

  handleRetrieveError(error:HttpErrorResponse){
    console.log(this.seriesSuccess);
    if(error!=undefined){
        if(this.countError==0){
        this.setErrorMessages(error);
      }
    this.countError++;
    }
    else this.sumSuccess++;
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      let temp_url=this.parseParams(params);
      this.studiesSub=this.studyService.getStudies(temp_url).subscribe(
        (response)=> {
          this.studies=response;
          if(this.studies.length==0)
          {
            this.errorText="No studies found with the applied parameters. Search for other studies instead!";
            this.errorTextDev="No additional information";
          }
        },
          (error:HttpErrorResponse)=>{
            this.setErrorMessages(error);
          }
        )
      });
  }

  parseParams(params:Params):string{
    this.url = params['url'];
    this.outParams=params['params'];
    this.Filter=params['type'];
    let temp_url=this.url+"/Search/Study";
    if(this.outParams!=""){
      if(this.Filter=="series") temp_url+="/series";
      else temp_url+="/study";
      temp_url+="/"+this.outParams;
    }
    return temp_url;
  }

  onChecked(checked:boolean) :void {
    this.studies.find(x=>x.studyInstanceUID==this.clicked)!.selected=checked;
  }


  createParams():void{
    this.params.forEach((a:IMatchingAttr) => {
      if(a.selected && a.attrValue!="") { 
        if(this.outParams=="") this.outParams=a.attrName.replace(" ","")+"="+a.attrValue;
        else this.outParams+="&"+a.attrName.replace(" ","")+"="+a.attrValue;
      }} );
  }

  onSeriesSuccess(seriesNumber:number):void{
    this.seriesSuccess+=seriesNumber
    console.log(this.seriesSuccess);
    this.checkIfRetrieveFinished();
  }

  onStudyRetrieve(studyInstanceUID:string){
    this.studyService.retrieveStudy(this.url+"/Retrieve/Study/"+studyInstanceUID).subscribe(
      (response)=>{
        this.studiesSuccess++;
        this.sumSuccess++;
        this.checkIfRetrieveFinished();
    },(error:HttpErrorResponse)=>{
      this.handleRetrieveError(error);
      this.checkIfRetrieveFinished();
    });
    }

  checkIfRetrieveFinished(){
    if(this.sumSuccess+this.countError==this.studies.filter(study=>study.selected==true).length)
    {
      if(this.sumSuccess>0){
        let text="";
        if(this.studiesSuccess>0) text=this.studiesSuccess+ " full study and "+this.seriesSuccess+" series successfully retrieved!";
        else text=this.seriesSuccess+" series successfully retrieved!";
      
        this.dialog.open(MatDialogComponent,
          {
            data: this.seriesSuccess==0 ? [this.studiesSuccess+" full study successfully retrieved!", "Success!","Find dicom file(s) at: "] : 
            [text, "Success!","Find dicom file(s) at: "]
          })
        }
      if(this.countError>0)
        this.dialog.open(ErrorDialogComponent,
          {
            data: [this.errorText+" ("+this.countError+" additional error(s) found)","Failure!",this.errorTextDev]
          });
      this.studiesSuccess=0;
      this.seriesSuccess=0;
      this.sumSuccess=0;
      this.countError=0;
    }
  }


  onSelect():void{
    this.selectMode=!this.selectMode;
    this.seriesComponent.OnStudySelect(undefined);
    this.mainCheckBox=false;
    this.studies.forEach(study => study.selected=false);
  }

  getStudy(uid:string):IStudy|undefined{
    return this.studies.find(study=> study.studyInstanceUID==uid);
  }

  isThereSelectedStudy():boolean{
    return this.studies.find(study=>study.selected==true)==undefined ? false : true; 
  }

  onClick(e:any, uid:string):void{
    this.clicked=uid;
    this.seriesComponent.OnStudyClick(this.getStudy(uid));
  }

  onCheckBoxChange(e:any): void{
    this.getStudy(e.target.id)!.selected=e.target.checked;
    if(!this.isThereSelectedStudy()) this.mainCheckBox=false;
    this.seriesComponent.OnStudySelect(this.getStudy(e.target.id))
  }

  onMainCheckBoxChange(e:any): void{
    this.mainCheckBox=e.target.checked;
    this.studies.forEach(study => {
      study.selected=e.target.checked;
      this.seriesComponent.OnStudySelect(study);
    });
  }

  onBack(): void{
    this.router.navigate(['dicom-web',]);
  }

  ngOnDistroy(){
    this.studiesSub.unsubscribe();
  }
}