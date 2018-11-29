import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    { path: 'home', component: HomeComponent},
    { // sirve para navegar entre paginas
        // este es un routing multiple, en los path no podran entrar si no hay usuario lgueado, el archivo authgard tiene esa funcion
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent},
            { path: 'messages', component: MessagesComponent},
            { path: 'lists', component: ListsComponent},
        ]
    },
 
    { path: '**', redirectTo: '', pathMatch: 'full'}, // cualquier otra ruta te regresa a home
];

// ejemplo: en el navbar si quieres poner rutas en la ruta se le pone [routerLink]="['/pagina']"  y el routerinkactive o el atajo a-router
