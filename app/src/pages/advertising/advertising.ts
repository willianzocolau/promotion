import { Component } from '@angular/core';
import { NavController, NavParams, ViewController, LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-advertising',
  templateUrl: 'advertising.html',
})
export class AdvertisingPage {
  public item: any = undefined;
  public valor: any = undefined;
  constructor(public navCtrl: NavController, 
    public navParams: NavParams, 
    public viewCtrl: ViewController,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController,
    public user: UserData,
    public server: ServerStrings,
    public http: HTTP) {
      this.item = this.navParams.data;
      console.log(this.item);
  }
  closeModal(){
    this.viewCtrl.dismiss(this.item);
  }
  Vote(is_positive: boolean){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();

    let endpoint: string = this.server.order("vote",this.item.promotion.id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    let body = {
      "is_positive": is_positive,
      "comment": "comentario"
    }
    this.http.post(endpoint, body, headers)
      .then(response => {
        console.log("Sucesso Advertising");
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({message: "Erro: " + dados});
        loading.dismiss();
        msg.present();
        console.log(exception);
      });
      loading.dismiss();
  }
}
