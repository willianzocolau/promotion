import { Injectable } from '@angular/core';

@Injectable()
export class ServerStrings {
  public static url: string = 'http://178.128.186.9/api/';
  // authControler
  get auth() {
    return new Auth();
  }

  // userControler
  user() {
    return ServerStrings.url + "user/";
  }
  userSearch(name: string) {
    return this.user() + "search/" + name;
  }
  userId(id: number) {
    return this.user() + id;
  }
  // promotionControler
  promotion() {
    return ServerStrings.url + "promotion/";
  }
  promotionSearch(name: string) {
    return this.promotion() + "?name=" + name;
  }
  promotionUserId(id: number) {
    return this.promotion() + "?user_id=" + id;
  }
  promotionRegister() {
    return this.promotion() + "register/";
  }
  // orderControler
  order(path: string, id: number) {
    if (path == "")
      return ServerStrings.url + "order/";
    else if (path == "aprove")
      return ServerStrings.url + "order/" + id.toString() + "/aprove";
    else if (path == "")
      return ServerStrings.url + "order/" + id.toString() + "/disaprove";
    return "";
  }
  // stateControler
  state() {
    return ServerStrings.url + "state/";
  }
  constructor() {
    console.log("ServerStrings provider");
  }
}

export class Auth {
  register() {
    return ServerStrings.url + "auth/register/";
  }
  login() {
    return ServerStrings.url + "auth/login/";
  }
  extend() {
    return ServerStrings.url + "auth/extend/";
  }
  logout() {
    return ServerStrings.url + "auth/logout/";
  }
  reset() {
    return ServerStrings.url + "auth/reset/";
  }
  change() {
    return ServerStrings.url + "auth/change/";
  }
}
