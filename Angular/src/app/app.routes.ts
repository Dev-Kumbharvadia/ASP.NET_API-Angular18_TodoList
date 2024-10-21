import { Routes } from '@angular/router';
import { LoginComponent } from './component/login/login.component';
import { LayoutComponent } from './component/layout/layout.component';
import { AdminComponent } from './component/admin/admin.component';
import { TodoComponent } from './component/todo/todo.component';
import { authGuard } from './guard/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'layout',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'admin',
        component: AdminComponent,
      },
      {
        path: 'todo',
        component: TodoComponent,
      },
    ],
  },
];
