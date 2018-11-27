import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // Si colocamos un componente dentro de otro componente
  // El componente principal seria el padre(Home) y este es el hijo(register)
  // Si desde el componente padre queremos enviar informacion, se utiliza el @input de angular core importado
  // Si desde el componente hijo se quiere enviar informacion al padre, se utiliza @output con la variable como si fuera eventemitter
  // Desde el padre se envia la informacion al hijo desde la llamada del componente en lo que seria la vista, aqui recibimos valuesfromhome
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(next => {
      console.log('Registrado'); }, error => {
        console.log('ocurrio un problema la insertar');
      });
  }

  cancel() {
    this.cancelRegister.emit(false); // asi se envia el valor del componente hijo al padre, con el emit dentro puede tener objectos,bool,etc
    console.log('cancelado');
  }

}
