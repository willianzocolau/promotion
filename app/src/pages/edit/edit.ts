import {Component} from "@angular/core";
import {NavController, AlertController, MenuController, PopoverController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";
import { HomePage } from "../home/home";
import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";

@Component({
  selector: 'page-edit',
  templateUrl: 'edit.html'
})

export class EditPage {
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
            name: ['', Validators.required],
            nickname: ['', Validators.required],
            cpf: ['', Validators.required],
            password: ['', Validators.required],
            confirm_password: ['', Validators.required],
            telephone: ['', Validators.required],
            cellphone: ['', Validators.required],
            image_url: ['', Validators.required]
        });
    }
    confirm(){
        let name: string = this.form.get('name').value;
        let nickname: string = this.form.get('nickname').value;
        let cpf: string = this.form.get('cpf').value.replace('.', '');
        cpf = cpf.replace('.', '');
        cpf = cpf.replace('-', '');
        let password: string = this.form.get('password').value;
        let c_password: string = this.form.get('confirm_password').value;
        let cellphone: string = this.form.get('cellphone').value;
        let telephone: string = this.form.get('telephone').value;
        let image_url: string = this.form.get('image_url').value;
        if(password != c_password){
            this.alertCtrl.create({title: 'Senhas não conferem!',buttons: ['Ok']}).present();
            return;
        }

        let headers = new HttpHeaders();
        headers = headers.set('Content-Type', 'application/json');
        headers = headers.set("Authorization", "Bearer " + this.user.getToken());
        if(name == "") name = null;
        if(nickname == "") nickname = null;
        if(cpf == "") cpf = null;
        if(cellphone == "") cellphone = null;
        if(telephone == "") telephone = null;
        if(image_url == "") image_url = null;
        var body = 
        {
            "name": name,
            "nickname": nickname,
            "cpf": cpf,
            "cellphone": cellphone,
            "telephone": telephone,
            "image_url": image_url
        };
        let url = this.server.userEdit();
        this.httpClient.patch(url, body, {headers: headers, responseType: "text"})
        .subscribe(
            res => {
                this.nav.setRoot(HomePage);
                this.alertCtrl.create({title: 'Perfil editado com sucesso!',buttons: ['Ok']}).present();
                console.log("Sucesso");
            },
            err => {
                this.alertCtrl.create({title: err.error,buttons: ['Ok']}).present();
                console.log(err);
            }
        );
    }

    presentNotifications(myEvent) {
        console.log(myEvent);
        let popover = this.popoverCtrl.create(NotificationsPage);
        popover.present({
            ev: myEvent
        });
    }
}
