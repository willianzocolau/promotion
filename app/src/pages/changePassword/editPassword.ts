import { Component } from "@angular/core";
import { NavController, AlertController, MenuController, PopoverController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';

import { NotificationsPage } from "../notifications/notifications";
import { HomePage } from "../home/home";
import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";

@Component({
  selector: 'page-edit',
  templateUrl: 'editPassword.html'
})

export class ChangePasswordPage {
    public form : FormGroup;
    public data : any;
  
    constructor(public nav: NavController, 
              public formBuilder: FormBuilder, 
              public alertCtrl: AlertController, 
              public menu: MenuController,
              public popoverCtrl: PopoverController,  
              private http: HTTP,
              private user: UserData,
              private server: ServerStrings
              ) {
        this.menu.swipeEnable(false);
        this.form = this.formBuilder.group({
            password: ['', Validators.required],
            new_password: ['', Validators.required],
            confirm_password: ['', Validators.required]
        });
    }
    confirm(){
        let password: string = this.form.get('password').value;
        let new_password: string = this.form.get('new_password').value;
        let c_password: string = this.form.get('confirm_password').value;

        if(new_password != c_password){
            this.alertCtrl.create({title: 'Senhas não conferem!',buttons: ['Ok']}).present();
            return;
        }

        let body = {
            "new_password": new_password,
            "old_password": password,
            "email": this.user.getEmail()
        };

        let headers = {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + this.user.getToken()
        };

        let endpoint = this.server.auth.change();

        console.log(endpoint);

        this.http.patch(endpoint, body, headers)
            .then(response => {
                this.alertCtrl.create({title: 'Senha alterada com sucesso!', buttons: ['Ok']}).present();
                this.nav.setRoot(HomePage);
            })
            .catch(exception => {
              this.alertCtrl.create({ title: "Erro:" + JSON.parse(exception.error).error, buttons: ['Ok']}).present();
              console.log(exception);
            });
    }

    presentNotifications(myEvent) {
        console.log(myEvent);
        let popover = this.popoverCtrl.create(NotificationsPage);
        popover.present({
            ev: myEvent
        });
    }
}
