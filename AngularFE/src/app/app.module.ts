import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { VplViewportComponent } from './vpl-viewport/vpl-viewport.component';

@NgModule({
  declarations: [
    AppComponent,
    VplViewportComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
