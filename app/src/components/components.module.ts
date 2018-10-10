import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './navbar/custom-navbar';
import { AdcardComponent } from './adcard/adcard';
@NgModule({
	declarations: [
		AdcardComponent,
		CustomNavComponent,
	],
	imports: [
		IonicModule,
	],
	exports: [
		AdcardComponent,
		CustomNavComponent,
	]
})
export class ComponentsModule {}
