import { Injectable } from '@angular/core';

@Injectable()
export class ServerStrings {
  public static url: string = 'http://178.128.186.9/api/';
  // authController
  get auth() {
    return new Auth();
  }

  // userController
  user() {
    return ServerStrings.url + "user/";
  }
  userSearch(name: string) {
    return this.user() + "search/" + name;
  }
  userId(id: number) {
    return this.user() + id;
  }
  userWishlist(){
    return this.user() + "wishlist/"
  }
  userWishlistId(id: number){
    return this.userWishlist() + id;
  }
  userMatchs(){
    return this.user() + "matchs/"
  }
  userMatchsId(id: number){
    return this.userMatchs() + id;
  }
  // promotionController
  promotion() {
    return ServerStrings.url + "promotion/";
  }
  promotionSearch(name: string) {
    return this.promotion() + "?name=" + name;
  }
  promotionId(id: number){
    return this.promotion() + id;
  }
  promotionUserId(id: number) {
    return this.promotion() + "?user_id=" + id;
  }
  promotionRegister() {
    return this.promotion() + "register/";
  }
  promotionOrders(id: number){
    return this.promotion() + id +"/orders";
  }
  // orderController
  order(path: string, id: number) {
    if (path == "")
      return ServerStrings.url + "order/";
    else if (path == "aprove")
      return ServerStrings.url + "order/" + id.toString() + "/approve";
    else if (path == "")
      return ServerStrings.url + "order/" + id.toString() + "/disapprove";
    else if (path == "vote")
      return ServerStrings.url + "order/" + id.toString() + "/vote"
    return "";
  }
  // stateController
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
