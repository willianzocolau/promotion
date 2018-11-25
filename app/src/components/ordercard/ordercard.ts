import { Component, Input } from '@angular/core';
import { LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'ordercard',
  templateUrl: 'ordercard.html'
})
export class OrdercardComponent {

  @Input() endpoint;
  @Input() type;
  @Input() infinite = 0;
  public list = [];
  public lastid = 1;

  constructor(public http: HTTP,
    public user: UserData,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController) {}

  ngAfterViewInit(){
    console.log("ngInit");
    this.listing(this.lastid);
  }

  listing(lastid: number) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.promotionUserId(this.user.getId()) + "&after=" + this.lastid;
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(promotion => {
          let endpoint = this.server.promotionOrders(promotion.id);
          let hide = {
            has_item: false,
            icon: "arrow-dropright", 
            value: false
          }
          this.http.get(endpoint, {}, headers)
            .then(response1 => {
              let dados = JSON.parse(response1.data);
              let orders = [];
              if(dados.length > 0){
                hide.has_item = true;
              }
              dados.forEach(order => {
                let endpoint = this.server.userId(order.approved_by);
                this.http.get(endpoint, {}, headers)
                .then(response2 => {
                  let approved = JSON.parse(response2.data);
                  orders.push({order, approved});
                })
                .catch(exception => {
                  let dados = JSON.parse(exception.error);
                  let msg = this.alertCtrl.create({
                    message: "Erro: " + dados.error
                  });
                  msg.present();    
                });
              });
              this.list.push({promotion, orders, hide});
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
        console.log(this.list);
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
  hide(item){
    item.hide.value = !item.hide.value;
    if(item.hide.value){
      item.hide.icon = "arrow-dropdown";
    }
    else{
      item.hide.icon = "arrow-dropright";
    }
  }
}
