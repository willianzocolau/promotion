import {Component} from "@angular/core";
import {NavController, NavParams, AlertController} from "ionic-angular";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Token } from '../../providers/token';
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
              public token: Token,
              public server: ServerStrings) {
    this.form = this.formBuilder.group({
      input: ['', Validators.maxLength(50)],
    });
    let msg = this.alertCtrl.create({
      message:  this.token.getToken()});
    msg.present();
  }

  // search by item
  searchBy(item) {
  }

  pesquisa(){
    let headers = new HttpHeaders();
    let input: string = this.form.get('input').value;
    headers = headers.set('Content-Type', 'application/json');    
    headers = headers.set("Authorization", "Bearer " + this.token.getToken());
    let url = this.server.api.promotion.search + input;
    while(this.promotions.length != 0){
      this.promotions.pop();
    }
    const req = this.httpClient.get(url, {headers: headers}).subscribe(
      res => {
        let data : any[];
        data = res as any[];
        data.forEach(element => {
          this.promotions.push({id:element.id, name:element.name, image_url:element.image_url, price:element.price});
        });
      },
      err => {
        let msg = this.alertCtrl.create({
          message: "erro:" + err.error});
        msg.present();
      }
    );
  }
}
