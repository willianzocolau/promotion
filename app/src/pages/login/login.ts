import {Component} from "@angular/core";
import {NavController, AlertController, ToastController, MenuController} from "ionic-angular";
import {HomePage} from "../home/home";
import {RegisterPage} from "../register/register";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';

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
              private httpClient: HttpClient) {
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
    let headers = new HttpHeaders();
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));
    let body: string = "";
    const req = this.httpClient.post('http://178.128.186.9/api/auth/login/', body, {headers: headers}).subscribe(
      res => {
        console.log("Sucesso");
        this.data = res;
        let erro = this.forgotCtrl.create({
          message:  this.data.token});
        erro.present();
        this.nav.setRoot(HomePage);
      },
      err => {
        console.log("Erro");
        let erro = this.forgotCtrl.create({
          message:  err.error + "Para logar use login: user@user.com senha:123abc" });
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
