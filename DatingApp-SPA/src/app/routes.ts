import { Routes, CanDeactivate } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChages } from './_guards/prevent-unsaved-changes.guard';

export const appRoutes: Routes = [
    { path: 'home', component: HomeComponent},
    { // sirve para navegar entre paginas
        // este es un routing multiple, en los path no podran entrar si no hay usuario lgueado, el archivo authgard tiene esa funcion
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver}},
            // tslint:disable-next-line:max-line-length
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver}}, // como se carga de manera asincrona esta pagina se utiliza un resolver, primero entra al resolver para traer los datos y luego ya carga la pagina
            // tslint:disable-next-line:max-line-length
            { path: 'member/edit', component: MemberEditComponent, resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChages]},
            { path: 'messages', component: MessagesComponent},
            { path: 'lists', component: ListsComponent},
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full'}, // cualquier otra ruta te regresa a home
];

// ejemplo: en el navbar si quieres poner rutas en la ruta se le pone [routerLink]="['/pagina']"  y el routerinkactive o el atajo a-router
