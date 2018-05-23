import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VplViewportComponent } from './vpl-viewport.component';

describe('VplViewportComponent', () => {
  let component: VplViewportComponent;
  let fixture: ComponentFixture<VplViewportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VplViewportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VplViewportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
