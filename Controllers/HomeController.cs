using ChatApp.Data;
using ChatApp.Models;
using ChatApp.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> manager;
        public  ApplicationDbContext context;
        public IHubContext<ChatHub> hub;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> manager,ApplicationDbContext context, IHubContext<ChatHub> hub)
        {
            _logger = logger;
            this.manager = manager;
            this.context = context;
            this.hub = hub;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user =await manager.GetUserAsync(User);
             ViewBag.usenrname=user.UserName;
            var messages = await context.Messages.ToListAsync();


            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Message message)
        
        {
            ModelState.Remove("Sender");
            ModelState.Remove("UserID");
            ModelState.Remove("UserName");




            if (ModelState.IsValid)
            {


                message.UserName = User.Identity.Name;

                message.When = DateTime.Now;

                var Sender = await manager.GetUserAsync(User);

                message.UserID = Sender.Id;

                var a = await context.AddAsync(message);

                await context.SaveChangesAsync();

                //signalR code
                Console.WriteLine("Message saved successfully");

                await hub.Clients.All.SendAsync("msgRecieve", message);

                Console.WriteLine("SignalR broadcast executed");
            }
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
