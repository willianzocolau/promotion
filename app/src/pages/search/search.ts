import { Component } from "@angular/core";
import { NavController, NavParams, AlertController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HTTP } from '@ionic-native/http';
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
    private http: HTTP,
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
    let input: string = this.form.get('input').value;

    if (!input) {
      let msg = this.alertCtrl.create({
        title: "Inválido",
        message: "Por favor, digite algo na caixa de texto"
      });
      msg.present();
    }
    else {
      let endpoint: string = this.server.promotionSearch(input);
      let headers = {
        'Authorization': 'Bearer ' + this.user.getToken()
      };
      this.promotions = [];

      this.http.get(endpoint, {}, headers)
        .then(response => {
          console.log(response.data);
          var dados = JSON.parse(response.data);
          dados.forEach(element => {
            this.promotions.push({ id: element.id, name: element.name, image_url: element.image_url, price: element.price });
          });
        })
        .catch(exception => {
          let msg = this.alertCtrl.create({
            message: "erro:" + exception.error
          });
          msg.present();
        });
    }
  }
}
