import {Component} from "@angular/core";
import {NavController} from "ionic-angular";
import {LoginPage} from "../login/login";
import {HomePage} from "../home/home";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { AlertController } from 'ionic-angular';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {

  public form : FormGroup;

  constructor(public nav: NavController
    , public formBuilder: FormBuilder
    , private alertCtrl: AlertController
    , private httpClient: HttpClient ) {
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
    let cpf: string = this.form.get('cpf').value;
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;

    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));

    //let body: string = '{"name": "Teste","nickname": "Teste2123","cpf": "01234567890"}';
    let body: string = '{"name": "Teste","nickname": "Teste2123","cpf": "01234567890"}';

    const req = this.httpClient.post('http://178.128.186.9/api/auth/register/', body, {headers: headers}).subscribe(
        res => {
          console.log("Sucesso");
        },
        err => {
          console.log("Erro");
        }
      );
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