import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';
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
    // tslint:disable-next-line:max-line-length
    const user: User = JSON.parse(localStorage.getItem('user')); // este sirve para recibir los datos de usuario y asi poder cargar la imagen a un lado del navbar
   // el const se tiene que declarar tipo User para cargar los datos en la interfaz
   // pero tambien el string completo del localstorage se tiene que convertir a json para que quede con a interfaz
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user; // aqui el localstorage de user ya en json y con interfaz user
      this.authService.changeMemberPhoto(user.photoUrl); // aqui obtiene la foto del perfil y no la default
    }
  }
}
