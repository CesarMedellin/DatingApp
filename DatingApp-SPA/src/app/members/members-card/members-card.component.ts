import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';

@Component({
  selector: 'app-members-card',
  templateUrl: './members-card.component.html',
  styleUrls: ['./members-card.component.css']
})
export class MembersCardComponent implements OnInit {
  @Input() user: User; // tiene el input porque es un componente hijo que recibe datos del padre que es memberlist
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) {
   }

  ngOnInit() {
  }
sendLike(id: number) {
  this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(data => {
    this.alertify.success('te ha gustado ' + this.user.knownAs);
  }, error => {
    this.alertify.error(error);
  });
}
}
