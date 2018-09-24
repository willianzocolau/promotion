import {Component} from "@angular/core";
import {NavController, AlertController, MenuController, PopoverController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";

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
                /*private httpClient: HttpClient,
                private token: Token,
                private server: ServerStrings*/
                ) {
        this.menu.swipeEnable(false);
        this.form = this.formBuilder.group({
            name: ['', Validators.required],
            nickname: ['', Validators.required],
            cpf: ['', Validators.required],
            email: ['', Validators.email],
            password: ['', Validators.required],
            confirm_password: ['', Validators.required]
          });
    }
    confirm(){

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