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
  @Input() infinite = 0;
  public list = [];
  public list1 = [];
  public list2 = []; 
  public list3 = [];
  public lastid = 1;

  constructor(public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController,
    public modalCtrl: ModalController) {}

  openPage(item: any){
    this.list.push( this.list2 );
    let modal = this.modalCtrl.create(AdvertisingPage, item);
    modal.onDidDismiss((data) => {
      console.log(data);
    });
    modal.present();
  }

  ngAfterViewInit(){
    if(this.type == "adcard-user"){
      this.userListing(this.lastid);
    }
  }

  ngOnChanges(){
    if(this.type == "adcard" && this.endpoint != undefined){
      this.listing(this.endpoint, this.lastid);
    }
    if(this.infinite > 0){
      this.userListing(this.lastid);
    }
  }

  copyList(list: any[]){
    let ret = new Array;
    list.forEach(element => {
      ret.push(element);
    });
    return ret;
  }

  userListing(lastid: number){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint: string = this.server.promotionUserId(this.user.getId())+"&after="+this.lastid;
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list3 = this.copyList(this.list2);
    this.list2 = this.copyList(this.list1);
    this.list1 = [];
    this.list = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(promotion => {
          this.list.push({promotion, user: this.user.get()});  
        });
        if(this.list.length > 0){
          this.lastid = this.list[this.list.length-1].promotion.id;
          console.log("lastid="+this.lastid);
        }
        this.list1 = this.copyList(this.list);
        this.list2.forEach(element => {
          this.list3.push(element);
        });
        this.list1.forEach(element => {
          this.list3.push(element);
        });
        this.list = this.copyList(this.list3);
        loading.dismiss();
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        console.log("Erro: " + dados.error);
        loading.dismiss();
      });
  }

  listing(endpoint: string, lastid: number) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    //let endpoint: string = this.server.promotionSearch(input);
    endpoint = endpoint + "?after=" + this.lastid;
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
        if(this.list.length > 0){
          this.lastid = this.list[this.list.length-1].promotion.id;
          console.log("lastid="+this.lastid);
        }
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
