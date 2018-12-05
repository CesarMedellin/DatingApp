import {Injectable} from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { AuthService } from '../_services/auth.service';

// tslint:disable-next-line:max-line-length
@Injectable() // El resolver sirve para que cuando una pagina cargue mas rapido que en lo que se manda a llamar el servicio pues que no se cargue antes de recibir los datos del servicio
 // Aqui el pipe lo utiizamos para retornar el error si es que lo muestra y si hay error te regresa a la pag members
export class MemberEditResolver implements Resolve<User> { // Implemta el resolve con user porque regresara el objeto de user
    // tslint:disable-next-line:max-line-length
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
        catchError(error => {
            this.alertify.error('Problema recibiendo tus datos');
            this.router.navigate(['/members']);
            return of(null); // este es para cerrar o cancelar el observable
        })
    );
}
}
