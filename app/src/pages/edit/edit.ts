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
              private http: HTTP,
              private user: UserData,
              private server: ServerStrings
              ) {
        this.menu.swipeEnable(false);
        this.form = this.formBuilder.group({
            name: ['', Validators.required],
            nickname: ['', Validators.required],
            telephone: ['', Validators.required],
            cellphone: ['', Validators.required]/*,
            image_url: ['', Validators.required]*/
        });

        this.form.controls['name'].setValue(user.getName());
        this.form.controls['nickname'].setValue(user.getNickname());
        this.form.controls['cellphone'].setValue(user.getCellphone());
        this.form.controls['telephone'].setValue(user.getTelephone());
    }

    confirm(){
        let name: string = this.form.get('name').value;
        let nickname: string = this.form.get('nickname').value;
        let cellphone: string = this.form.get('cellphone').value;
        let telephone: string = this.form.get('telephone').value;
        let image_url: string = "";//this.form.get('image_url').value;

        if(name == "") name = null;
        if(nickname == "") nickname = null;
        if(cellphone == "") cellphone = null;
        if(telephone == "") telephone = null;
        if(image_url == "") image_url = null;
        
        let body = {
            "name": name,
            "nickname": nickname,
            "cellphone": cellphone,
            "telephone": telephone,
            "image_url": image_url
        };

        let headers = {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + this.user.getToken()
        };

        let endpoint = this.server.user();
        this.http.patch(endpoint, body, headers)
            .then(response => {
                this.alertCtrl.create({title: 'Perfil editado com sucesso!', buttons: ['Ok']}).present();
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
