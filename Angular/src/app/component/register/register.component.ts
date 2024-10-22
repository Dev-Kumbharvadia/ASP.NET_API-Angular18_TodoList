import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerObj: any = {
      username: '',
      password: '',
      email: '',
  };

  http = inject(HttpClient);

  onRegister(){
    debugger;
    this.http.post("https://localhost:7250/api/User/register",this.registerObj).subscribe((res:any)=>{
      if(res.success){
        alert("Register successfull")
      } else {
        alert("API error")
      }
    })
  }

}
