import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './navbar/custom-navbar';
import { AdcardComponent } from './adcard/adcard';
import { ListComponent } from './list/list';
import { WishcardComponent } from './wishcard/wishcard';
@NgModule({
	declarations: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
    WishcardComponent,
	],
	imports: [
		IonicModule,
	],
	exports: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
    WishcardComponent,
	]
})
export class ComponentsModule {}
