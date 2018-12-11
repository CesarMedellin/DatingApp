import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
// tslint:disable-next-line:max-line-length
baseUrl = environment.apiUrl; // en el environment dentro de environments se puede guardar alguna variable como de urls para poder acceder a ellas desde otras partes
constructor(private http: HttpClient) { }

getUsers(page?, ItemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

  let params = new HttpParams();

  // if para paginacion
  if (page != null && ItemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', ItemsPerPage);
  }

  // if para filtros
  if (userParams != null) {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }
  // if para likes
  if (likesParam === 'Likers') {
    params = params.append('likers', 'true');
  }
  if (likesParam === 'Likees') {
    params = params.append('likees', 'true');
  }


  // tslint:disable-next-line:max-line-length
  return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params}) // en lugar de retornar todo, entra al pipe, hace la conversion y regresa el array de usuarios paginado
  .pipe(map(response => { // los pipes tambien pueden combinar multiples funciones en una sola
    // tslint:disable-next-line:max-line-length
    paginatedResult.result = response.body; // el map recibe la informacion del servidor y la convierte para mandarla de regreso, el return de arriba no avanza hasta que el map dentro del pipe retorne la info
    if (response.headers.get('Pagination') != null) {
      // tslint:disable-next-line:max-line-length
      paginatedResult.pagination = JSON.parse(response.headers.get('Pagination')); // aqui el valor que regresa del servidor hay que convertirlo a json object o json normal porque de regreso recibe un json serializado con diferente ordenamiento
    }
    return paginatedResult;
  })
);
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

sendLike(id: number, recipientId: number) {
  return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
}

}
