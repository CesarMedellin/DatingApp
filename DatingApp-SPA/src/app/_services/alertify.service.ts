import { Injectable } from '@angular/core';
declare let alertify: any; // como ya se importo el script en el angular.json y los estilos en styles.css ya no hace falta importarlo
@Injectable({
  providedIn: 'root'
})
export class AlertifyService {
// este servicio es para las notificaciones que aparecen en la esquina de la pantalla
constructor() { }

confirm(message: string, okCallback: () => any) { // recibe el mensaje y el callback del boton
  alertify.confirm(message, function(e) {
    if (e) {
      okCallback();
    } else {}
  });
}
success(message: string) {
  alertify.success(message);
}

error(message: string) {
  alertify.error(message);
}

warning(message: string) {
  alertify.warning(message);
}

message(message: string) {
  alertify.message(message);
}
}
