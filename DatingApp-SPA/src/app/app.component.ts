import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit { // si queremos ejecutar otros componentes desde el appcomponent se pone el implements oninit

  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // tslint:disable-next-line:max-line-length
    const token = localStorage.getItem('token'); // todo esto sirve para cuando haya una sesion se carguen los datos del usuario que se muestran en pantalla
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
  }
}
