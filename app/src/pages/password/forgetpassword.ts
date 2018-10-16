import {Component} from "@angular/core";
import { NavController, AlertController, LoadingController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';
import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";
import { LoginPage } from "../login/login";

@Component({
  selector: 'page-forgetpassword',
  templateUrl: 'forgetpassword.html'
})

export class ForgetPasswordPage {

  public form : FormGroup;

  constructor(public nav: NavController,
    public formBuilder: FormBuilder,
    public http: HTTP,
    public alertCtrl: AlertController,
    public loadingCtrl: LoadingController,
    public server: ServerStrings,
    public user: UserData) {
      this.form = this.formBuilder.group({
        email: ['', Validators.email],
        code: ['', Validators.required],
        password: ['', Validators.required],
        confirm_password: ['', Validators.required]
      });

      this.user.getEmailAsync().then((email) => {
        if (email != null)
          this.form.controls['email'].setValue(email.toLowerCase());
      });  
  }

  reset() {
    let email: string = this.form.get('email').value;
    let password: string = this.form.get('password').value;
    let c_password: string = this.form.get('confirm_password').value;
    let code: string = this.form.get('code').value;

    let loading = this.loadingCtrl.create({ content: 'Solicitando...' });
    loading.present();

    if(password != c_password){
      this.alertCtrl.create({title: 'Senhas não conferem!',buttons: ['Ok']}).present();
      return;
    }

    let endpoint: string = this.server.auth.change();

    let headers = {
      'Content-type': 'application/json'
    };

    let body =
    {
      "new_password": password,
      "email": email,
      "reset_code": code
    };

    this.http.post(endpoint, body, headers)
      .then(response => {
        loading.dismiss();
        this.nav.setRoot(LoginPage);
      })
      .catch(exception => {
        this.alertCtrl.create({ title: 'Erro: ' + JSON.parse(exception.error).error, buttons: ['Ok'] }).present();
        console.log("Erro");
        loading.dismiss();
      });
  }

}