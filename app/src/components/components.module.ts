import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './navbar/custom-navbar';
import { AdcardComponent } from './adcard/adcard';
import { ListComponent } from './list/list';
import { WishcardComponent } from './wishcard/wishcard';
import { OrdercardComponent } from './ordercard/ordercard';
@NgModule({
	declarations: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
    WishcardComponent,
    OrdercardComponent,
	],
	imports: [
		IonicModule,
	],
	exports: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
    WishcardComponent,
    OrdercardComponent,
	]
})
export class ComponentsModule {}
