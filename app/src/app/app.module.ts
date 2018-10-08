import { NgModule } from "@angular/core";
import { IonicApp, IonicModule } from "ionic-angular";
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { IonicStorageModule } from '@ionic/storage';
import { HTTP } from '@ionic-native/http';

import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { Keyboard } from '@ionic-native/keyboard';
import { UserData } from '../providers/userData';
import { ServerStrings } from '../providers/serverStrings';

import { MyApp } from "./app.component";

import { SettingsPage } from "../pages/settings/settings";
import { HomePage } from "../pages/home/home";
import { LoginPage } from "../pages/login/login";
import { NotificationsPage } from "../pages/notifications/notifications";
import { RegisterPage } from "../pages/register/register";
import { SearchPage } from "../pages/search/search";
import { EditPage } from "../pages/edit/edit";
import { EditAuthPage } from "../pages/edit/editAuth";

import { BrMaskerModule } from 'brmasker-ionic-3';
// import services
// end import services
// end import services

// import pages
// end import pages

@NgModule({
  declarations: [
    MyApp,
    SettingsPage,
    HomePage,
    LoginPage,
    NotificationsPage,
    RegisterPage,
    SearchPage,
    EditPage,
    EditAuthPage,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
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
    SettingsPage,
    HomePage,
    LoginPage,
    NotificationsPage,
    RegisterPage,
    SearchPage,
    EditPage,
    EditAuthPage,
  ],
  providers: [
    StatusBar,
    SplashScreen,
    Keyboard,
    UserData,
    ServerStrings,
    HTTP
  ]
})

export class AppModule {}
