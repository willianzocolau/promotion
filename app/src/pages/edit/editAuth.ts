import {Component} from "@angular/core";
import {NavController, AlertController, MenuController, PopoverController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import {NotificationsPage} from "../notifications/notifications";
import {SettingsPage} from "../settings/settings";
import {EditPage} from "../edit/edit";

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
                /*private httpClient: HttpClient,
                private token: Token,
                private server: ServerStrings*/
                ) {
        this.menu.swipeEnable(false);
        this.form = this.formBuilder.group({
            password: ['', Validators.required],
        });
    }
    confirm(){ 
        this.nav.push(EditPage);
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