using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Message
    {
        public int ID   { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        public string UserID{get;set;}
        public virtual AppUser Sender{ get; set; }
        public DateTime When { get; set; }

    }
}
