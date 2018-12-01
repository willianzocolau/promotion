import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { LoadingController, AlertController } from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';
import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";
import { MyAdvertisingPage } from '../my-advertising/my-advertising';
/**
 * Generated class for the CreateAdPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@Component({
  selector: 'page-create-ad',
  templateUrl: 'create-ad.html',
})
export class CreateAdPage {

  public form : FormGroup;
  public states = [];
  public stores = [];

  constructor(public navCtrl: NavController, public navParams: NavParams, 
    public formBuilder: FormBuilder, public http:HTTP, public server: ServerStrings,
    public user : UserData, public loadingCtrl : LoadingController, public alertCtrl: AlertController) {
      this.form = this.formBuilder.group({
        name: ['', Validators.required],
        price: ['',Validators.required],
        image: ['',Validators.required],
        store_id: ['',Validators.required],
        state_id: ['',Validators.required]
      });
      this.getStates();
      this.getStores();
  }

  getStores(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.store();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.stores = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        this.stores = JSON.parse(response.data);
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

  getStates(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.state();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    this.states = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        this.states = JSON.parse(response.data);
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

  createAd(){
    let name = this.form.get('name').value;
    let price = this.form.get('price').value;
    let image = this.form.get('image').value;
    let store_id = this.form.get('store_id').value;
    let state_id = this.form.get('state_id').value;

    let loading = this.loadingCtrl.create({ content: 'Registrando...' });
    loading.present();

    let endpoint: string = this.server.promotionRegister();

    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };

    let body = {
        "name": name,
        "price": price,
        "image_url": image,
        "state_id": state_id,
        "store_id": store_id
      }
    
      this.http.post(endpoint, body, headers)
      .then(response => {
        this.alertCtrl.create({ title: 'Cadastrado com sucesso!', buttons: ['Ok'] }).present();
        console.log("Sucesso");
        loading.dismiss();
        this.navCtrl.setRoot(MyAdvertisingPage);
      })
      .catch(exception => {
        this.alertCtrl.create({ title: 'Erro: ' + JSON.parse(exception.error).error, buttons: ['Ok'] }).present();
        console.log("Erro");
        loading.dismiss();
      });
    }
}
