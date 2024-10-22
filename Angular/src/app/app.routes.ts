import { Routes } from '@angular/router';
import { LoginComponent } from './component/login/login.component';
import { LayoutComponent } from './component/layout/layout.component';
import { AdminComponent } from './component/admin/admin.component';
import { TodoComponent } from './component/todo/todo.component';
import { authGuard } from './guard/auth.guard';
import { RegisterComponent } from './component/register/register.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },  
  {
    path: '',
    component: LoginComponent,
  },
  {
    path: 'signup',
    component: RegisterComponent,
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
