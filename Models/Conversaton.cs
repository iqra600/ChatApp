namespace ChatApp.Models
{
    public class Conversaton
    {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
