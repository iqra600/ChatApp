using Microsoft.AspNetCore.Identity;

namespace ChatApp.Models
{
    public class AppUser:IdentityUser
    {
        public AppUser()
        {
            messages=new HashSet<Message>();
          
        }

        public   ICollection<Message> messages { get; set; }
        public ICollection<Conversaton> Senders { get; set; }
        public ICollection<Conversaton> Recivers { get; set; }






    }
}
