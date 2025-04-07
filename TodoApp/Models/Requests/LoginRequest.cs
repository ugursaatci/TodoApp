using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
