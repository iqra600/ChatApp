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
using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> manager;
        public ApplicationDbContext context;
        public IHubContext<ChatHub> hub;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> manager, ApplicationDbContext context, IHubContext<ChatHub> hub)
        {
            _logger = logger;
            this.manager = manager;
            this.context = context;
            this.hub = hub;
        }

        public async Task<IActionResult> ListChat()
        {
            var user = await manager.GetUserAsync(User);
            ViewBag.user = user;
            var conversations = await context.Conversations.Where(x => x.SenderId == user.Id || x.ReciverId==user.Id).Include(x => x.Reciever).Include(x => x.Sender).Include(x=>x.Messages).OrderByDescending(x => x.Messages.Max(x=>(DateTime)x.When)).ToListAsync();
            return View(conversations);
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? Id, string? ReciverId)
        {
            var user = await manager.GetUserAsync(User);
            ViewBag.usenrname = user.UserName;
            ViewBag.CurrentUserId = user.Id;
            //int ID = Id.HasValue ? Id.Value : 0;
            ViewBag.ReciverID = !string.IsNullOrWhiteSpace(ReciverId) ? ReciverId.Trim() : "";
            if (Id.HasValue && Id.Value > 0)
            {
                var messages = await context.Messages.Where(x => x.CoversationId == Id.Value).Include(x => x.Sender).Include(x => x.Reciver).ToListAsync();
                return View(messages);

            }
            if (!string.IsNullOrWhiteSpace(ReciverId))
            {

                var conversations = context.Conversations.Where(x => x.ReciverId == ReciverId.Trim()).ToList();
                var conversationsender = context.Conversations.Where(x => x.ReciverId == user.Id).ToList();

                foreach (var conversation in conversations)
                {
                    if (conversation.SenderId == user.Id)
                    {
                        var PreviousMessagesChat = context.Messages.Where(x => x.CoversationId == conversation.Id).ToList();
                        return View(PreviousMessagesChat);                
                    }
                }
                foreach (var bb in conversationsender)
                {
                    if (bb.SenderId == ReciverId)
                    {
                        var PreviousMessagesChat = context.Messages.Where(x => x.CoversationId == bb.Id).ToList();
                        return View(PreviousMessagesChat);
                    }
                }
            }
            TempData["Error"] = "ds";
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Message message)
        {
            ModelState.Remove("Sender");
            ModelState.Remove("UserID");
            ModelState.Remove("UserName");
            ModelState.Remove("Reciver");
            ModelState.Remove("Conversation");
            ModelState.Remove("ReciverId");
            if (ModelState.IsValid)
            {
                var user = await manager.GetUserAsync(User);
                var reciveruser = await manager.FindByIdAsync(message.ReciverId.Trim());
                message.UserName = User.Identity.Name;
                message.When = DateTime.Now;
                message.UserID = user.Id;
               

                var ExistingMessage = context.Conversations.FirstOrDefault(x => x.ReciverId == reciveruser.Id && x.SenderId == user.Id);
                var ExistingMessagea = context.Conversations.FirstOrDefault(x => x.ReciverId == user.Id && x.SenderId == reciveruser.Id);

                if (ExistingMessage == null && ExistingMessagea == null)
                {
                    Conversaton conversaton = new Conversaton();
                    conversaton.SenderId = user.Id;
                    conversaton.CreatedAt = DateTime.Now;
                    conversaton.ReciverId = reciveruser.Id;

                    await context.Conversations.AddAsync(conversaton);
                    await context.SaveChangesAsync();
                    message.CoversationId = conversaton.Id;
                    // send message

                }
                else
                {
                    if (ExistingMessage != null)
                    {
                        message.CoversationId = ExistingMessage.Id;
                    }
                    else
                    {
                        message.CoversationId = ExistingMessagea.Id;
                    }


                }
                var a = await context.Messages.AddAsync(message);
                int i=await context.SaveChangesAsync();
                var ConversationData = await context.Conversations.Include(x => x.Sender).Include(x => x.Reciever).FirstOrDefaultAsync(x=>x.Id==message.CoversationId);
                await hub.Clients.All.SendAsync("msgRecieve", message);
                await hub.Clients.All.SendAsync("UpdateList", ConversationData);


                return Ok();

                // before

                //    var ExistingMessage = context.Messages.FirstOrDefault(x => x.ReciverId == reciveruser.Id && x.UserID == user.Id);
                //    message.UserName = User.Identity.Name;
                //    message.When = DateTime.Now;
                //    message.UserID = user.Id;
                //    // message.CoversationId = ExistingMessage.CoversationId;
                //    if (ExistingMessage != null)
                //    {
                //        message.CoversationId = ExistingMessage.CoversationId;
                //        var a = await context.Messages.AddAsync(message);
                //        await context.SaveChangesAsync();
                //        return Ok();
                //    }
                //    else
                //    {
                //        //creating conversation record for tracking previus conversation having or not.
                //        Conversaton conversaton = new Conversaton();
                //        conversaton.SenderId = user.Id;
                //        conversaton.CreatedAt = DateTime.Now;
                //        conversaton.ReciverId = reciveruser.Id;

                //        await context.Conversations.AddAsync(conversaton);
                //        await context.SaveChangesAsync();
                //        message.CoversationId = conversaton.Id;

                //        //we got the conversaton id so i can pass it with send message object as forign key as reference.
                //        //now adding new message into message list.

                //        var a = await context.Messages.AddAsync(message);
                //        await context.SaveChangesAsync();
                //        return Ok();



                //    }
                //}
                //else
                //{
                //    ViewBag.ModelError = "kindly enter message text";
                //    return BadRequest();
                //    //if model state is not valid then else block run
                //}



                //ModelState.Remove("Sender");
                //ModelState.Remove("UserID");
                //ModelState.Remove("UserName");

                //if (ModelState.IsValid)
                //{
                //    message.UserName = User.Identity.Name;

                //    message.When = DateTime.Now;

                //    var Sender = await manager.GetUserAsync(User);

                //    message.UserID = Sender.Id;

                //    var a = await context.AddAsync(message);

                //    await context.SaveChangesAsync();

                //    //signalR code
                //    Console.WriteLine("Message saved successfully");

                //    await hub.Clients.All.SendAsync("msgRecieve", message);

                //    Console.WriteLine("SignalR broadcast executed");
                //}
                //return Ok();
            }
            else
            {

                ViewBag.ModelError = "kindly enter message text";
                    return BadRequest();
            }
        }



        [HttpGet]
        public IActionResult ViewChat(string RecieverId)//need to pass parameter string for rcvrid
        {

            if (string.IsNullOrWhiteSpace(RecieverId))
            {
                return BadRequest();

            }
            else
            {
                string UserId = manager.GetUserId(User);
                var ExistingConversation = context.Conversations.Single(x => x.ReciverId == RecieverId && x.SenderId == UserId);
                if (ExistingConversation != null)
                {
                    var messages = context.Messages.Where(x => x.CoversationId == ExistingConversation.Id).ToList();
                    return View("Index", messages);
                }
                else
                {

                    return View("index");
                }
            }

            //when user enter reciver id the person wth whome he or she wants to start conversation then in convsersation table check is any previous conversation id generated with this recvr id f yes then chat interface return with all previous conversation 
            //if not then chat interface return and also new conversation entity added into conversation table where genereted conversationid and senderid and reciverid (enter in input box by sender)
            return View("Index");
            //chat iterface view ame like index view


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
