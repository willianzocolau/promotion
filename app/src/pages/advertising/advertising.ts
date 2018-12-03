import { Component } from '@angular/core';
import { LoadingController, AlertController, NavController, NavParams, ViewController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-advertising',
  templateUrl: 'advertising.html',
})
export class AdvertisingPage {
  public item: any = undefined;
  public votes = [];
  constructor(public navCtrl: NavController, 
    public navParams: NavParams, 
    public viewCtrl: ViewController,
    public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController) {
      this.item = this.navParams.data;
      console.log(this.item);
      this.dadosPromocao(this.item.promotion.id);
  }
  closeModal(){
    this.viewCtrl.dismiss(this.item);
  }

  dadosPromocao(id: number){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();

    let endpoint = this.server.promotionId(id);
    
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    
    this.http.get(endpoint, {}, headers)
    .then(response => {
      let promotion = JSON.parse(response.data);
      console.log(promotion);
      this.votes = promotion.votes;
      loading.dismiss();
    })
    .catch(exception =>{
      let dados = JSON.parse(exception.error);
      let msg = this.alertCtrl.create({
        message: "Erro: " + dados.error
      });
      loading.dismiss();
      msg.present();
    });
  }
}
