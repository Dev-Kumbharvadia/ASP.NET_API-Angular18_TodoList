import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginObj: any = {
    username : '',
    password: ''
  };

  router = inject(Router);
  http = inject(HttpClient);

  onLogin(){
    this.http.post("https://localhost:7116/api/User/login",this.loginObj).subscribe((res:any)=>{
      if(res.success){
        alert("login Sucess");
        sessionStorage.setItem("jwtToken",res.data)
        this.router.navigateByUrl("layout/todo")
      } else{
        alert("errrrr");
      }
    })
  }
}
