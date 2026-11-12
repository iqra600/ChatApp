
// ============================================
// 1. SIGNALR CONNECTION
// ============================================

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();


// ============================================
// 2. INDEX PAGE ELEMENTS
// ============================================

const messageForm = document.getElementById("messageForm");
const messageInput = document.getElementById("messageInput");
const chatMessages = document.getElementById("chatMessages");

const username = messageInput?.dataset.username;
const ReciverID = messageInput?.dataset.reciverid;

// Current logged-in user ID
const currentUserId = document.body.dataset.userId;


// ============================================
// 3. RECEIVE MESSAGE EVENT
// ============================================

connection.on("msgRecieve", function (message) {

    console.log("Message received:", message);

    displayMessage(message);
});


// ============================================
// 4. DISPLAY MESSAGE METHOD
// ============================================

function displayMessage(message) {

    
    // Remove welcome message
    const welcome = document.querySelector(".welcome-message");

    if (welcome) {
        welcome.remove();
    }

    // Check whether message belongs to logged-in user
    const messageUserId = message.userID ?? message.UserID;

    const isMine =
        String(messageUserId) === String(currentUserId);

    // Message row
    const messageRow = document.createElement("div");

    messageRow.className = isMine
        ? "message-row sent"
        : "message-row received";

    // Sender name
    const sender = document.createElement("div");

    sender.className = "sender-name";

    sender.textContent = message.userName ?? message.UserName;

    // Message text
    const bubble = document.createElement("div");

    bubble.className = "message-bubble";

    bubble.textContent = message.text ?? message.Text;

    // Message time
    const time = document.createElement("small");

    time.className = "message-time";

    const messageDate = message.when ?? message.When;

    time.textContent = new Date(messageDate)
        .toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit"
        });

    // Append elements
    messageRow.appendChild(sender);
    messageRow.appendChild(bubble);
    messageRow.appendChild(time);

    chatMessages.appendChild(messageRow);

    // Scroll window to bottom after new message
    scrolldown();

    // Scroll to latest message
    chatMessages.scrollTop = chatMessages.scrollHeight;

}
window.addEventListener("load", scrollToBottom);
function scrolldown() {
    requestAnimationFrame(() => {
        window.scrollTo({
            top: document.documentElement.scrollHeight,
            behavior: "smooth"
        });
    });
}






// ============================================
// SCROLL TO LAST MESSAGE ON PAGE LOAD
// ============================================


function scrollToBottom() {
    window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: "smooth"
    });
}


// ============================================
// 5. SEND MESSAGE METHOD
// ============================================


    messageForm.addEventListener("submit", async function (e) {

        e.preventDefault();

        const text = messageInput.value.trim();

        if (!text) return;

        try {

            const response = await fetch("/Home/Create", {

                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({

                    text: text,

                    ReciverId: ReciverID

                })
            });

            if (!response.ok) {

                throw new Error("Message sending failed");

            }

            messageInput.value = "";

            messageInput.focus();

        }
        catch (error) {

            console.error("Message sending error:", error);

        }
    });



// ============================================
// 6. START SIGNALR CONNECTION
// ============================================

async function startConnection() {

    try {

        await connection.start();

        console.log("Index SignalR connected!");

    }
    catch (error) {

        console.error("SignalR connection error:", error);

        setTimeout(startConnection, 5000);

    }
}

startConnection();




















