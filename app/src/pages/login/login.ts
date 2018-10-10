import { Component } from "@angular/core";
import { NavController, AlertController, ToastController, MenuController, LoadingController } from "ionic-angular";
import { HomePage } from "../home/home";
import { RegisterPage } from "../register/register";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";
import { HTTP } from '@ionic-native/http';

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
              private httpClient: HttpClient,
              private user: UserData,
              private server: ServerStrings,
              private http: HTTP,
              private loadingCtrl: LoadingController) {
    this.form = this.formBuilder.group({
      email: ['', Validators.email],
      password: ['', Validators.required],
    });
    this.menu.swipeEnable(false);
    let loading = this.loadingCtrl.create({ content: 'Loading' });
    loading.present();
    this.user.getTokenAsync().then((token) => {
      if (token != null) {
        let endpoint: string = this.server.auth.extend();
        let headers = {
          'Authorization': 'Bearer ' + token,
          'Content-type': 'application/json'
        };

        http.post(endpoint, {}, headers)
          .then(response => {
            var dados = JSON.parse(response.data);
            this.user.setToken(dados.token);

            let endpoint: string = this.server.user();
            let headers = {
              'Authorization': 'Bearer ' + dados.token
            };

            http.get(endpoint, {}, headers)
              .then(res => {
                this.user.update(JSON.parse(res.data));
                loading.dismiss();
                this.nav.setRoot(HomePage);
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
      else {
        loading.dismiss();
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
    let headers = new HttpHeaders();
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));
    let body: string = "";
    let url: string = this.server.auth.login();
    this.httpClient.post(url, body, {headers: headers}).subscribe(
      res => {
        console.log("Sucesso");
        this.data = res;
        this.user.update(res);
        this.nav.setRoot(HomePage);
      },
      err => {
        console.log("Erro");
        let erro = this.alertCtrl.create({
          message:  err.error });
        erro.present();
      }
    );
  }

  forgotPass() {
    let forgot = this.alertCtrl.create({
      title: 'Forgot Password?',
      message: "Enter you email address to send a reset link password.",
      inputs: [
        {
          name: 'email',
          placeholder: 'Email',
          type: 'email'
        },
      ],
      buttons: [
        {
          text: 'Cancel',
          handler: data => {
            console.log('Cancel clicked');
          }
        },
        {
          text: 'Send',
          handler: data => {
            console.log('Send clicked');
            let toast = this.toastCtrl.create({
              message: 'Email was sended successfully',
              duration: 3000,
              position: 'top',
              cssClass: 'dark-trans',
              closeButtonText: 'OK',
              showCloseButton: true
            });
            toast.present();
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
