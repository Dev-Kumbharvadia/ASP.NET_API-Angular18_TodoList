using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Apply authorization to the entire controller
    public class TodoController : ControllerBase
    {
        // For simplicity, we'll use an in-memory list to store todo items.
        private static List<TodoItem> _todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 2", IsCompleted = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        };

        // GET: api/todo/all
        [HttpGet("all")]
        public ActionResult<ApiResponse<List<TodoItem>>> GetAllTodoItems()
        {
            var response = new ApiResponse<List<TodoItem>>
            {
                Message = "Todo items retrieved successfully",
                Success = true,
                Data = _todoItems.ToList()
            };
            return Ok(response);
        }

        // GET: api/todo/{id}
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<TodoItem>> GetTodoItemById(Guid id)
        {
            var todoItem = _todoItems.FirstOrDefault(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound(new ApiResponse<TodoItem>
                {
                    Message = "Todo item not found",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new ApiResponse<TodoItem>
            {
                Message = "Todo item retrieved successfully",
                Success = true,
                Data = todoItem
            });
        }

        // POST: api/todo/create
        [HttpPost("create")]
        public ActionResult<ApiResponse<TodoItem>> CreateTodoItem([FromBody] AddTodo newTodo)
        {
            // Create a new TodoItem and associate it with the current user
            var todoItem = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = newTodo.Title,
                Description = newTodo.Description,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = GetCurrentUserId() // Associate with the current user's ID
            };

            _todoItems.Add(todoItem);

            var response = new ApiResponse<TodoItem>
            {
                Message = "Todo item created successfully",
                Success = true,
                Data = todoItem
            };

            return CreatedAtAction(nameof(GetTodoItemById), new { id = todoItem.Id }, response);
        }

        // PUT: api/todo/update/{id}
        [HttpPut("update/{id}")]
        public ActionResult<ApiResponse<TodoItem>> UpdateTodoItem(Guid id, [FromBody] TodoItem updatedTodo)
        {
            var existingTodo = _todoItems.FirstOrDefault(t => t.Id == id);

            if (existingTodo == null)
            {
                return NotFound(new ApiResponse<TodoItem>
                {
                    Message = "Todo item not found",
                    Success = false,
                    Data = null
                });
            }

            existingTodo.Title = updatedTodo.Title;
            existingTodo.Description = updatedTodo.Description;
            existingTodo.IsCompleted = updatedTodo.IsCompleted;
            existingTodo.DueDate = updatedTodo.DueDate;
            existingTodo.UpdatedAt = DateTime.Now;

            var response = new ApiResponse<TodoItem>
            {
                Message = "Todo item updated successfully",
                Success = true,
                Data = existingTodo
            };

            return Ok(response);
        }

        // DELETE: api/todo/delete/{id}
        [HttpDelete("delete/{id}")]
        public ActionResult<ApiResponse<TodoItem>> DeleteTodoItem(Guid id)
        {
            var todoItem = _todoItems.FirstOrDefault(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound(new ApiResponse<TodoItem>
                {
                    Message = "Todo item not found",
                    Success = false,
                    Data = null
                });
            }

            _todoItems.Remove(todoItem);

            var response = new ApiResponse<TodoItem>
            {
                Message = "Todo item deleted successfully",
                Success = true,
                Data = todoItem
            };

            return Ok(response);
        }

        // Helper method to get the current user's ID from the claims
        private int GetCurrentUserId()
        {
            // Assuming you are using UserId as the NameIdentifier claim
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
