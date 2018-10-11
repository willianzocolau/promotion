import { Component } from '@angular/core';
import { NavController, NavParams, AlertController, LoadingController, IonicPage } from "ionic-angular";
import { HTTP } from '@ionic-native/http';
import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

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
  constructor(private http: HTTP,
              public alertCtrl: AlertController,
              public nav: NavController,
              public navParams: NavParams,
              public user: UserData,
              public server: ServerStrings,
              public loadingCtrl: LoadingController) {
    this.minhas_promocoes();
  }

  minhas_promocoes(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.promotionUserId(this.user.getId());
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(promotion => {
          this.promotions.push({promotion, user: this.user.get()});  
        });
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        msg.present();
      });
      console.log(this.promotions);
  }


}
