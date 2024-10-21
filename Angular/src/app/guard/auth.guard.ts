import { inject, Injectable } from '@angular/core';
import { CanActivate, CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const loggedUser = localStorage.getItem('empErpUser');
    if(loggedUser!=null){
        return true;
    }else{
        router.navigateByUrl('login')
        return false;
    }
}