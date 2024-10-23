import { Component, inject, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoListService } from '../../services/todo-list.service';
import { AsyncPipe, DatePipe, JsonPipe } from '@angular/common';
import { Todo } from '../../model/class';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [AsyncPipe, DatePipe, FormsModule, JsonPipe, DatePipe],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.css'
})
export class TodoComponent implements OnInit {

  todoService = inject(TodoListService);
  authService = inject(AuthService);
  http = inject(HttpClient);

  todoList$: Observable<any> = new Observable<any>();
  todoObj: Todo = new Todo();
  todoList: any[] = [];

  ngOnInit(): void {
    this.getByUserId();
  }

  constructor () {
  }

  getByUserId() {
    if (this.authService.USER_ID) { // Check if USER_ID is set
      this.http.get(`https://localhost:7250/api/Todo/user/${this.authService.USER_ID}`)
        .subscribe((res: any) => {
          this.todoList = res.data; // Assuming the response contains a data array
        }, (error: any) => {
          console.error('Error fetching todos:', error);
          alert('Error fetching todo items');
        });
    } else {
      console.error('User ID is not set.');
      alert('User ID is not set.');
    }
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
        this.getByUserId();
      }
    }, (error: any) => {
      console.error('Error updating todo:', error);
      alert('Error updating todo');
    });
  }

  onDelete(id: string) {
      this.http.delete(`https://localhost:7250/api/Todo/delete/${id}`).subscribe(() => {
        this.todoList = this.todoList.filter(todo => todo.id !== id); // Remove from the list
        alert('Todo deleted successfully');
      }, (error: any) => {
        console.error('Error deleting todo:', error);
        alert('Error deleting todo');
      });
  }

  resetForm() {
    this.resetTodo();  // Call to reset todo object
  }

  addTodo() {
    if (!this.todoObj.title || !this.todoObj.description) {
      alert('Please fill in all required fields');
      return;
    }
    this.todoObj.userId = this.authService.USER_ID; // Set user ID
    console.log('Todo Object:', this.todoObj); // Log to check the content

    this.http.post('https://localhost:7250/api/Todo/create', this.todoObj).subscribe((res: any) => {
      this.todoList.push(res); // Append the newly added todo to the list
      alert('Todo added successfully');
      this.resetForm(); // Clear the form
      this.getByUserId();
    }, (error: any) => {
      console.error('Error adding todo:', error);
      alert('Error adding new todo');
    });
  }

  resetTodo() {
    this.todoObj = new Todo(); // Reset the todo object (clear form)
  }

  onEdit(item: Todo) {
    // Create a new instance of Todo to avoid direct reference issues
    this.todoObj = { ...item }; // Spread operator to create a shallow copy of the item

    if (item.dueDate) {
      this.todoObj.dueDate = new Date(item.dueDate); // Converts ISO string to Date
  } else {
      this.todoObj.dueDate = null; // Handle null case
  }
  }
}
