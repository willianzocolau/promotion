import { Injectable } from '@angular/core';

@Injectable()
export class ServerStrings{
    public url: string = 'http://178.128.186.9/';
    // authControler
    auth(path: string){
        if(path == "register")
            return this.url + "api/auth/register/";
        else if(path == "login")
            return this.url + "api/auth/login/";
        else if(path == "extend")
            return this.url + "api/auth/extend/";
        else if(path == "logout")
            return this.url + "api/auth/logout/";
        return "";
    }
    // userControler
    user(){
        return this.url + "api/user/";
    }
    userSearch(name: string){
        return this.user() + "search/" + name;
    }
    userId(id: number){
        return this.user() + id;
    }
    userEdit(){
        return this.user() + "edit/";
    }
    // promotionControler
    promotion(){
        return this.url + "api/promotion/";
    }
    promotionSearch(name: string){
        return this.promotion() + "?name=" + name;
    }
    promotionRegister(){
        return this.promotion() + "register/";
    }
    // orderControler
    order(path: string, id: number){
        if(path == "")
            return this.url + "api/order/";
        else if(path == "aprove")
            return this.url + "api/order/" + id.toString() + "/aprove";
        else if(path == "")
            return this.url + "api/order/" + id.toString() + "/disaprove";
        return "";
    }
    // stateControler
    state(){
        return this.url + "api/state/";
    }
    constructor(){
        console.log("ServerStrings provider");
    }
}
