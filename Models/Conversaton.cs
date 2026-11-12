using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ChatApp.Models
{
    public class Conversaton
    {

        public int Id { get; set; }
        public string SenderId { get; set; }
        public virtual AppUser Sender { get; set; }

        public string ReciverId { get; set; }
        //  public virtual ReciverInfo Reciever { get; set; }
        public virtual AppUser Reciever { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual List<Message> Messages { get; set; }

    }
}
