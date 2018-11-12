import { Component } from '@angular/core';
import { NavController, NavParams } from "ionic-angular";
import { CreateAdPage } from "../create-ad/create-ad";


/**
 * Generated class for the MyAdvertisingPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@Component({
  selector: 'page-my-advertising',
  templateUrl: 'my-advertising.html',
})
export class MyAdvertisingPage {
  constructor(public nav: NavController, public navParams: NavParams) {}

  criar() {
    this.nav.setRoot(CreateAdPage);
  }
}
