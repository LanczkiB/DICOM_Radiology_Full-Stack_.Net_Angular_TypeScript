import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { UrlTree } from '@angular/router';
import { Subscription } from 'rxjs/internal/Subscription';
import { ImageService } from '../services/instance.service';

@Component({
  selector: 'hw-image-dialog',
  templateUrl: './image-dialog.component.html',
  styleUrls: ['./image-dialog.component.css']
})

export class ImageDialogComponent implements OnInit {
    imageToShow!:SafeUrl;
    instances:string[]=[];
    url:string;
    seriesUID:string;
    studyUID:string;
    instanceUID!:string;
    counter:number=0;
    done:boolean=false;
    
    
    constructor(
      public dialogRef: MatDialogRef<ImageDialogComponent>,
      @Optional() @Inject(MAT_DIALOG_DATA) public myCustomData: any,private imageService:ImageService
    ) {
      let temp:string=myCustomData;
      this.url=temp;
      this.seriesUID=temp.split("/")[5];
      this.studyUID=temp.split("/")[4];
      console.log(myCustomData);

    }
  
    ngOnInit(): void {
      this.imageService.getInstanceUIDs(this.url.replace(this.studyUID,"Search").replace(this.seriesUID,"Instances/")+"StudyInstanceUID="+this.studyUID+"&SeriesInstanceUID="+this.seriesUID).subscribe(
      (response)=>{
        console.log(response)
        response.forEach(instance=>this.instances.push(instance.SOPInstanceUID.toString()))
        console.log(this.instances[0]);
        this.refreshImage();
        setTimeout(() => this.done=true,500);
      }
      );  
    }

    refreshImage(){
      this.instanceUID = this.instances[this.counter];
      this.imageService.get_file(this.url.replace(this.studyUID,"Retrieve").replace(this.seriesUID,"Rendered") + "/Instances/"+this.studyUID+"/"+this.seriesUID+"/"+this.instances[this.counter] + "/.jpeg").subscribe(
        (response)=>{this.imageToShow=response}
      );
    }
  
    modalDialogExit(){
      this.dialogRef.close();
    }

    modalDialogNext() {
      if(this.counter+1<this.instances.length)this.counter++;
      this.refreshImage();
    }

    modalDialogPrevious() {
      if(this.counter>0)this.counter--;
      this.refreshImage();
    }
    
  }