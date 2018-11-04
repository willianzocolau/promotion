import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './navbar/custom-navbar';
import { AdcardComponent } from './adcard/adcard';
import { ListComponent } from './list/list';
@NgModule({
	declarations: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
	],
	imports: [
		IonicModule,
	],
	exports: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
	]
})
export class ComponentsModule {}
