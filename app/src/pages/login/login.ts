import {Component} from "@angular/core";
import {NavController, AlertController, ToastController, MenuController} from "ionic-angular";
import {HomePage} from "../home/home";
import {RegisterPage} from "../register/register";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";

@Component({
  selector: 'page-login',
  templateUrl: 'login.html'
})
export class LoginPage {

  public form : FormGroup;
  public data : any;

  constructor(public nav: NavController, 
              public formBuilder: FormBuilder, 
              public forgotCtrl: AlertController, 
              public menu: MenuController, 
              public toastCtrl: ToastController, 
              private httpClient: HttpClient,
              private user: UserData,
              private server: ServerStrings
              ) {
    this.form = this.formBuilder.group({
      email: ['', Validators.email],
      password: ['', Validators.required],
    });
    this.menu.swipeEnable(false);
  }

  // go to register page
  register() {
    this.nav.setRoot(RegisterPage);
  }

  // login and go to home page
  login() {
    this.nav.setRoot(HomePage);  // PULAR O CADASTRO
    let headers = new HttpHeaders();
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));
    let body: string = "";
    let url: string = this.server.auth("login");
    const req = this.httpClient.post(url, body, {headers: headers}).subscribe(
      res => {
        console.log("Sucesso");
        this.data = res;
        this.user.setToken(this.data.token);
        this.user.setEmail(email);
        this.nav.setRoot(HomePage);
      },
      err => {
        console.log("Erro");
        let erro = this.forgotCtrl.create({
          message:  err.error });
        erro.present();
      }
    );
  }

  forgotPass() {
    let forgot = this.forgotCtrl.create({
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

}
