import { Component } from '@angular/core';
import { NavController, NavParams, LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-seller',
  templateUrl: 'seller.html',
})
export class SellerPage {
  public param = "?store_id=1&approved=false";
  public list = [];

  constructor(
    public navCtrl: NavController, 
    public navParams: NavParams,
    public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController) {
    this.getOrders();
  }

  approve(id: number){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("approve", id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.patch(endpoint, {}, headers)
      .then(response => {
        //console.log(dados);
        this.navCtrl.setRoot(SellerPage);
        loading.dismiss();
      })
      .catch(exception => {
        console.log(exception);
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        this.navCtrl.setRoot(SellerPage);
        msg.present();
      });
  }

  disapprove(id: number){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("disapprove", id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.patch(endpoint, {}, headers)
      .then(response => {
        //console.log(dados);
        this.navCtrl.setRoot(SellerPage);
        loading.dismiss();
      })
      .catch(exception => {
        console.log(exception);
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        this.navCtrl.setRoot(SellerPage);
        msg.present();
      });
  }

  delete(id: number){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("",0) + id;
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };

    this.http.delete(endpoint, {}, headers)
      .then(response => {
        //console.log(dados);
        this.navCtrl.setRoot(SellerPage);
        loading.dismiss();
      })
      .catch(exception => {
        console.log(exception);
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        this.navCtrl.setRoot(SellerPage);
        msg.present();
      });
  }

  listar(type: string){
    if(type == "todos"){
      this.param = "?store_id=1";
    }
    else if(type == "aprovados"){
      this.param = "?store_id=1&approved=true";
    }
    else if(type == "nao_aprovados"){
      this.param = "?store_id=1&approved=false";
    }
    this.getOrders();
  }

  getOrders(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("", 0) + this.param;
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        this.list = dados;
        //console.log(dados);
        loading.dismiss();
      })
      .catch(exception => {
        console.log(exception);
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        msg.present();
      });
  }
}
