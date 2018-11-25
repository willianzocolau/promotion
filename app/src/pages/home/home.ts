import { Component } from "@angular/core";
import { NavController, NavParams } from "ionic-angular";
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})

export class HomePage {

  public endpoint = undefined;

  constructor(public nav: NavController, public navParams: NavParams, public server: ServerStrings) {
    this.endpoint = this.server.promotion();
  }

  doInfinite(infiniteScroll) {
    console.log('Begin async operation');
    setTimeout(() => {
      console.log('Async operation has ended');
      infiniteScroll.complete();
    }, 500);
  }
  
}
