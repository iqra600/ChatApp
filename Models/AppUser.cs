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
       



    }
}
