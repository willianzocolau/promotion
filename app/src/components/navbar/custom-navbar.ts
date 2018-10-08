import { Component } from '@angular/core';
import { NavController, PopoverController } from "ionic-angular";
import { NotificationsPage } from "../../pages/notifications/notifications";

/* Exemplos:
 * https://stackoverflow.com/questions/43507800/custom-component-in-ionic-v3
 * https://stackoverflow.com/questions/41566079/ionic2-same-header-for-all-pages
*/

@Component({
  selector: 'custom-navbar',
  templateUrl: 'custom-navbar.html'
})
export class CustomNavComponent {
  constructor(public navCtrl: NavController,
              public popoverCtrl: PopoverController) { }

  presentNotifications(myEvent) {
    console.log(myEvent);
    let popover = this.popoverCtrl.create(NotificationsPage);
    popover.present({
      ev: myEvent
    });
  }
}
