import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../_models/message';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
@Input() recipientId: number;
messages: Message[];
newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentuserId = +this.authService.decodedToken.nameid; // el mas es para que sea como numero
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        tap(messages => {
          for (let i = 0; i < messages.length; i++) {
            if (messages[i].isRead === false && messages[i].recipientId === currentuserId) {
                this.userService.markAsRead(currentuserId, messages[i].id);
            }
          }
        })
      )
      .subscribe(messages => {
        this.messages = messages;
      }, error => {
        this.alertify.error(error);
      });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    // tslint:disable-next-line:max-line-length
    // el subscribe otra definicion de como funciona es que eporque lo que hagamos regresara un resultado y con el subsrcibe lo transformamos para entender lo que regreso
    // tslint:disable-next-line:max-line-length
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => { // la lista messages es de tipo mensaje asi que lo que retorna el subsribe tiene que ser tipo message
      // tslint:disable-next-line:max-line-length
      this.messages.unshift(message); // unshift sirve para voltear el orden del array que viene siendo lo mismo que push asi que agregara el mensaje a la lista que ya estaba antes mostrandose
    this.newMessage.content = ''; // se vacia el mensaje que se acaba de enviar
    }, error => {
      this.alertify.error(error);
    });
  }

}
