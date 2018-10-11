import { Component } from "@angular/core";

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})

export class HomePage {
  public promotions = [];
  constructor() {      
    this.promotions = [{      
      promotion: {
        "registerdate": "10-09-2018",
        "description": "Mouse muito legal",
        "likes": 12,
        "comments": 7,
        "image_url": "https://purepng.com/public/uploads/large/purepng.com-pc-mousepc-mousepcmouselaptop-mouse-1701528347542ws1aa.png"
      },
      user:{
        "nickname": "andre",
        "image_url": "http://ok.com/ok.jpg"
      }
    }];
  }
}
