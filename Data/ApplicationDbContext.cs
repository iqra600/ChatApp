using ChatApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversaton> Conversations { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
            builder.Entity<Message>().HasOne<AppUser>(x => x.Sender).WithMany(x => x.messages).HasForeignKey(x => x.UserID);

            // Message -> Conversation
            builder.Entity<Message>()
                .HasOne(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.CoversationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversaton>()
         .HasOne(c => c.Sender)
         .WithMany(u => u.Senders)
         .HasForeignKey(c => c.SenderId)
         .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversaton>()
                .HasOne(c => c.Reciever)
                .WithMany(u => u.Recivers)
                .HasForeignKey(c => c.ReciverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
