import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
user: User;
// dos variables del tipo de ngxgallery para mostrar imagenes
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];
  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    // tslint:disable-next-line:max-line-length
    this.route.data.subscribe(data => { // se usa un resolver porque esta pagina se abre apartir de un click en otra y es de manera asincrona, antes cargaba primero la pagina y daba error ahora ya espera a los datos
      this.user = data['user']; // aqui se suscribe al resolver que trajo la informacion para poder mostrarla
    });

    // asi se cargaria la galeria
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();

  }

  getImages() {
    const imageUrls = [];
    for (let i = 0; i < this.user.photos.length; i++) {
     imageUrls.push({
        small: this.user.photos[i].url,
        medium: this.user.photos[i].url,
        big: this.user.photos[i].url,
        description: this.user.photos[i].description,
     });
    }
    return imageUrls;
  }

  // tslint:disable-next-line:max-line-length
  // Ya no se usara este porque al momento de cargar la pagina trata de obtener el id de manera asincrona cuando aun no lo tiene y pues por eso en el html se le pone ? a los parametros
  // loadUser() {
    // se obtiene la url y apartir de ahi se trae los datos del id del cliente que este
    // como la url es string y el parametro es int se le pone un + antes para convertirlo
    // como el servicio getUser devuelve un observable hay que susribirse para recibir la info
    // el id de la url se declara en el archcivo routes
    // this.userService.getUser(+this.route.snapshot.params['id']).subscribe((user: User) => {
      // this.user = user; // el this.user es el que esta por el export y el otro es el que retorna el calor
    // }, error => {
      // this.alertify.error(error);
    // });
  // }

}
