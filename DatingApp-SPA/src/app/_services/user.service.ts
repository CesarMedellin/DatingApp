import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
// tslint:disable-next-line:max-line-length
baseUrl = environment.apiUrl; // en el environment dentro de environments se puede guardar alguna variable como de urls para poder acceder a ellas desde otras partes
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
  return this.http.get<User[]>(this.baseUrl + 'users');
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User) {
  return this.http.put(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(userId: number, id: number) {
  // tslint:disable-next-line:max-line-length
  return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {}); // se tiene que enviar un objeto para que se pueda consumir, asi que se envia uno vacio
}

deletePhoto(userId: number, id: number) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
}

}
