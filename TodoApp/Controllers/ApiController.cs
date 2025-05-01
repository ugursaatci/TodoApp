using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TodoApp.Models;
using TodoApp.Models.Requests;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly string nameID = "nameid";

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
            var userId = getUserID(HttpContext.Request.Headers["Authorization"]);

            if (request.DueTime < DateTime.Now)
            {
                return BadRequest("Görev Zamanı Geçmiş Olamaz!");
            }
            
            Models.Task task = new() 
            {
                ID = Guid.NewGuid().ToString(),
                Title = request.Title,
                Description = request.Description,
                IsCompleted = false,
                CreatedDate = DateTime.Now,
                DueTime = request.DueTime,
                UserID = userId,
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

        [Authorize]
        [HttpGet("get-task-list")]
        public async Task<IActionResult> GetTaskList() 
        {
            var userId = getUserID(HttpContext.Request.Headers["Authorization"]);

            var todoList = await _context.Tasks.Where(i => i.UserID == userId).ToListAsync();
            if (todoList.Count == 0)
            {
                return BadRequest("Kullanıcıya ait görev bulunamadı!");
            }
            
            string jsonString = JsonSerializer.Serialize(todoList);
            
            return Ok(jsonString);
        }

        [Authorize]
        [HttpGet("get-task-detail")]
        public async Task<IActionResult> GetTaskDetail(string taskId) 
        {
            var userId = getUserID(HttpContext.Request.Headers["Authorization"]);
            var task = await _context.Tasks.FirstOrDefaultAsync(i => i.ID == taskId && i.UserID == userId);

            if (task == null)
            {
                return BadRequest("Görev Bulunamadı!");
            }

            string result = JsonSerializer.Serialize(task);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTodo(UpdateTodoRequest request)
        {
            var userId = getUserID(HttpContext.Request.Headers["Authorization"]);
            var todoTask = _context.Tasks.Where(u => u.ID == request.ID && u.UserID == userId).FirstOrDefault();
            if (todoTask != null)
            {
                todoTask.IsCompleted = request.IsCompleted;
                todoTask.Title = request.Title;
                todoTask.Description = request.Description;
                var result = await _context.SaveChangesAsync();
                return Ok(result);
            }

            throw new Exception("Görev Bulunamadı");
        }

        [Authorize]
        [HttpDelete("delete-task")]
        public async Task<IActionResult> DeleteTodo(string taskID)
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

        private int getUserID(string token)
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                var claim = jwtToken.Claims.FirstOrDefault(x => x.Type == nameID)?.Value;
                if (claim != null && Int32.TryParse(claim, out int id))
                {
                    return id;
                }

                throw new Exception("Token Decode Edilirken Hata Oluştu!");
            }
            catch (Exception ex)
            {
                throw new Exception("JWT Token çözülürken hata oluştu: " + ex.Message);
            }
        }
    }
}
