import {Component} from "@angular/core";
import {NavController, NavParams, AlertController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-search',
  templateUrl: 'search.html'
})

export class SearchPage {
  public form: FormGroup;

  public promotions = [];

  constructor(public formBuilder: FormBuilder,
    private httpClient: HttpClient,
    public alertCtrl: AlertController,
    public nav: NavController,
    public navParams: NavParams,
    public user: UserData,
    public server: ServerStrings) {
    this.form = this.formBuilder.group({
      input: ['', Validators.maxLength(50)],
    });
  }

  // search by item
  searchBy(item) {
  }

  pesquisa() {
    let headers = new HttpHeaders();
    let input: string = this.form.get('input').value;

    if (!input) {
      let msg = this.alertCtrl.create({
        title: "Inválido",
        message: "Por favor, digite algo na caixa de texto"
      });
      msg.present();
    }
    else {
      headers = headers.set('Content-Type', 'application/json');
      headers = headers.set("Authorization", "Bearer " + this.user.getToken());
      let url = this.server.promotionSearch(input);
      this.promotions = [];
      const req = this.httpClient.get(url, { headers: headers }).subscribe(
        res => {
          /*let data: any[];
          data = res as any[];
          data.forEach(element => {
            this.promotions.push({ id: element.id, name: element.name, image_url: element.image_url, price: element.price });
          });*/
          console.log(res);
        },
        err => {
          let msg = this.alertCtrl.create({
            message: "erro:" + err.error
          });
          msg.present();
        }
      );
    }
  }
}
