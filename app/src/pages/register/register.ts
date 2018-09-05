import {Component} from "@angular/core";
import {NavController} from "ionic-angular";
import {LoginPage} from "../login/login";
import {HomePage} from "../home/home";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { AlertController } from 'ionic-angular';
import { Http , Headers, Response } from '@angular/http';

@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {

  public form : FormGroup;

  constructor(public nav: NavController, public formBuilder: FormBuilder, private alertCtrl: AlertController, private http: Http) {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      nickname: ['', Validators.required],
      cpf: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', Validators.required],
      confirm_password: ['', Validators.required]
    });
  }

  // register and go to home page
  register() {
    const body = JSON.stringify({username: this.form.get('email').value, password: this.form.get('password').value});

    let headers = new Headers();
    headers = headers.append("Authorization", "Basic " + btoa("username:password"));
    headers = headers.append("Content-Type", "application/x-www-form-urlencoded");

    this.http.post('http://178.128.186.9/api/auth/register/',body, {headers: headers}).subscribe(response => {
        let alert = this.alertCtrl.create({title: 'Sucesso', subTitle: '', buttons: ['Ok']});
        alert.present();
    }, err => {
        let alert = this.alertCtrl.create({title: 'Falha', subTitle: '', buttons: ['Ok']});
        alert.present();
    });
  }

  // go to login page
  login() {
    this.nav.setRoot(LoginPage);
  }

  doAlert() {
    let alert = this.alertCtrl.create({
      title: 'New Friend!',
      subTitle: this.form.get('password').value,
      buttons: ['Ok']
    });

    alert.present();
  }
}
