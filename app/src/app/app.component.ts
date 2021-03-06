import { Component, ViewChild } from "@angular/core";
import { Platform, Nav, Events, LoadingController } from "ionic-angular";

import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { Keyboard } from '@ionic-native/keyboard';
import { HTTP } from '@ionic-native/http';

import { HomePage } from "../pages/home/home";
import { LoginPage } from "../pages/login/login";
import { SearchPage } from "../pages/search/search";
import { EditAuthPage } from "../pages/edit/editAuth";
import { SettingsPage } from "../pages/settings/settings";
import { MyAdvertisingPage } from "../pages/my-advertising/my-advertising";
import { UserData } from "../providers/userData";
import { ServerStrings } from "../providers/serverStrings";
import { WishListPage } from "../pages/wishlist/wishlist";
import { SaleHistoryPage } from "../pages/saleHistory/saleHistory";
import { PurchaseHistoryPage } from "../pages/purchase-history/purchase-history";
import { SellerPage } from "../pages/seller/seller";

export interface MenuItem {
    title: string;
    function: any;
    icon: string;
}

@Component({
  templateUrl: 'app.html'
})

export class MyApp {
  @ViewChild(Nav) nav: Nav;

  rootPage: any = LoginPage;

  appMenuItems: Array<MenuItem>;
  appMenuSellerItems: Array<MenuItem>;
  nickname: string;
  credit: string;
  public userurl;
  type: number;

  constructor(
    public platform: Platform,
    public statusBar: StatusBar,
    public splashScreen: SplashScreen,
    public keyboard: Keyboard,
    public user: UserData,
    public events: Events,
    public http: HTTP,
    public server: ServerStrings,
    public loadingCtrl: LoadingController,
  ) {
    this.initializeApp();
    
    this.appMenuSellerItems = [
      { title: 'Home', function: () => { this.openPage(SellerPage) }, icon: 'home' },
      { title: 'Editar perfil', function: () => { this.openPage(EditAuthPage) }, icon: 'contact' },
      { title: 'Configurações', function: () => { this.pushPage(SettingsPage) }, icon: 'settings' },
      { title: 'Sair', function: () => { this.logout() }, icon: 'exit' },
    ];

    this.appMenuItems = [
      { title: 'Home', function: () => { this.openPage(HomePage) }, icon: 'home' },
      { title: 'Pesquisar', function: () => { this.openPage(SearchPage) }, icon: 'search' },
      { title: 'Meus anúncios', function: () => { this.openPage(MyAdvertisingPage) }, icon: 'pricetags' },
      { title: 'Lista de desejos', function: () => { this.openPage(WishListPage) }, icon: 'list' },
      { title: 'Histórico de vendas', function: () => { this.openPage(SaleHistoryPage) }, icon: 'time' },
      { title: 'Histórico de compras', function: () => { this.openPage(PurchaseHistoryPage) }, icon: 'time' },
      { title: 'Editar perfil', function: () => { this.openPage(EditAuthPage) }, icon: 'contact' },
      { title: 'Configurações', function: () => { this.pushPage(SettingsPage) }, icon: 'settings' },
      { title: 'Sair', function: () => { this.logout() }, icon: 'exit' },
    ];

    this.events.subscribe('user:updated', (userData) => {
      this.nickname = user.getNickname();
      this.credit = user.getCredit().toFixed(2);
      this.userurl = user.getImage_Url();
      this.type = user.getType();
    });
  }

  initializeApp() {
    this.platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.

      //*** Control Splash Screen
      this.splashScreen.hide();

      //*** Control Status Bar
      this.statusBar.styleDefault();
      this.statusBar.overlaysWebView(false);

      //*** Control Keyboard
      this.keyboard.disableScroll(true);

      this.http.setDataSerializer('json');
    });
  }

  openPage(page) {
    this.nav.setRoot(page);
  }

  pushPage(page) {
    this.nav.push(page);
  }

  executeFunction(func) {
    func();
  }

  logout() {
    let loading = this.loadingCtrl.create({ content: 'Saindo...' });
    loading.present();

    let endpoint: string = this.server.auth.logout();
    let headers = {
      'Authorization': 'Bearer ' + this.user.getToken(),
      'Content-type': 'application/json'
    };

    this.http.post(endpoint, {}, headers)
      .then(response => {
        this.user.setToken(null);
        loading.dismiss();
        this.openPage(LoginPage);
      })
      .catch(error => {
        this.user.setToken(null);
        loading.dismiss();
        this.openPage(LoginPage);
      });
  }
}
