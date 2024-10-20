import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Observable } from 'rxjs';
import { TodoItem } from './models/todo.model'; // Ensure this import points to the correct file
import { AsyncPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HttpClientModule,
    AsyncPipe,
    FormsModule,
    ReactiveFormsModule,
    NgClass,
    DatePipe,
    NgIf,
    NgFor
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'], // Corrected styleUrls
})
export class AppComponent {
  http = inject(HttpClient);

  todoForm = new FormGroup({
    title: new FormControl<string>(''),
    description: new FormControl<string | null>(''),
    dueDate: new FormControl<string>(''),
    isCompleted: new FormControl<boolean>(false),
  });

  todos$ = this.getTodos();

  onFormSubmit() {
    const addTodoRequest = {
      title: this.todoForm.value.title,
      description: this.todoForm.value.description,
      isCompleted: this.todoForm.value.isCompleted,
      dueDate: this.todoForm.value.dueDate,
      createdAt: new Date(), // Set current date for createdAt
      updatedAt: new Date(), // Set current date for updatedAt
      userId: 0, // Set the appropriate userId value as needed
    };
  
    this.http
      .post('https://localhost:7116/api/TodoItems', addTodoRequest) // Correct API endpoint
      .subscribe({
        next: (value) => {
          console.log(value);
          this.todos$ = this.getTodos();
          this.todoForm.reset();
        },
        error: (error) => {
          console.error('Error adding todo:', error);
          alert('Failed to add the todo item.'); // Alert the user on error
        }
      });
  }

  onEdit(id: string) {
    // Implement the edit logic here
    alert("Edit feature not implemented yet."); // Placeholder for edit functionality
  }

  onDelete(id: string) {
    this.http.delete(`https://localhost:7116/api/TodoItems/${id}`) // Correct API endpoint
      .subscribe({
        next: (value) => {
          alert("Item deleted");
          this.todos$ = this.getTodos();
        },
      });
  }

  private getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>('https://localhost:7116/api/TodoItems'); // Correct API endpoint
  }
}
