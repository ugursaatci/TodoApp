namespace TodoApp.Models.Requests
{
    public class CreateTaskRequest
    {
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueTime { get; set; }

    }
}
