import { Component } from "@angular/core";
import { NavController, NavParams, AlertController, LoadingController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';
import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-search',
  templateUrl: 'search.html'
})

export class SearchPage {
  public form: FormGroup;

  public promotions = [];

  constructor(public formBuilder: FormBuilder,
              private http: HTTP,
              public alertCtrl: AlertController,
              public nav: NavController,
              public navParams: NavParams,
              public user: UserData,
              public server: ServerStrings,
              public loadingCtrl: LoadingController) {
    this.form = this.formBuilder.group({
      input: ['', Validators.maxLength(50)],
    });
  }

  // search by item
  searchBy(item) {
  }

  pesquisa() {
    let loading = this.loadingCtrl.create({ content: 'Pesquisando...' });
    loading.present();

    let input: string = this.form.get('input').value;

    if (!input) {
      let msg = this.alertCtrl.create({
        title: "Inválido",
        message: "Por favor, digite algo na caixa de texto"
      });
      loading.dismiss();
      msg.present();
    }
    else {
      let endpoint: string = this.server.promotionSearch(input);
      let headers = {
        'Authorization': 'Bearer ' + this.user.getToken()
      };
      this.promotions = [];

      this.http.get(endpoint, {}, headers)
        .then(response => {
          let dados = JSON.parse(response.data);
          dados.forEach(promotion => {
            let endpoint = this.server.userId(promotion.user_id);
            this.http.get(endpoint, {}, headers)
              .then(response => {
                let user = JSON.parse(response.data);
                this.promotions.push({promotion, user});  
              })
              .catch(exception => {
                let dados = JSON.parse(exception.error);
                let msg = this.alertCtrl.create({
                  message: "Erro: " + dados.error
                });
                msg.present();
              });
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
    }
  }
}
