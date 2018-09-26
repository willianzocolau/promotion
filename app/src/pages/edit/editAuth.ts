import {Component} from "@angular/core";
import {NavController, AlertController, MenuController, PopoverController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";
import {EditPage} from "../edit/edit";
import {HomePage} from "../home/home";
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
                private httpClient: HttpClient,
                private user: UserData,
                private server: ServerStrings
                ) {
        this.menu.swipeEnable(false);
        this.form = this.formBuilder.group({
            password: ['', Validators.required],
        });
    }
    confirm() {
        let headers = new HttpHeaders();
        let email: string = this.user.getEmail();
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
                this.nav.push(EditPage);
            },
            err => {
                console.log("Erro");
                let erro = this.alertCtrl.create({
                    message:  err.error });
                erro.present();
                this.nav.setRoot(HomePage);
            }
        );
    }

     // to go account page
    goToAccount() {
        this.nav.push(SettingsPage);
    }

    presentNotifications(myEvent) {
        console.log(myEvent);
        let popover = this.popoverCtrl.create(NotificationsPage);
        popover.present({
            ev: myEvent
        });
    }
}