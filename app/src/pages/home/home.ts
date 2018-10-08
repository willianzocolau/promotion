import {Component} from "@angular/core";
import {NavController, PopoverController} from "ionic-angular";
import {Storage} from '@ionic/storage';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";
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
              public popoverCtrl: PopoverController,
              private user: UserData,
              private server: ServerStrings,
              private http: HTTP) {
            
    /*let url: string = this.server.user();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    console.log(this.user.getToken());

    this.http.get(url , {}, headers)
    .then(data => {
      var dados = JSON.parse(data.data);
      this.user.setId(dados.id);
      this.user.setNickname(dados.nickname);
      this.user.setImage_Url(dados.image_url);
      //this.user.setId(dados.register_date);
      this.user.setType(dados.type);
      this.user.setCredit(dados.credit);
      this.user.setEmail(dados.email);   
      this.user.setName(dados.name);   
      this.user.setState(dados.state);    
    })
    .catch(error => {
      console.log(error);
    });*/
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

  // to go account page
  goToAccount() {
    this.nav.push(SettingsPage);
  }

  presentNotifications(myEvent) {
    console.log(myEvent);
    let popover = this.popoverCtrl.create(NotificationsPage);
    popover.present({
      ev: myEvent
    });
  }

}
