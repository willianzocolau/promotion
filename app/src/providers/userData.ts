import { Injectable } from '@angular/core';

@Injectable()
export class UserData{
    private token : string = "";
    private email : string = "";
    constructor(){
        console.log("Token provider");
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