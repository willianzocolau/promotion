import { Component, Input } from '@angular/core';
import { ModalController, LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { AdvertisingPage } from '../../pages/advertising/advertising';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'adcard',
  templateUrl: 'adcard.html'
})
export class AdcardComponent {
  @Input() endpoint;
  @Input() type;
  public list = [];

  constructor(public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController,
    public modalCtrl: ModalController) {}

  openPage(item: any){
    let modal = this.modalCtrl.create(AdvertisingPage, item);
    modal.onDidDismiss((data) => {
      console.log(data);
    });
    modal.present();
  }

  ngAfterViewInit(){
    console.log("ngInit");
    if(this.type == "adcard-user"){
      this.userListing();
    }
  }

  ngOnChanges(){
    if(this.type == "adcard" && this.endpoint != undefined){
      this.listing(this.endpoint);
    }
    console.log("ngChange");
  }

  userListing(){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.promotionUserId(this.user.getId());
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(promotion => {
          this.list.push({promotion, user: this.user.get()});  
        });
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        console.log("Erro: " + dados.error);
        loading.dismiss();
      });
  }

  listing(endpoint: string) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    //let endpoint: string = this.server.promotionSearch(input);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(promotion => {
          let endpoint = this.server.userId(promotion.user_id);
          this.http.get(endpoint, {}, headers)
            .then(response => {
              let user = JSON.parse(response.data);
              this.list.push({promotion, user});  
            })
            .catch(exception => {
              let dados = JSON.parse(exception.error);
              let msg = this.alertCtrl.create({
                message: "Erro: " + dados.error
              });
              msg.present();
            });
        });
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        msg.present();
      });
  }
}
