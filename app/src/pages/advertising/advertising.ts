import { Component } from '@angular/core';
import { NavController, NavParams, ViewController } from 'ionic-angular';

@Component({
  selector: 'page-advertising',
  templateUrl: 'advertising.html',
})
export class AdvertisingPage {
  public item: any = undefined;
  public valor: any = undefined;
  constructor(public navCtrl: NavController, 
    public navParams: NavParams, 
    public viewCtrl: ViewController) {
      this.item = this.navParams.data;
      console.log(this.item);
  }
  closeModal(){
    this.viewCtrl.dismiss(this.item);
  }

}
