import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { ISerie } from '../model/ISerie';
import { SeriesService } from '../services/series.service';
import { IStudy } from '../model/IStudy';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { ImageDialogComponent } from '../image-dialog/image-dialog.component';
import { ErrorDialogComponent } from '../error-dialog/error-dialog.component';


@Component({
  selector: 'hw-series-list',
  templateUrl: './series-list.component.html',
  styleUrls: ['./series-list.component.css']
})
export class SeriesListComponent implements OnInit {
  @Input() url!: string;
  @Input() params!: string;
  @Input() filter!:string;
  @Input() selectModeFromParent: boolean=false;
  @Input() clickedStudyFromParent!: string;
  @Output() mainChecked = new EventEmitter<boolean>();
  @Output() seriesNumber = new EventEmitter<number>();
  @Output() studyUID = new EventEmitter<string>();
  @Output() retrieveErrorMessage=new EventEmitter<HttpErrorResponse|undefined>();

  series:ISerie[]=[];

  clickedStudy!:IStudy;
  selectedStudy!:IStudy;
  errorCount:number=0;
  error:HttpErrorResponse|undefined=undefined;
  address!:string;

  
  mainCheckBox:boolean=false;
  errorText:string="";
  errorTextDev:string="";
  errorMessage:string | undefined=undefined;
  helperText:string="Select a study to see its series";

  constructor(private seriesService:SeriesService,private dialog:MatDialog) {
    
  }

  getSeries(study:IStudy):ISerie[]{
    if(study!=undefined)
    return this.series.filter(s=>s.studyUID==study.studyInstanceUID);
    else return [];
  }

  getSelectedSeries(study:string):ISerie[]{
    return this.series.filter(s=>(s.studyUID==study && s.selected==true));
  }

  studySeriesAlreadyLoaded(uid:string):boolean{
    return this.series.find(serie=>serie.studyUID==uid)==undefined ? false : true; 
  }

  selectSeries():ISerie[]{
      this.series.forEach(serie => {
        if(serie.studyUID==this.selectedStudy.studyInstanceUID){
        serie.selected=this.selectedStudy.selected;}
      });
    this.mainCheckboxSync();
    return this.series;
  }

  onMainCheckBoxChange(e:any): void{
    this.mainCheckBox=e.target.checked;
    this.series.forEach(serie => {
      if(serie.studyUID==this.clickedStudy.studyInstanceUID) serie.selected=e.target.checked});
    this.mainCheckboxSync();
  }

  onCheckBoxChange(e:any): void{
    this.series.forEach(serie => {
      if(e.target.id==serie.seriesInstanceUID) serie.selected=e.target.checked});
    this.mainCheckboxSync();
  }


  Retrieve(studyUid:string, filter:string):any{
      if(filter=="study" && this.series.filter(serie=>serie.studyUID==studyUid).length==this.series.filter(serie=>serie.studyUID==studyUid && serie.selected==true).length)
          //Ha mindegyik ki van jelölve
          this.studyUID.emit(studyUid);
      else{
        //mindegyik serie-t letölti, amelyik ki van jelölve és a studyuid megegyezik
        let count=0; 
        this.errorCount=0; 
        this.series.filter(serie=>serie.selected==true && serie.studyUID==studyUid).forEach(s=>{
          this.seriesService.RetrieveSerie(this.url+"/Retrieve/Series/"+s.studyUID+"/"+s.seriesInstanceUID).subscribe(
            (response)=>{this.address=response.toString(); 
              count++;
              if(count+this.errorCount==this.series.filter(serie=>(serie.selected==true && serie.studyUID==studyUid)).length)
              {
                this.retrieveErrorMessage.emit(this.error);
                this.seriesNumber.emit(count);
              }}
            ,(error:HttpErrorResponse)=>{
              this.errorCount++;
              if(this.error==undefined)this.error=error;
                if(count+this.errorCount==this.series.filter(serie=>(serie.selected==true && serie.studyUID==studyUid)).length){
                  this.retrieveErrorMessage.emit(this.error);
                  this.seriesNumber.emit(count);
                }
            }
          );
          });
      }
    }

  OnStudyClick(clickedStudyUid:IStudy|undefined){
    if(this.clickedStudy!=clickedStudyUid && clickedStudyUid!=undefined){
    this.clickedStudy=clickedStudyUid;
        if(!this.studySeriesAlreadyLoaded(this.clickedStudy.studyInstanceUID)){
          this.manageSeries(false);
        }
        this.mainCheckboxSync();
    }
  }
  
 OnStudySelect(selectedStudyUid:IStudy|undefined){
  if(selectedStudyUid!=undefined)
  {
    this.selectedStudy=selectedStudyUid;
    if(!this.studySeriesAlreadyLoaded(this.selectedStudy.studyInstanceUID)){
        this.manageSeries(true);
    }
    else this.series=this.selectSeries();
  }
  else {this.series.forEach(series=>series.selected=false);this.mainCheckBox=false;}
 }

  ngOnInit(): void {
    this.series.forEach(series=>series.selected=false);
    this.mainCheckBox=false;
  }

  showImage(seriesUID:string,studyUID:string){
    this.dialog.open(ImageDialogComponent,
      {
        data: this.url+"/"+studyUID+"/"+seriesUID
      }
  );
  }

  manageSeries(selected:boolean):void{
    this.seriesService.getSeries(this.url+"/Search/Series/"+this.filter+"/"+this.params).subscribe(
    (series)=> {
      if(this.series==undefined) {console.log(series.length);this.series=series; this.helperText="There are no series for the selected study!"}
      else 
        series.forEach(p=>this.series[this.series.length]=p);
      if(selected) this.series=this.selectSeries();
      if(this.clickedStudy!=undefined && this.clickedStudy==this.selectedStudy)this.mainCheckboxSync();
    },
    (error:HttpErrorResponse)=>{
      if(error.error.userMessage!=undefined){
        this.errorTextDev=error.error.message;
        this.errorText=error.error.userMessage;
      }
      else {
        this.errorText=error.message;
        this.errorTextDev="No additional information";
      }
      });
  }

  mainCheckboxSync():void{
    if(this.series.length!=0){
      let isThereSelected=this.series.filter(serie=>(serie.selected==true && serie.studyUID==this.clickedStudy.studyInstanceUID)).length;
      if(isThereSelected==this.series.filter(serie=>serie.studyUID==this.clickedStudy.studyInstanceUID).length)
       { this.mainCheckBox=true;this.mainChecked.emit(true) }
      else {
        this.mainCheckBox=false;
        if(this.series.filter(serie=>(serie.selected==true && serie.studyUID==this.clickedStudy.studyInstanceUID)).length==0)
        this.mainChecked.emit(false);
        else {this.mainChecked.emit(true);}
      }      
    }
  }

  ngOnDestroy(){
    
  }
}