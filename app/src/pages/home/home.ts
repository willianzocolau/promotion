import {Component} from "@angular/core";
import {NavController, PopoverController} from "ionic-angular";
import {Storage} from '@ionic/storage';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";
import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})

export class HomePage {


  constructor(private storage: Storage,
              public nav: NavController,
              public popoverCtrl: PopoverController,
              private user: UserData,
              private server: ServerStrings) {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));
    let body: string = "";
    let url: string = this.server.auth("login");
    const req = this.httpClient.post(url, body, {headers: headers}).subscribe(
      res => {
        console.log("Sucesso");
        this.data = res;
        this.user.setToken(this.data.token);
        this.user.setEmail(email);
        this.nav.setRoot(HomePage);
      },
      err => {
        console.log("Erro");
        let erro = this.forgotCtrl.create({
          message:  err.error + "Para logar use login: user@user.com senha:123abc" });
        erro.present();
      }
    ); 
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

//
