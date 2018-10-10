import { Component, Input } from '@angular/core';

/**
 * Generated class for the AdcardComponent component.
 *
 * See https://angular.io/api/core/Component for more info on Angular
 * Components.
 */
@Component({
  selector: 'adcard',
  templateUrl: 'adcard.html'
})
export class AdcardComponent {

  @Input() list;
  
  constructor() {}

}
