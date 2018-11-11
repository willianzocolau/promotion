import { Component, Input } from '@angular/core';

/**
 * Generated class for the ListingComponent component.
 *
 * See https://angular.io/api/core/Component for more info on Angular
 * Components.
 */
@Component({
  selector: 'list',
  templateUrl: 'list.html'
})
export class ListComponent {
  @Input() type = undefined;
  @Input() endpoint = undefined;

  public adcard = false;
  public wishlist = false;
  public order = false;
  public infinite = 0;

  constructor() {}

  ngAfterViewInit(){
    switch(this.type){
      case 'adcard-user':
        this.adcard = true; 
        break;
      case 'wishlist':
        this.wishlist = true;
        break;
      case 'order':
        this.order = true;
        break;
    } 
  }
  ngOnChanges(){
    switch(this.type){
      case 'adcard':
        this.adcard = true; 
        break;
    }
  }
  doInfinite(infiniteScroll) {
    setTimeout(() => {
      this.infinite++;
      infiniteScroll.complete();
    }, 500);
  }
}
