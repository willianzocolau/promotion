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
    console.log("Teste");

    let username: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;

    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Accept', 'application/json');

    headers = headers.set('Access-Control-Allow-Origin', 'http://localhost:8100');
    headers = headers.set('Access-Control-Allow-Credentials', 'true');
    
    headers = headers.set("Authorization", "Basic " + btoa(username + ":" + password));

    let body: string = '{"name": "Teste","nickname": "Teste2123","cpf": "01234567890"}';

    console.log(btoa(username + ":" + password));
    console.log(body);

    const req = this.httpClient.post('http://178.128.186.9/api/auth/register/', body, {headers: headers}).subscribe(
        res => {
          console.log("Sucesso");
        },
        err => {
          console.log("Erro");
        }
      );
      console.log(req);
  }

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