import { Component } from '@angular/core';
import { NavController, NavParams, LoadingController, AlertController } from "ionic-angular";
import { HTTP } from '@ionic-native/http';
import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

/**
 * Generated class for the MyAdvertisingPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@Component({
  selector: 'page-wishlist',
  templateUrl: 'wishlist.html',
})
export class WishListPage {
  private wishlist = [{
    "name": "nome",
    "id": 0,
    "register_date": "2018-10-17",
  }];
  private form: FormGroup;
  constructor(private http: HTTP,
              public formBuilder: FormBuilder,
              public nav: NavController,
              public navParams: NavParams,
              public user: UserData,
              public server: ServerStrings,
              public alertCtrl: AlertController,
              public loadingCtrl: LoadingController) {
    this.form = this.formBuilder.group({
      input: ['', Validators.maxLength(50)],
    });
    this.getWishlist();
  }

  getWishlist(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    //loading.present();
    let endpoint: string = this.server.userWishlist();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(item => {
          this.wishlist.push(item);  
        });
        loading.dismiss();
        console.log("Sucesso");
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        console.log("Erro: " + dados.error);
        console.log(exception);
        loading.dismiss();
      });
  }
  add(){
    let loading = this.loadingCtrl.create({ content: 'Adicionando...' });
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
      let endpoint: string = this.server.userWishlist();
        let headers = {
          'Authorization': 'Bearer ' + this.user.getToken(),
          'Content-type': 'application/json'
        };
        let body = {
          "name": input,
        }
        this.http.post(endpoint, body, headers)
          .then(response => {
            let dados = JSON.parse(response.data);
            loading.dismiss();
            console.log(dados);
            console.log("Sucesso");
          })
          .catch(exception => {
            let dados = JSON.parse(exception.error);
            console.log("Erro: " + dados.error);
            console.log(exception);
            loading.dismiss();
          });
      }
    }
}