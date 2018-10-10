import { Component } from "@angular/core";
import { NavController, AlertController, LoadingController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';

import { LoginPage } from "../login/login";

import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";

@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {

  public form : FormGroup;

  constructor(public nav: NavController,
              public formBuilder: FormBuilder,
              public alertCtrl: AlertController,
              public http: HTTP,
              public loadingCtrl: LoadingController,
              public server: ServerStrings,
              public user: UserData) {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      nickname: ['', Validators.required],
      cpf: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', Validators.required],
      confirm_password: ['', Validators.required]
    });
  }

  register() {
    let name: string = this.form.get('name').value;
    let nickname: string = this.form.get('nickname').value;
    let cpf: string = this.form.get('cpf').value.replace('.', '');
    cpf = cpf.replace('.', '');
    cpf = cpf.replace('-', '');
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;
    let c_password: string = this.form.get('confirm_password').value;

    if(password != c_password){
      this.alertCtrl.create({title: 'Senhas não conferem!',buttons: ['Ok']}).present();
      return;
    }

    let loading = this.loadingCtrl.create({ content: 'Registrando...' });
    loading.present();

    let endpoint: string = this.server.auth.register();
    let headers = {
      'Authorization': 'Basic ' + btoa(email + ":" + password),
      'Content-type': 'application/json'
    };
    let body =
    {
      "name": name,
      "nickname": nickname,
      "cpf": cpf,
      "email": email,
      "password": password
    };

    this.http.post(endpoint, body, headers)
      .then(response => {
        this.user.setToken(JSON.parse(response.data).token);
        this.alertCtrl.create({ title: 'Cadastrado com sucesso!', buttons: ['Ok'] }).present();
        console.log("Sucesso");
        loading.dismiss();
        this.nav.setRoot(LoginPage);
      })
      .catch(exception => {
        this.alertCtrl.create({ title: 'Erro: ' + JSON.parse(exception.error).error, buttons: ['Ok'] }).present();
        console.log("Erro");
        loading.dismiss();
      });
  }

  login() {
    this.nav.setRoot(LoginPage);
  }

  doAlert() {
    this.alertCtrl.create({
      title: 'New Friend!',
      subTitle: this.form.get('password').value,
      buttons: ['Ok']
    }).present();
  }
}
