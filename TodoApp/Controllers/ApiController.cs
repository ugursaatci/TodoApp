using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetTodos(int userId) 
        {
            var todoList = await _context.Tasks.Where(i => i.UserID == userId).ToListAsync();
            if (todoList.Count == 0)
            {
                return BadRequest("Kullanıcıya ait görev bulunamadı!");
            }
            
            string jsonString = JsonSerializer.Serialize(todoList);
            
            return Ok(jsonString);
        }

        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTodo(UpdateTodoRequest request)
        {
            var todoTask = _context.Tasks.Where(u => u.ID == request.ID).FirstOrDefault();
            if (todoTask != null)
            {
                todoTask.IsCompleted = request.IsCompleted;
                todoTask.Title = request.Title;
                todoTask.Description = request.Description;
                var result = await _context.SaveChangesAsync();
                return Ok(result);
            }

            return BadRequest("Görev Bulunamadı");
        }

        [HttpDelete("delete-task")]
        public async Task<IActionResult> DeleteTodo(Guid taskID)
        {
            var todoTask = _context.Tasks.FirstOrDefault(u => u.ID == taskID);
            if (todoTask != null)
            {
                _context.Tasks.Remove(todoTask);
                await _context.SaveChangesAsync();
                return Ok("Görev Silindi");
            }
            return BadRequest("Görev Bulunamadı");
        }
    }
}
