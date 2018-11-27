import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

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


  constructor(private authService: AuthService) { }
// en el constructor se hace esto, si se quieren usa frameworks o funciones de otro componente aqui viene siendo como el using en c#,
// obvio tambien se tiene que poner el import arriba

  ngOnInit() {
  }

  login() {
    // en la linea de abajo se manda a llamar una funcion de otro componente
    // cada vez que se utilize el httpclient te tienes que suscribir
    this.authService.login(this.model).subscribe(next => { // el subscribe es algo asi como un try catch en c#
      console.log('Login correcto'); // el next es porque hizo todo correcto
    }, error => { // error es porque ocurrio un error
      console.log('Fallo el login');
    }); // para mandar a llamar el objeto model se coloca el this e igual al mandar a llamar un metodo
    // de otra clase primero se referencia en el constructor y dentro de la funcion se manda a llamar con this de igual manera
  }

  loggedIn() {
const token = localStorage.getItem('token');
  return !!token; // esto es como un if si tiene algo que retorna o sino lo tiene
  }

  logout() {
    localStorage.removeItem('token');
    console.log('Usuario desconectado');
  }

}
