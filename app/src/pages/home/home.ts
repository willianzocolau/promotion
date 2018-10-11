import { Component } from "@angular/core";
import { NavController, NavParams, LoadingController } from "ionic-angular";
import { HTTP } from '@ionic-native/http';
import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})

export class HomePage {
  public promotions = [];
  
  constructor(private http: HTTP,
              public nav: NavController,
              public navParams: NavParams,
              public user: UserData,
              public server: ServerStrings,
              public loadingCtrl: LoadingController) {     
    this.listar_promocoes();
  }

  listar_promocoes(){
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
        console.log("Erro: " + dados.error);
        loading.dismiss();
      });
  }
}
