import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';

@Component({
  selector: 'app-members-card',
  templateUrl: './members-card.component.html',
  styleUrls: ['./members-card.component.css']
})
export class MembersCardComponent implements OnInit {
  @Input() user: User; // tiene el input porque es un componente hijo que recibe datos del padre que es memberlist
  constructor() { }

  ngOnInit() {
  }

}
