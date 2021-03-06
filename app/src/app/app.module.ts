import { NgModule } from "@angular/core";
import { IonicApp, IonicModule } from "ionic-angular";
import { BrowserModule } from '@angular/platform-browser';
import { IonicStorageModule } from '@ionic/storage';
import { HTTP } from '@ionic-native/http';
import { Camera } from '@ionic-native/camera';

import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { Keyboard } from '@ionic-native/keyboard';
import { UserData } from '../providers/userData';
import { ServerStrings } from '../providers/serverStrings';

import { MyApp } from "./app.component";

import { CustomNavComponent } from "../components/navbar/custom-navbar";
import { AdcardComponent } from "../components/adcard/adcard";
import { ListComponent } from "../components/list/list";
import { WishcardComponent } from "../components/wishcard/wishcard";
import { OrdercardComponent } from "../components/ordercard/ordercard";
import { HistorycardComponent } from "../components/historycard/historycard";
import { ImguploadComponent } from "../components/imgupload/imgupload";

import { SettingsPage } from "../pages/settings/settings";
import { HomePage } from "../pages/home/home";
import { LoginPage } from "../pages/login/login";
import { NotificationsPage } from "../pages/notifications/notifications";
import { RegisterPage } from "../pages/register/register";
import { SearchPage } from "../pages/search/search";
import { EditPage } from "../pages/edit/edit";
import { EditAuthPage } from "../pages/edit/editAuth";
import { MyAdvertisingPage } from "../pages/my-advertising/my-advertising";
import { ForgetPasswordPage } from "../pages/password/forgetpassword";
import { WishListPage } from "../pages/wishlist/wishlist";
import { AdvertisingPage } from "../pages/advertising/advertising";
import { SaleHistoryPage } from "../pages/saleHistory/saleHistory";
import { CreateAdPage } from "../pages/create-ad/create-ad";
import { PurchaseHistoryPage } from "../pages/purchase-history/purchase-history";

import { BrMaskerModule } from 'brmasker-ionic-3';
import { SellerPage } from "../pages/seller/seller";
// import services
// end import services
// end import services

// import pages
// end import pages

@NgModule({
  declarations: [
    MyApp,
    CustomNavComponent,
    AdcardComponent,
    WishcardComponent,
    ListComponent,
    OrdercardComponent,
    HistorycardComponent,
    ImguploadComponent,
    SettingsPage,
    HomePage,
    LoginPage,
    NotificationsPage,
    RegisterPage,
    SearchPage,
    EditPage,
    EditAuthPage,
    MyAdvertisingPage,
    ForgetPasswordPage,
    WishListPage,
    AdvertisingPage,
    SaleHistoryPage,
    CreateAdPage,
    PurchaseHistoryPage,
    SellerPage,
  ],
  imports: [
    BrowserModule,
    IonicModule.forRoot(MyApp, {
      scrollPadding: false,
      scrollAssist: true,
      autoFocusAssist: false
    }),
    IonicStorageModule.forRoot({
      name: '__ionic3_start_theme',
        driverOrder: ['indexeddb', 'sqlite', 'websql']
    }),
    BrMaskerModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    CustomNavComponent,
    AdcardComponent,
    WishcardComponent,
    ListComponent,
    OrdercardComponent,
    HistorycardComponent,
    ImguploadComponent,
    SettingsPage,
    HomePage,
    LoginPage,
    NotificationsPage,
    RegisterPage,
    SearchPage,
    EditPage,
    EditAuthPage,
    MyAdvertisingPage,
    ForgetPasswordPage,
    WishListPage,
    AdvertisingPage,
    SaleHistoryPage,
    CreateAdPage,
    PurchaseHistoryPage,
    SellerPage,
  ],
  providers: [
    StatusBar,
    SplashScreen,
    Keyboard,
    UserData,
    ServerStrings,
    HTTP,
    Camera,
  ]
})

export class AppModule {}
