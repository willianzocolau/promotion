import { Component } from "@angular/core";
import { NavController, NavParams, AlertController } from "ionic-angular";
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'page-search',
  templateUrl: 'search.html'
})

export class SearchPage {
  
  public form: FormGroup;
  public endpoint = undefined;

  constructor(public formBuilder: FormBuilder,
              public alertCtrl: AlertController,
              public nav: NavController,
              public navParams: NavParams,
              public server: ServerStrings) {
    this.form = this.formBuilder.group({
      input: ['', Validators.maxLength(50)],
    });
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
      this.endpoint = this.server.promotionSearch(input);
    }
  }
}
