import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { MyAdvertisingPage } from './my-advertising';

@NgModule({
  declarations: [
    MyAdvertisingPage,
  ],
  imports: [
    IonicPageModule.forChild(MyAdvertisingPage),
  ],
})
export class MyAdvertisingPageModule {}
