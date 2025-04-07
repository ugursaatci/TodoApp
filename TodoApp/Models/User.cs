using Microsoft.AspNetCore.Identity;

namespace TodoApp.Models
{
    public class User: IdentityUser<int>
    {
        public ICollection<Task> Tasks { get; set; }

    }
}
