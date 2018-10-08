import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './custom-navbar';

@NgModule({
  declarations: [
    CustomNavComponent,
  ],
  imports: [
    IonicModule,
  ],
  exports: [
    CustomNavComponent
  ],
  entryComponents: [
    CustomNavComponent
  ]
})
export class MyHeaderComponentModule { }
