import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'hw-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.css']
})
export class ErrorDialogComponent implements OnInit {

  title:string;
  message:string;
  detail:string;
  errorMessage:string|undefined=undefined;

  constructor(
    public dialogRef: MatDialogRef<ErrorDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public myCustomData: any
  ) {
    this.title=myCustomData[1];
    this.message=myCustomData[0]
    this.detail=myCustomData[2]
  }

  ngOnInit(): void {
    console.log("popup")
  }

  showMessage(){
    if(this.errorMessage==undefined) this.errorMessage=this.detail;
    else this.errorMessage=undefined;
  }

  modalDialogYes() {
    this.dialogRef.close();
  }
}
