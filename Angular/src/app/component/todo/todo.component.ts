import { Component, inject, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoListService } from '../../services/todo-list.service';
import { AsyncPipe, DatePipe, JsonPipe } from '@angular/common';
import { Todo } from '../../model/class';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [AsyncPipe, DatePipe, FormsModule, JsonPipe],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.css'
})
export class TodoComponent implements OnInit {

  todoService = inject(TodoListService);

http = inject(HttpClient);

  todoList$: Observable<any> = new Observable<any>;
  todoObj: Todo = new Todo();

  todoList: any[] =[];

  ngOnInit(): void {
    this.todoList$ = this.todoService.getAllTodo();
    this.getJwtAll();
  }

  getJwtAll(){
    this.http.get('https://localhost:7250/api/Todo/all').subscribe((res: any)=>{
      this.todoList = res.data;
    })
  }

  updateTodo(){

  }

  deleteTodo(id: string){
    const isDelte = confirm("Are you sure?")
    if(isDelte){
      this.todoService.deleteTodo(id).subscribe((res: any) => 
      {
        if(!res){
          alert("Deleted")
          this.todoList$ = this.todoService.getAllTodo();
        } else {
          alert("server side error")
        }
      });
    }
  }

  addTodo(){
    
  }

  onEdit(item: any){
    this.todoObj = item;
  }
}
