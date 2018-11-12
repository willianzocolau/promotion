import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import {Validators, FormBuilder, FormGroup } from '@angular/forms';

/**
 * Generated class for the CreateAdPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-create-ad',
  templateUrl: 'create-ad.html',
})
export class CreateAdPage {

  public form : FormGroup;

  constructor(public navCtrl: NavController, public navParams: NavParams, 
    public formBuilder: FormBuilder) {
      this.form = this.formBuilder.group({
        name: ['', Validators.required],
        price: ['',Validators.required],
        image: ['',Validators.required],
        store_id: ['',Validators.required],
        state_id: ['',Validators.required]
      });
  }

  logForm(){
    console.log(this.form.value)
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad CreateAdPage');
  }

}
