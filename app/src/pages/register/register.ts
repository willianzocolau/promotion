import {Component} from "@angular/core";
import {NavController} from "ionic-angular";
import {LoginPage} from "../login/login";
import {HomePage} from "../home/home";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { AlertController } from 'ionic-angular';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ServerStrings } from "../../providers/serverStrings";

@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {

  public form : FormGroup;

  constructor(public nav: NavController
    , public formBuilder: FormBuilder
    , private alertCtrl: AlertController
    , private httpClient: HttpClient
    , private server: ServerStrings ) {
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
    let url: string = this.server.auth("register");

    console.log(cpf);

    if(password != c_password){
      this.alertCtrl.create({title: 'Senhas não conferem!',buttons: ['Ok']}).present();
      return;
    }

    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Basic " + btoa(email + ":" + password));

    var body = 
    {
        "name": name,
        "nickname": nickname,
        "cpf": cpf,
        "email": email,
        "password": password
    };

    this.httpClient.post(url, body, {headers: headers}).subscribe(
        res => {
          this.nav.setRoot(LoginPage);
          this.alertCtrl.create({title: 'Cadastro criado com sucesso!',buttons: ['Ok']}).present();
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