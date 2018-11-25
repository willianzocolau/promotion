import { Component } from '@angular/core';
import { LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

/**
 * Generated class for the WishlistcardComponent component.
 *
 * See https://angular.io/api/core/Component for more info on Angular
 * Components.
 */
@Component({
  selector: 'wishcard',
  templateUrl: 'wishcard.html'
})
export class WishcardComponent {

  public list = [];

  constructor(public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController) {}
  
  ngAfterViewInit(){
    this.listing();
  }

  listing(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.userWishlist();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(item => {
          this.list.push(item);
        });
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
          let msg = this.alertCtrl.create({message: "Erro: " + dados.error});
          loading.dismiss();
          msg.present();
      });
  }
  delete(id) {
    let loading = this.loadingCtrl.create({ content: 'Aguarde...' });
    loading.present();
    let endpoint: string = this.server.userWishlist() + id;
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.http.delete(endpoint, {}, headers)
      .then(response => {
        console.log("Sucesso");
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
          let msg = this.alertCtrl.create({message: "Erro: " + dados.error});
          loading.dismiss();
          msg.present();
      });
  }
}
