import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage';
import { Events } from 'ionic-angular';

@Injectable()
export class UserData {
  private id: number = null;
  private nickname: string = null;
  private image_url: string = null;
  private register_date: string = null;
  private type: number = null;
  private credit: number = null;
  private email: string = null;
  private name: string = null;
  private state: number = null;
  private token: string = null;
  private telephone: number = null;
  private cellphone: number = null;

  constructor(private storage: Storage,
    private events: Events,
  ) {
    console.log("Token provider");
  }

  getId() {
    return this.id;
  }

  setId(id: number) {
    this.id = id;
  }

  getNickname() {
    return this.nickname;
  }

  setNickname(nickname: string) {
    this.nickname = nickname;
  }

  getImage_Url() {
    return this.image_url;
  }

  setImage_Url(image_url: string) {
    this.image_url = image_url
  }

  getRegister_Date() {
    return this.register_date;
  }

  setRegister_Date(register_date: string) {
    this.register_date = register_date
  }

  getType() {
    return this.type;
  }

  setType(type: number) {
    this.type = type;
  }

  getCredit() {
    return this.credit;
  }

  setCredit(credit: number) {
    this.credit = credit;
  }

  getName() {
    return this.name;
  }

  setName(name: string) {
    this.name = name;
  }

  getState() {
    return this.state;
  }

  setState(state: number) {
    this.state = state;
  }

  getTokenAsync() {
    if (this.token === undefined) {
      return this.storage.get('token');
    }
    return Promise.resolve(this.token);
  }

  getToken() {
    return this.token;
  }

  setToken(token: string) {
    this.token = token;
    this.storage.set('token', this.token);
  }

  getEmailAsync() {
    if (this.email === undefined) {
      return this.storage.get('email');
    }
    return Promise.resolve(this.email);
  }

  getEmail() {
    return this.email;
  }

  setEmail(email: string) {
    this.email = email;
    this.storage.set('email', this.email);
  }

  getTelephone() {
    return this.telephone;
  }

  setTelephone(telephone: number) {
    this.telephone = telephone;
  }

  setCellphone(cellphone: number) {
    this.cellphone = cellphone;
  }

  getCellphone() {
    return this.cellphone;
  }


  update(data: any) {
    if (data.token !== undefined) {
      this.setToken(data.token);
    }

    if (data.id !== undefined) {
      this.setId(data.id);
    }

    if (data.nickname !== undefined) {
      this.setNickname(data.nickname);
    }

    if (data.image_url !== undefined) {
      this.setImage_Url(data.image_url);
    }

    if (data.register_date !== undefined) {
      this.setRegister_Date(data.register_date);
    }

    if (data.type !== undefined) {
      this.setType(data.type);
    }

    if (data.credit !== undefined) {
      this.setCredit(data.credit);
    }

    if (data.email !== undefined) {
      this.setEmail(data.email);
    }

    if (data.name !== undefined) {
      this.setName(data.name);
    }

    if (data.state !== undefined) {
      this.setState(data.state);
    }
    
    this.events.publish('user:updated', this);
  }
  get(){
    let user = {
      "id": this.id,
      "nickname": this.nickname,
      "image_url": this.image_url,
      "register_date": this.register_date,
      "type": this.type,
      "credit": this.credit,
      "email": this.email,
      "name": this.name,
      "state": this.state,
      "token": this.token
    }
    return user;
  }
}
