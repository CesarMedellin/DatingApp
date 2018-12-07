import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from '../../_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  // un componente hijo es cuando se manda a llamar un componente dentro de otro componente
  @Input() photos: Photo[]; // los input sirven para recibir datos de los componentes padre
  // tslint:disable-next-line:max-line-length
  @Output() getMemberPhotoChange = new EventEmitter<string>(); // los output sirven para enviar datos a los componentes padre, y con el emitter se le pasa la informacion que se quiere enviar al padre
 uploader: FileUploader;
 hasBaseDropZoneOver = false;
 baseUrl = environment.apiUrl;
 currentMain: Photo;
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      // tslint:disable-next-line:max-line-length
      this.currentMain = this.photos.filter(p => p.isMain === true)[0]; // este sirve que para el momento de cambiar la imagen de perfil, cambie a la foto qeu se trajo como main
      this.currentMain.isMain = false;
      photo.isMain = true;
      // this.getMemberPhotoChange.emit(photo.url); // se emite la funcion que se enviara al padre con el valor que queremos enviar
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto(id: number) {
    this.alertify.confirm('Estas seguro de eliminar la foto?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('La foto fue eliminada');
      }, error => {
        this.alertify.error('error al borrar la foto');
      });
    });
  }

}
