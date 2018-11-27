import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode = false;
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }


  registerToggle() {
    this.registerMode = true;
  }

  cancelRegisterMode(registerMode: boolean) { // desde el componente hijo se retorna un boolean por eso se declara asi el parametro
  this.registerMode = registerMode;
  }

}
