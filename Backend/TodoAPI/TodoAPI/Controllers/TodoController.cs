using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TodoAPI.Data;
using TodoAPI.Models;
using TodoAPI.Models.Entity;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/todo/all
        [HttpGet("all")]
        public ActionResult<ApiResponse<List<TodoItem>>> GetAllTodoItems()
        {
            try
            {
                var todoItems = _context.TodoItems.ToList();
                return Ok(new ApiResponse<List<TodoItem>>
                {
                    Message = "Todo items retrieved successfully",
                    Success = true,
                    Data = todoItems
                });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging library)
                return StatusCode(500, new ApiResponse<List<TodoItem>>
                {
                    Message = "Error retrieving todo items",
                    Success = false,
                    Data = null
                });
            }
        }

        // GET: api/todo/{id}
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<TodoItem>> GetTodoItemById(Guid id)
        {
            try
            {
                var todoItem = _context.TodoItems.Find(id);

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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<TodoItem>
                {
                    Message = "Error retrieving todo item",
                    Success = false,
                    Data = null
                });
            }
        }

        // GET: api/todo/user/{userId}
        [HttpGet("user/{userId}")]
        public ActionResult<ApiResponse<List<TodoItem>>> GetTodoItemsByUserId(Guid userId)
        {
            try
            {
                var todoItems = _context.TodoItems.Where(t => t.UserId == userId).ToList();

                if (!todoItems.Any())
                {
                    return NotFound(new ApiResponse<List<TodoItem>>
                    {
                        Message = "No todo items found for this user",
                        Success = false,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<TodoItem>>
                {
                    Message = "Todo items retrieved successfully",
                    Success = true,
                    Data = todoItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<TodoItem>>
                {
                    Message = "Error retrieving todo items",
                    Success = false,
                    Data = null
                });
            }
        }

        // POST: api/todo/create
        [HttpPost("create")]
        public ActionResult<ApiResponse<TodoItem>> CreateTodoItem([FromBody] AddTodo newTodo)
        {
            if (newTodo == null)
            {
                return BadRequest(new ApiResponse<TodoItem>
                {
                    Message = "Invalid input data",
                    Success = false,
                    Data = null
                });
            }

            try
            {
                var todoItem = new TodoItem
                {
                    Id = Guid.NewGuid(),
                    Title = newTodo.Title,
                    Description = newTodo.Description,
                    IsCompleted = newTodo.IsCompleted,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = newTodo.UserId // Automatically associate with current user
                };

                _context.TodoItems.Add(todoItem);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetTodoItemById), new { id = todoItem.Id }, new ApiResponse<TodoItem>
                {
                    Message = "Todo item created successfully",
                    Success = true,
                    Data = todoItem
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<TodoItem>
                {
                    Message = "Error creating Todo item",
                    Success = false,
                    Data = null
                });
            }
        }

        // PUT: api/todo/update/{id}
        [HttpPut("update/{id}")]
        public ActionResult<ApiResponse<TodoItem>> UpdateTodoItem(Guid id, [FromBody] UpdateTodo updatedTodo)
        {
            if (!ModelState.IsValid || updatedTodo == null)
            {
                return BadRequest(new ApiResponse<TodoItem>
                {
                    Message = "Invalid data",
                    Success = false,
                    Data = null
                });
            }

            try
            {
                var existingTodo = _context.TodoItems.Find(id);
                if (existingTodo == null)
                {
                    return NotFound(new ApiResponse<TodoItem>
                    {
                        Message = "Todo item not found",
                        Success = false,
                        Data = null
                    });
                }

                // Update the todo item properties
                existingTodo.Title = updatedTodo.Title;
                existingTodo.Description = updatedTodo.Description;
                existingTodo.IsCompleted = updatedTodo.IsCompleted;
                existingTodo.DueDate = updatedTodo.DueDate;
                existingTodo.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok(new ApiResponse<TodoItem>
                {
                    Message = "Todo item updated successfully",
                    Success = true,
                    Data = existingTodo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<TodoItem>
                {
                    Message = "Error updating Todo item",
                    Success = false,
                    Data = null
                });
            }
        }

        // DELETE: api/todo/delete/{id}
        [HttpDelete("delete/{id}")]
        public ActionResult<ApiResponse<TodoItem>> DeleteTodoItem(Guid id)
        {
            try
            {
                var todoItem = _context.TodoItems.Find(id);
                if (todoItem == null)
                {
                    return NotFound(new ApiResponse<TodoItem>
                    {
                        Message = "Todo item not found",
                        Success = false,
                        Data = null
                    });
                }

                _context.TodoItems.Remove(todoItem);
                _context.SaveChanges();

                return Ok(new ApiResponse<TodoItem>
                {
                    Message = "Todo item deleted successfully",
                    Success = true,
                    Data = todoItem
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<TodoItem>
                {
                    Message = "Error deleting Todo item",
                    Success = false,
                    Data = null
                });
            }
        }
    }
}
