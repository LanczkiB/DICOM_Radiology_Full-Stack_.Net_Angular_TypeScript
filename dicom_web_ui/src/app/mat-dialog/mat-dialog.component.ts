import { Component, Inject, OnInit, Optional } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
    templateUrl: './mat-dialog.component.html',
    styleUrls: ['./mat-dialog.component.css']
  })

export class MatDialogComponent implements OnInit {
    fromWebPage!: string;
    fromWebDialog!: string;
    title:string;
    message:string;
    detail:string;
    
    constructor(
      public dialogRef: MatDialogRef<MatDialogComponent>,
      @Optional() @Inject(MAT_DIALOG_DATA) public myCustomData: any
    ) {
      this.title=myCustomData[1];
      this.message=myCustomData[0]
      this.detail=myCustomData[2]
    }
  
    ngOnInit(): void {
    }
  
    modalDialogYes() {
      this.dialogRef.close();
    }
    
  }