using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using test1.Data;
using test1.Models;
using System;
using System.Linq;

namespace test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoDbContext dbContext;

        public TodoItemsController(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllTodoItems()
        {
            var todoItems = dbContext.TodoItems.ToList();
            return Ok(todoItems);
        }

        [HttpPost]
        public IActionResult AddTodoItem(TodoItemDTO request)
        {
            // Validate the request (optional, but recommended)
            if (request == null || string.IsNullOrEmpty(request.Title))
            {
                return BadRequest("Title is required.");
            }

            var domainModelTodoItem = new TodoItem
            {
                Id = Guid.NewGuid(), // Assuming you use Guid for Id, change to int if necessary
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow, // Use UTC for consistency
                UpdatedAt = DateTime.UtcNow // Use UTC for consistency
                // UserId can be set if needed
            };

            dbContext.TodoItems.Add(domainModelTodoItem);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetAllTodoItems), new { id = domainModelTodoItem.Id }, domainModelTodoItem);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteTodoItem(Guid id)
        {
            var todoItem = dbContext.TodoItems.Find(id);

            if (todoItem == null)
            {
                return NotFound($"Todo item with ID {id} not found.");
            }

            dbContext.TodoItems.Remove(todoItem);
            dbContext.SaveChanges();

            return NoContent(); // 204 No Content
        }
    }
}
