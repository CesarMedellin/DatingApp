import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // el model de aqui abajo debe es como un var dentro de una clase o metodo de c# y abajo se declara como cualquier tipo any y en array{}
model: any = {}; // este objeto servira para guardar aqui el password y el usuario
// el model se esta recargando dentro de la vista del componente en el username y password
// y se manda a llamar dentro del ngSubmit del form en la funcion login
photoUrl: string; // variable para cargar la foto de perfil en navbar

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }
// cuando es private solo se puede usar en el componnte pero cuando es public tambien se puede usar en la vista
// en el constructor se hace esto, si se quieren usa frameworks o funciones de otro componente aqui viene siendo como el using en c#,
// obvio tambien se tiene que poner el import arriba

  ngOnInit() {
    // tslint:disable-next-line:max-line-length
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl); /// se obtiene del authservice la url de la foto y se cargara
  }

  login() {
    // en la linea de abajo se manda a llamar una funcion de otro componente
    // cada vez que se utilize el httpclient te tienes que suscribir
    // tslint:disable-next-line:max-line-length
    this.authService.login(this.model).subscribe(next => { // el subscribe es algo asi como un try catch en c# pero se necesitan para poder ejecutarser los servicios
      this.alertify.success('Login correcto'); // el next es porque hizo todo correcto
    }, error => { // error es porque ocurrio un error
      this.alertify.error(error);
    }, () => { // este es un parametro vacio, si llega en next va a members, pero no se con error
      this.router.navigate(['/members']);
    }); // para mandar a llamar el objeto model se coloca el this e igual al mandar a llamar un metodo
    // de otra clase primero se referencia en el constructor y dentro de la funcion se manda a llamar con this de igual manera
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null; // se elimina esto para ya no tener datos del usuario con la sesion
    this.authService.currentUser = null; // se elimina esto para ya no tener datos del usuario con la sesion
    this.alertify.message('Usuario desconectado');
    this.router.navigate(['/home']);
  }

}
