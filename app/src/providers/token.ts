import { Injectable } from '@angular/core';

@Injectable()
export class Token{
    private data : string = "";
    constructor(){
        console.log("Token provider");
    }
    getToken(){
        return this.data;
    }

    setToken(token: string){
        this.data = token;
    }
}