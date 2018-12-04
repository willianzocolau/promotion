import { Component } from "@angular/core";
import { NavController, AlertController, ToastController, MenuController, LoadingController } from "ionic-angular";
import { HomePage } from "../home/home";
import { RegisterPage } from "../register/register";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';

import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";
import { ForgetPasswordPage } from "../password/forgetpassword";
import { SellerPage } from "../seller/seller";

@Component({
  selector: 'page-login',
  templateUrl: 'login.html'
})
export class LoginPage {

  public form : FormGroup;
  public data : any;

  constructor(public nav: NavController, 
              public formBuilder: FormBuilder, 
              public alertCtrl: AlertController, 
              public menu: MenuController, 
              public toastCtrl: ToastController,
              public user: UserData,
              public server: ServerStrings,
              public http: HTTP,
              public loadingCtrl: LoadingController) {
    this.form = this.formBuilder.group({
      email: ['', Validators.email],
      password: ['', Validators.required],
    });
    this.menu.swipeEnable(false);
    
    this.user.getTokenAsync().then((token) => {
      if (token != null) {
        let loading = this.loadingCtrl.create({ content: 'Carregando...' });
        loading.present();

        let endpoint: string = this.server.auth.extend();
        let headers = {
          'Authorization': 'Bearer ' + token,
          'Content-type': 'application/json'
        };

        http.post(endpoint, {}, headers)
          .then(response => {
            let dados = JSON.parse(response.data);
            this.user.setToken(dados.token);

            let endpoint: string = this.server.user();
            let headers = {
              'Authorization': 'Bearer ' + dados.token
            };

            http.get(endpoint, {}, headers)
              .then(res => {
                this.user.update(JSON.parse(res.data));
                loading.dismiss();
                if(this.user.getType() == 1){
                  this.nav.setRoot(SellerPage);
                }
                else{
                  this.nav.setRoot(HomePage);
                }
              })
              .catch(err => {
                loading.dismiss();
                this.user.setToken(null);
              });
          })
          .catch(error => {
            loading.dismiss();
          });
      }
    });
    this.user.getEmailAsync().then((email) => {
      if (email != null)
        this.form.controls['email'].setValue(email.toLowerCase());
    });
  }

  // go to register page
  register() {
    this.nav.setRoot(RegisterPage);
  }

  // login and go to home page
  login() {
    let loading = this.loadingCtrl.create({ content: 'Entrando...' });
    loading.present();

    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;

    let endpoint: string = this.server.auth.login();
    let headers = {
      'Authorization': 'Basic ' + btoa(email + ":" + password),
      'Content-type': 'application/json'
    };

    this.http.post(endpoint, {}, headers)
      .then(response => {
        console.log("Sucesso");
        let dados = JSON.parse(response.data);
        this.data = dados;
        this.user.update(dados);
        loading.dismiss();
        console.log(this.user.getType());
        if(this.user.getType() == 1){
          this.nav.setRoot(SellerPage);
        }
        else{
          this.nav.setRoot(HomePage);
        }
      })
      .catch(exception => {
        console.log("Erro");
        let dados = JSON.parse(exception.error);
        let erro = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        erro.present();
      });
  }

  forgotPass() {
    let forgot = this.alertCtrl.create({
      title: 'Esqueceu sua senha?',
      message: "Informe o seu e-mail para resetar sua senha.",
      inputs: [
        {
          name: 'email',
          placeholder: 'Email',
          type: 'email'
        },
      ],
      buttons: [
        {
          text: 'Cancelar',
          handler: data => {
            console.log('Cancel clicked');
          }
        },
        {
          text: 'Enviar',
          handler: data => {
            let loading = this.loadingCtrl.create({ content: 'Enviando...' });
            loading.present();

            let email: string = data.email;
            console.log(data.email);

            let endpoint: string = this.server.auth.reset();

            let headers = {
              'Content-type': 'application/json'
            };

            let body = {"email": email};

            this.http.post(endpoint, body, headers)
              .then(response => {
                console.log("Sucesso");
                loading.dismiss();
                this.nav.setRoot(ForgetPasswordPage);
              })
              .catch(exception => {
                console.log("Erro");
                let dados = JSON.parse(exception.error);
                let erro = this.alertCtrl.create({
                  message: "Erro: " + dados.error
                });
                loading.dismiss();
                erro.present();
              });
          }
        }
      ]
    });
    forgot.present();
  }

  moveFocus(nextElement) {
    nextElement.setFocus();
  }

}
