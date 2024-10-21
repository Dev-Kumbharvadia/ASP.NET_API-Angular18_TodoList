import { Component, inject, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoListService } from '../../services/todo-list.service';
import { AsyncPipe, DatePipe } from '@angular/common';
import { Todo } from '../../model/class';
import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [AsyncPipe, DatePipe],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.css'
})
export class TodoComponent implements OnInit {

  todoService = inject(TodoListService);

  todoList$: Observable<any> = new Observable<any>;
  todoObj: Todo = new Todo();


  ngOnInit(): void {
    this.todoList$ = this.todoService.getAllTodo();
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
