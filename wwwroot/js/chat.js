 
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

const messageForm = document.getElementById("messageForm");
const messageInput = document.getElementById("messageInput");
const chatMessages = document.getElementById("chatMessages");

const input= messageForm.querySelector('input[type="text"]');
const username= input.dataset.username;


// Current logged-in user ID
const currentUserId = document.body.dataset.userId;

// Receive message
connection.on("msgRecieve", function (message) {
    console.log("Message received:", message);
    displayMessage(message);
});

// Display message
function displayMessage(message) {

    const welcome = document.querySelector(".welcome-message");

    if (welcome) {
        welcome.remove();
    }

    const isMine = message.userID === currentUserId;

    const messageRow = document.createElement("div");

    messageRow.className = isMine
        ? "message-row sent"
        : "message-row received";

    const sender = document.createElement("div");
    sender.className = "sender-name";
    sender.textContent = message.userName;

    const bubble = document.createElement("div");
    bubble.className = "message-bubble";
    bubble.textContent = message.text;

    const time = document.createElement("small");
    time.className = "message-time";

    time.textContent = new Date(message.when)
        .toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit"
        });

    messageRow.appendChild(sender);
    messageRow.appendChild(bubble);
    messageRow.appendChild(time);

    chatMessages.appendChild(messageRow);

    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Send message
// messageForm.addEventListener("submit", async function (e) {

//     e.preventDefault();

//     const text = messageInput.value.trim();

//     if (!text) return;

//     try {

//         await connection.invoke("SendMessage", {
//             text: text
//         });

//         messageInput.value = "";
//         messageInput.focus();

//     }
//     catch (error) {

//         console.error("Message sending error:", error);

//     }

// });
messageForm.addEventListener("submit", async function (e) {

    e.preventDefault();

    const text = messageInput.value.trim();

    if (!text) return;

    try {

        const response = await fetch('/Home/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                text: text           
            })
        });

        if (!response.ok) {
            throw new Error("Message sending failed");
        }

        messageInput.value = "";
        messageInput.focus();

    }
    catch (error) {
        console.error(error);
    }
});

// Start connection
async function startConnection() {

    try {

        await connection.start();

        console.log("SignalR connected!");

    }
    catch (error) {

        console.error(error);

        setTimeout(startConnection, 5000);
    }
}

startConnection();




