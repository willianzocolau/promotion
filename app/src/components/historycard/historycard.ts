import { Component, Input } from '@angular/core';
import { LoadingController, AlertController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';

@Component({
  selector: 'historycard',
  templateUrl: 'historycard.html'
})
export class HistorycardComponent {

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
    this.listing(this.lastid);
  }

  listing(lastid: number) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("", 0) + "?user_id=" +this.user.getId();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken()
    };
    this.list = [];

    this.http.get(endpoint, {}, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        dados.forEach(order => {
          let endpoint = this.server.promotionId(order.promotion_id);
          this.http.get(endpoint, {}, headers)
          .then(response2 => {
            let promotion = JSON.parse(response2.data);
            let endpoint = this.server.userId(order.approved_by);
            this.http.get(endpoint, {}, headers)
            .then(response3 => {
              let approved = JSON.parse(response3.data);
              this.list.push({order, promotion, approved});  
            })
            .catch(exception3 =>{
              let dados = JSON.parse(exception3.error);
              let msg = this.alertCtrl.create({
                message: "Erro: " + dados.error
              });
              loading.dismiss();
              msg.present();
            });
          })
          .catch(exception2 => {
            let dados = JSON.parse(exception2.error);
            let msg = this.alertCtrl.create({
              message: "Erro: " + dados.error
            });
            loading.dismiss();
            msg.present();
          });
        });
      })      
      .catch(exception1 => {
        let dados = JSON.parse(exception1.error);
        let msg = this.alertCtrl.create({
          message: "Erro: " + dados.error
        });
        loading.dismiss();
        msg.present();
      });
  }
}
