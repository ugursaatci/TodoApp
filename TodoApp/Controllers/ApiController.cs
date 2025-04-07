using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Models.Requests;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly TodoContext _context;

        public ApiController(TodoContext todoContext) 
        {
            _context = todoContext;
        }

        [Authorize]
        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask(CreateTaskRequest request) 
        {
            if (request == null)
            {
                return BadRequest("Invalid Request");
            }
            Models.Task task = new() 
            {
                ID = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                IsCompleted = true,
                DueTime = request.DueTime,
                UserID = request.UserID
            };

            try
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return Ok(request);
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpGet("get-todo-list")]
        public async Task<IActionResult> GetTodos() 
        {
            return Ok();
        }
        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTodo()
        {
            return Ok();
        }
        [HttpDelete("delete-task")]
        public async Task<IActionResult> DeleteTodo()
        {
            return Ok();
        }
    }
}
