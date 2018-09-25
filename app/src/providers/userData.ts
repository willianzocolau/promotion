import { Injectable } from '@angular/core';

@Injectable()
export class UserData{
    private id : int = null;
    private nickname :  string = null;
    private image_url : string = null;
    private register_date : date = null;
    //private type : int = null;
    private credit : double = null;
    private email : string = null;
    private name : string = null;
    //private state : string = null;
    private token : string = null;

    constructor(){
        console.log("Token provider");
    }

    getId(){
        return this.id;
    }

    getNickname(){
        return this.nickname;
    }

    setNickname(nickname: string){
        this.nickname = nickname;
    }

    getImage_Url(){
        return this.image_url;
    }

    getRegister_Date(){
        return this.register_date;
    }

    getCredit(){
        return this.credit;
    }

    setCredit(credit: double){
        this.credit = credit;
    }

    getName(){
        return this.name;
    }

    setName(name: string){
        this.name = name;
    }

    getToken(){
        return this.token;
    }

    setToken(token: string){
        this.token = token;
    }

    getEmail(){
        return this.email;
    }

    setEmail(email: string){
        this.email = email;
    }
}