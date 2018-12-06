import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject} from 'rxjs';
import {map} from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl + 'auth/'; // es la baseurl del proyecto al servicio donde entrara
jwtHelper = new JwtHelperService();
decodedToken: any;
currentUser: User;
photoUrl = new BehaviorSubject<string>('../../assets/user.png'); // imagen default
currentPhotoUrl = this.photoUrl.asObservable();
constructor(private http: HttpClient) { } // dentro del constructor se manda a llamar el modulo de httpclient
 // que sirve para conectarse a un servicio y siempre recibir como objecto predefinido un objeto JSON

 changeMemberPhoto(photoUrl: string) {
   this.photoUrl.next(photoUrl);
 }

 login(model: any) {
  return this.http.post(this.baseUrl + 'login', model) // el model es un objecto que trae la password y username
   .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token); // es el servicio de login que si es correcto regresa el token del login
        localStorage.setItem('user', JSON.stringify(user.user)); // se convierte en json porque lo que recibe del servidor es un objeto
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
        this.changeMemberPhoto(this.currentUser.photoUrl);
        // console.log(this.decodedToken);
      }
    }

    )
  );
}
loggedIn() {
  const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
    }
register(model: any) {
return this.http.post(this.baseUrl + 'register', model);
}

}
