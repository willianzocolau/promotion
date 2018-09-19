import { Injectable } from '@angular/core';

@Injectable()
export class ServerStrings{
    public url: string = 'http://178.128.186.9/';
    public api = {
        auth:{
            register: this.url + "api/auth/register/",
            login: this.url + "api/auth/login/",
            extend: this.url + "api/auth/extend/",
            logout: this.url + "api/auth/logout/"
        },
        user:{   
            self: this.url + "api/user/",
            search: this.url + "api/user/search/",
        },
        promotion:{
            self: this.url + "api/promotion/",
            search: this.url + "api/promotion/search/",
            register: this.url + "api/promotion/register/",
        }
    }
    constructor(){
        console.log("ServerStrings provider");
    }
}