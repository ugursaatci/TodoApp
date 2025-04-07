namespace TodoApp.Models
{
    public class Task
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueTime { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
