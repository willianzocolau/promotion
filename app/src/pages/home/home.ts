import {Component} from "@angular/core";
import {NavController} from "ionic-angular";
import {Storage} from '@ionic/storage';

import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";

import { HTTP } from '@ionic-native/http';

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})

export class HomePage {
  public promotions = [];
  constructor(private storage: Storage,
              public nav: NavController,
              private user: UserData,
              private server: ServerStrings,
              private http: HTTP) {
            
    this.promotions = [
        {
            "username": "andre",
            "date": "10-09-2018",
            "description": "Mouse muito legal",
            "likes": 12,
            "comments": 7,
            "image_url": "https://purepng.com/public/uploads/large/purepng.com-pc-mousepc-mousepcmouselaptop-mouse-1701528347542ws1aa.png"
        },
        {
            "username": "andre",
            "date": "11-08-2018",
            "description": "outro Mouse muito legal",
            "likes": 16,
            "comments": 10,
            "image_url": "https://purepng.com/public/uploads/large/purepng.com-pc-mousepc-mousepcmouselaptop-mouse-1701528347542ws1aa.png"
        },

    ];
  }
}
