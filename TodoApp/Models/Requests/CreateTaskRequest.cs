namespace TodoApp.Models.Requests
{
    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueTime { get; set; }

    }
}
