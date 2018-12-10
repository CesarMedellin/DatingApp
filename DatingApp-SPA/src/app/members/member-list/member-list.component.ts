import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from '../../_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
users: User[];
user: User = JSON.parse(localStorage.getItem('user'));
genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];
userParams: any = {};
pagination: Pagination;
  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    // se conecta a un resolver en el routes.ts y el resolver es el que se conecta al servicio
    // porque ntes todavia no regresaba de pedir los datos y ya habia cargado la pagina
    this.route.data.subscribe(data => {
    // tslint:disable-next-line:max-line-length
    this.users = data['users'].result; // este data como ya tiene la paginacion, podemos acceder a toda la paginacion de la funcion desde aqui
    this.pagination = data['users'].pagination;
  });
  // para la paginacion aqui se aplica lo mismo que en la api para los filtros
  this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
  this.userParams.minAge = 18;
  this.userParams.maxAge = 99;
  this.userParams.orderBy = 'lastActive';
  }
// paginacion
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  // tslint:disable-next-line:max-line-length
  loadUsers() { // este load users manda la pagina actual y los items por paginas actuales al servicio de getusers para regresar la paginacion que se coloco
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }

  resetFilters() { // esta funcion es para restablecer los filtros
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }
// end paginacion
}
