import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = 'http://localhost:5000/api/auth/'; // es la baseurl del proyecto al servicio donde entrara
constructor(private http: HttpClient) { } // dentro del constructor se manda a llamar el modulo de httpclient
 // que sirve para conectarse a un servicio y siempre recibir como objecto predefinido un objeto JSON
login(model: any) {
  return this.http.post(this.baseUrl + 'login', model) // el model es un objecto que trae la password y username
   .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token); // es el servicio de login que si es correcto regresa el token del login
      }
    }

    )
  );
}

register(model: any) {
return this.http.post(this.baseUrl + 'register', model);
}

}
