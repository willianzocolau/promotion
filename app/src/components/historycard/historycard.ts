import { Component, Input } from '@angular/core';
import { LoadingController, AlertController, NavController } from 'ionic-angular';
import { HTTP } from '@ionic-native/http';

import { SaleHistoryPage } from '../../pages/saleHistory/saleHistory';

import { UserData } from '../../providers/userData';
import { ServerStrings } from '../../providers/serverStrings';
import { OrdercardComponent } from '../ordercard/ordercard';

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
    public alertCtrl: AlertController,
    public nav: NavController) {}

  ngAfterViewInit(){
    this.listing(this.lastid);
  }

  commented(votes, order){
    let result = false;
    votes.forEach(element => {
      console.log(element.order_id + " " + order.id);
      if(element.order_id == order.id){
        result = true;
      }
    });
    return result;
  }

  listing(lastid: number) {
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();
    let endpoint = this.server.order("", 0) + "?user_id=" +this.user.getId();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
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
              let comment = this.commented(promotion.votes, order);
              this.list.push({order, promotion, approved, comment});  
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
        loading.dismiss();
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
  vote(item, is_positive: boolean){
    let msg = this.alertCtrl.create({
      title: 'Comentário',
      message: "Deixe seu comentário",
      inputs: [
        {
          name: 'comment',
          placeholder: '...',
        },
      ],
      buttons: [
        {
          text: 'Cancelar',
          handler: data => {
            console.log('Cancel clicked');
          }
        },
        {
          text: 'Save',
          handler: data => {
            this.voteRequest(item, is_positive, data.comment);
          }
        }
      ]
    });
    msg.present();
  }

  voteRequest(item, is_positive: boolean, comment: string){
    let loading = this.loadingCtrl.create({ content: 'Carregando...' });
    loading.present();

    let endpoint: string = this.server.order("vote", item.id);
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };
    let body = {
      "is_positive": is_positive,
      "comment": comment
    }
    this.http.post(endpoint, body, headers)
      .then(response => {
        loading.dismiss();
        this.nav.setRoot(SaleHistoryPage);
      })
      .catch(exception => {
        let dados = JSON.parse(exception.error);
        let msg = this.alertCtrl.create({message: "Erro: " + dados});
        loading.dismiss();
        msg.present();
        console.log(exception);
      });
      loading.dismiss();
  }
}
