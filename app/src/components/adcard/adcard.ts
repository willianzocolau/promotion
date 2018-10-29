import { Component, Input } from '@angular/core';
import { ModalController } from 'ionic-angular';

import { AdvertisingPage } from '../../pages/advertising/advertising'

@Component({
  selector: 'adcard',
  templateUrl: 'adcard.html'
})
export class AdcardComponent {

  @Input() list;
  
  constructor(public modalCtrl: ModalController) {}

  openPage(item: any){
    let modal = this.modalCtrl.create(AdvertisingPage, item);
    modal.onDidDismiss((data) => {
      console.log(data);
    });
    modal.present();
  }

}
