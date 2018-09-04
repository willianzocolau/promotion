import {Component} from "@angular/core";
import {NavController} from "ionic-angular";
import {LoginPage} from "../login/login";
import {HomePage} from "../home/home";
import {Validators, FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {

  public form : FormGroup;

  constructor(public nav: NavController, public formBuilder: FormBuilder) {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      nickname: ['', Validators.required],
      cpf: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', Validators.required],
      confirm_password: ['', Validators.required]
    });
  }

  // register and go to home page
  register() {
    console.log(this.form.value)
  }

  // go to login page
  login() {
    this.nav.setRoot(LoginPage);
  }
}
