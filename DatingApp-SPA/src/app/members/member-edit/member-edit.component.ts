import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm; // este sirve para poder acceder al form y asi
  user: User;
  // tslint:disable-next-line:max-line-length
  @HostListener('window:beforeunload', ['$event']) // este sirve para cuando quieras cerrar la pestaña te muestre un mensaje si no has guardado cambios
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
  }
}
  // tslint:disable-next-line:max-line-length
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userService: UserService, private authService: AuthService) { } // se pone en el constructor los modulos que se vayan a utilizar

  ngOnInit() {
    this.route.data.subscribe(data => { // es lo primero que carga asi que carga la informacion desde el resolver
      this.user = data['user'];
    });
  }

  updateUser() {
    // tslint:disable-next-line:max-line-length
    // en ese subscribe es donde se utiliza el authservice para obtener el id del token y el user service para acceder al servicio de update del usuario
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Se actualizo el perfil');
      this.editForm.reset(this.user); // una vez que el servicio se haya guardado correctamente este editform resetea el form

    }, error => {
      this.alertify.error(error);
    });
  }

}

