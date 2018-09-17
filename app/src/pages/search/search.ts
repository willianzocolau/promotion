import {Component} from "@angular/core";
import {NavController, NavParams} from "ionic-angular";
import {Storage} from '@ionic/storage';

// import {SearchCarsPage} from "../search-cars/search-cars";

@Component({
  selector: 'page-search',
  templateUrl: 'search.html'
})

export class SearchPage {
  public fromto: any;
  // places
  public promotions = {
    basket: [
      {
        id: 1,
        name: "Imagem 1",
        imgurl:"https://brandmark.io/logo-rank/random/bp.png", 
        price: "10,99"
      },
      {
        id: 2,
        name: "Imagem 2",
        imgurl:"https://brandmark.io/logo-rank/random/bp.png", 
        price: "20,99"
      },
    ],
  };

  constructor(private storage: Storage, public nav: NavController, public navParams: NavParams) {
    this.fromto = this.navParams.data;
  }

  // search by item
  searchBy(item) {
    if (this.fromto === 'from') {
      this.storage.set('pickup', item.name);
    }

    if (this.fromto === 'to') {
      this.storage.set('dropOff', item.name);
    }
    // this.nav.push(SearchCarsPage);
    this.nav.pop();
  }
}
