import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';

// tslint:disable-next-line:max-line-length
@Injectable() // El resolver sirve para que cuando una pagina cargue mas rapido que en lo que se manda a llamar el servicio pues que no se cargue antes de recibir los datos del servicio
 // Aqui el pipe lo utiizamos para retornar el error si es que lo muestra y si hay error te regresa a la pag members
export class MessagesResolver implements Resolve<Message[]> { // Implemta el resolve con user porque regresara el objeto de user
    pageNumber = 1;
    pageSize = 5; // por default la api devuelve 10... No se en que casos manda el valor por default
    messageContainer = 'Unread';
    // tslint:disable-next-line:max-line-length
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.userService.getMessages(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize, this.messageContainer)
    .pipe(
        catchError(error => {
            this.alertify.error('Problema recibiendo datos');
            this.router.navigate(['/home']); // aqui te regresa al home si hay error
            return of(null); // este es para cerrar o cancelar el observable
        })
    );
}
}
