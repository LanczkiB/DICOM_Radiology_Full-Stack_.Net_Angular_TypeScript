import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DicomWebComponent } from './dicom-web.component';

describe('DicomWebComponent', () => {
  let component: DicomWebComponent;
  let fixture: ComponentFixture<DicomWebComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DicomWebComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DicomWebComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
