import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage';

@Injectable()
export class UserData{
    private id : number = null;
    private nickname :  string = null;
    private image_url : string = null;
    private register_date : string = null;
    private type : string = null;
    private credit : string = null;
    public email : string = null;
    private name : string = null;
    private state : string = null;
    public token : string = null;

    constructor(private storage: Storage){
        console.log("Token provider");
    }

    getId(){
        return this.storage.get('id');
    }

    setId(id: number){
        this.id = id;
        this.storage.set('id', this.id);
    }

    getNickname(){
        return this.storage.get('nickname');
    }

    setNickname(nickname: string){
        this.nickname = nickname;
        this.storage.set('nickname', this.nickname);
    }

    getImage_Url(){
        return this.storage.get('image_url');
    }

    setImage_Url(image_url: string){
        this.image_url = image_url
        this.storage.set('image_url', this.image_url);
    }

    getRegister_Date(){
        return this.storage.get('register_date');
    }

    setRegister_Date(register_date: string){
        this.register_date = register_date
        this.storage.set('register_date', this.register_date);
    }

    getType(){
        return this.storage.get('type');
    }

    setType(type: string){
        this.type = type;
        this.storage.set('type', this.type);
    }

    getCredit(){
        return this.storage.get('credit');
    }

    setCredit(credit: string){
        this.credit = credit;
        this.storage.set('credit', this.credit);
    }

    getName(){
        return this.storage.get('name');
    }

    setName(name: string){
        this.name = name;
        this.storage.set('name', this.name);
    }

    getState(){
        return this.storage.get('state');
    }

    setState(state: string){
        this.state = state;
        this.storage.set('state', this.state);
    }

    getToken(){
        return this.storage.get('token');
    }

    setToken(token: string){
        this.token = token;
        this.storage.set('token', this.token);
    }

    getEmail(){
        return this.storage.get('email');
    }

    setEmail(email: string){
        this.email = email;
        this.storage.set('email', this.email);
    }
}