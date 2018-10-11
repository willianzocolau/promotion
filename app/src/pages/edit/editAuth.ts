import { Component } from "@angular/core";
import { NavController, AlertController, MenuController, PopoverController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';

import { NotificationsPage } from "../notifications/notifications";
import { EditPage } from "../edit/edit";
import { UserData } from "../../providers/userData";
import { ServerStrings } from "../../providers/serverStrings";

@Component({
    selector: 'page-editAuth',
    templateUrl: 'editAuth.html'
})
export class EditAuthPage {
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
        });
    }
    confirm() {
      this.user.getEmailAsync().then((email) => {
        let password = this.form.get('password').value;
        let headers = {
          'Content-Type': 'application/json',
          'Authorization': 'Basic ' + btoa(email + ":" + password)
        }
        let endpoint = this.server.auth.login();
        this.http.post(endpoint, "", headers)
          .then( response => {
            let dados = JSON.parse(response.data);
            this.data = dados;
            this.user.update(dados);
            this.nav.push(EditPage);
          })
          .catch( exception => {
            console.log("Erro");
            let dados = JSON.parse(exception.error);
            let erro = this.alertCtrl.create({
              message: "Erro: " + dados.error
            });
            erro.present();
          });
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
