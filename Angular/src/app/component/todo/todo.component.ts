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
    this.getAllTodo();
  }

  getAllTodo() {
    this.http.get('https://localhost:7250/api/Todo/all').subscribe((res: any) => {
      this.todoList = res.data; // Assuming the response contains a data array
    }, (error: any) => {
      console.error('Error fetching todos:', error);
      alert('Error fetching todo items');
    });
  }
  
  getById(){
    this.http.get('https://localhost:7250/api/Todo/',{}).subscribe((res: any) => {
      this.todoList = res.data; // Assuming the response contains a data array
    }, (error: any) => {
      console.error('Error fetching todos:', error);
      alert('Error fetching todo items');
    });
  }

  onUpdate() {
    if (!this.todoObj.id) {
      alert('No todo selected for update');
      return;
    }
  
    this.http.put(`https://localhost:7250/api/Todo/update/${this.todoObj.id}`, this.todoObj).subscribe((res: any) => {
      const index = this.todoList.findIndex(todo => todo.id === this.todoObj.id);
      if (index !== -1) {
        this.todoList[index] = res; // Update the todo in the list
        alert('Todo updated successfully');
        this.resetTodo(); // Clear the form
      }
    }, (error: any) => {
      console.error('Error updating todo:', error);
      alert('Error updating todo');
    });
  }
  

  onDelete(id: string) {
    const isDelete = confirm('Are you sure you want to delete this todo?');
    if (isDelete) {
      this.http.delete(`https://localhost:7250/api/Todo/delete/${id}`).subscribe(() => {
        this.todoList = this.todoList.filter(todo => todo.id !== id); // Remove from list
        alert('Todo deleted successfully');
      }, (error: any) => {
        console.error('Error deleting todo:', error);
        alert('Error deleting todo');
      });
    }
  }
  

  addTodo() {
    if (!this.todoObj.title || !this.todoObj.description) {
      alert('Please fill in all required fields');
      return;
    }
  
    this.http.post('https://localhost:7250/api/Todo/create', this.todoObj).subscribe((res: any) => {
      this.todoList.push(res); // Append the newly added todo to the list
      alert('Todo added successfully');
      this.resetTodo(); // Clear the form
    }, (error: any) => {
      console.error('Error adding todo:', error);
      alert('Error adding new todo');
    });
  }
  
  resetTodo() {
    this.todoObj = new Todo(); // Reset the todo object (clear form)
  }
  

  onEdit(item: any){
    this.todoObj = item;
  }
}
