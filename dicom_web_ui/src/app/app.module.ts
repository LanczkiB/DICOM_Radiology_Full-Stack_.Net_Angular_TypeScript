import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { DicomWebComponent } from './dicom-web/dicom-web.component';
import { MatDialogModule } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StudyListComponent } from './study-list/study-list.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { SeriesListComponent } from './series-list/series-list.component';
import { MaterialModule } from '../material.module';
import { ImageDialogComponent } from './image-dialog/image-dialog.component';
import { ErrorDialogComponent } from './error-dialog/error-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    DicomWebComponent,
    StudyListComponent,
    SeriesListComponent,
    ImageDialogComponent,
    ErrorDialogComponent
  ],
  imports: [
    BrowserModule,
    MaterialModule,
    RouterModule.forRoot([
      {path: 'dicom-web', component: DicomWebComponent},
      {path: 'dicom-web/studies',component: StudyListComponent},
      {path: 'dicom-web/studies/:url/:params/:type',component: StudyListComponent},
      {path: '', redirectTo: 'dicom-web', pathMatch:'full'},
      {path: '**', redirectTo: 'dicom-web', pathMatch: 'full'}
    ]),
    BrowserAnimationsModule,
    MatDialogModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
