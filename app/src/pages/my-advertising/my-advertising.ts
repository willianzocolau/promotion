import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';

/**
 * Generated class for the MyAdvertisingPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-my-advertising',
  templateUrl: 'my-advertising.html',
})
export class MyAdvertisingPage {
  private promotions = [];
  constructor(public navCtrl: NavController, public navParams: NavParams) {
    this.promotions = [
      {
        "username": "user1",
        "date": "10-09-2018",
        "description": "Mouse muito legal",
        "likes": 12,
        "comments": 7,
        "image_url": "https://purepng.com/public/uploads/large/purepng.com-pc-mousepc-mousepcmouselaptop-mouse-1701528347542ws1aa.png"
      },
      {
        "username": "user2",
        "date": "11-08-2018",
        "description": "outro Mouse muito legal",
        "likes": 14,
        "comments": 9,
        "image_url": "https://purepng.com/public/uploads/large/purepng.com-pc-mousepc-mousepcmouselaptop-mouse-1701528347542ws1aa.png"
      },
    ];
  }

  criar() {
  }


}
