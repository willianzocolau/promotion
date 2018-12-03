import { NgModule } from '@angular/core';
import { IonicModule } from 'ionic-angular';
import { CustomNavComponent } from './navbar/custom-navbar';
import { AdcardComponent } from './adcard/adcard';
import { ListComponent } from './list/list';
import { WishcardComponent } from './wishcard/wishcard';
import { OrdercardComponent } from './ordercard/ordercard';
import { HistorycardComponent } from './historycard/historycard';
import { ImguploadComponent } from './imgupload/imgupload';
@NgModule({
	declarations: [
		AdcardComponent,
		CustomNavComponent,
    ListComponent,
    WishcardComponent,
    OrdercardComponent,
    HistorycardComponent,
    ImguploadComponent,
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
    HistorycardComponent,
    ImguploadComponent,
	]
})
export class ComponentsModule {}
