import {Component} from "@angular/core";
import {ViewController} from "ionic-angular";
import { LoadingController, AlertController } from "ionic-angular";
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-notifications',
  templateUrl: 'notifications.html'
})

export class NotificationsPage {
  private notifications = [];
  constructor(
    public viewCtrl: ViewController,
    public alertCtrl: AlertController,
    public loadingCtrl: LoadingController,
    public user: UserData,
    public server: ServerStrings,
    public http: HTTP ) {
    this.getNotifications();
  }

  async promotionName(id: number) {
    let endpoint: string = this.server.promotionId(id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    let response = await this.http.get(endpoint, {}, headers);
    if(response.data != null){
      let dados = JSON.parse(response.data);
      let resp: string = dados.name;
      return resp;
    }
    return null;
  }

  notificationItem(notification: any){
    let name = this.promotionName(notification.promotion_id).catch(excepion => {
      console.log(excepion.error);
    });
    if(notification.is_active){
      let item = {
        "id": notification.id,
        "name": name,
        "register_date": notification.register_date,
        "is_active": notification.is_active,
        "user_id": notification.user_id,
        "promotion_id": notification.promotion_id,
        "back_color": "secondary",
        "text_color": "white",
        "icon": "mail"
      }
      return item;
    }
    else{
      let item = {
        "id": notification.id,
        "name": name,
        "register_date": notification.register_date,
        "is_active": notification.is_active,
        "user_id": notification.user_id,
        "promotion_id": notification.promotion_id,
        "back_color": "white",
        "text_color": "secondary",
        "icon": "mail-open"
      }
      return item;
    }
  }

  getNotifications(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.userMatchs();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.matchs.forEach(item => {
          this.notifications.push(this.notificationItem(item));
        });
        loading.dismiss();
        console.log("Sucesso: Notification");
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
          let msg = this.alertCtrl.create({message: "Erro: " + dados.error});
          loading.dismiss();
          msg.present();
          console.log(exception);
      });
  }

  close(id: number) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.userMatchsId(id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.http.patch(endpoint, {}, headers)
      .then(response => {
        loading.dismiss();
        console.log("Sucesso: close(id)");
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
          let msg = this.alertCtrl.create({message: "Erro: " + dados.error});
          loading.dismiss();
          msg.present();
          console.log(exception);
      });
    this.viewCtrl.dismiss();
  }
}
