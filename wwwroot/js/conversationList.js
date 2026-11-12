
// ============================================
// 1. SIGNALR CONNECTION
// ============================================

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

console.log("on js of list");


// ============================================
// 2. CURRENT LOGGED-IN USER ID
// ============================================

const currentUserId = document.body.dataset.userId;


// ============================================
// 3. RECEIVE UPDATE LIST EVENT
// ============================================

connection.on("UpdateList", function (conversation) {

    console.log("Conversation list updated:", conversation);

    updateConversationList(conversation, currentUserId);

});


// ============================================
// 4. UPDATE CONVERSATION LIST METHOD
// ============================================

function updateConversationList(conversation, currentUserId) {

    const chatList = document.getElementById("ChatList");

    if (!chatList || !conversation) return;


    // ========================================
    // GET CONVERSATION PROPERTIES
    // ========================================

    const id = conversation.id ?? conversation.Id;

    const sender = conversation.sender ?? conversation.Sender;

    // Your C# property is Reciever
    const receiver = conversation.reciever ?? conversation.Reciever;

    const senderId = conversation.senderId ?? conversation.SenderId;

    const receiverId = conversation.recieverId ?? conversation.ReciverId;

    const createdAt = conversation.createdAt ?? conversation.CreatedAt;


    // ========================================
    // IDENTIFY OTHER USER
    // ========================================

    const otherUser =
        String(senderId) === String(currentUserId)
            ? receiver
            : sender;

    if (!otherUser || !id) return;


    const userId = otherUser.id ?? otherUser.Id;

    const userName =
        otherUser.userName ?? otherUser.UserName ?? "Unknown";


    // ========================================
    // CHECK EXISTING CONVERSATION
    // ========================================

    const matchingRows = chatList.querySelectorAll(
        `[data-conversation-id="${CSS.escape(String(id))}"]`
    );

    let row;

    if (matchingRows.length > 0) {

        // Reuse the first existing row
        row = matchingRows[0];

        // Remove any accidental duplicate rows
        matchingRows.forEach((duplicate, index) => {
            if (index > 0) {
                duplicate.remove();
            }
        });

        // Remove existing row from its current position
        row.remove();

    }
    else {

        // Create a new row only when ID does not exist
        row = document.createElement("a");

        row.className = "conversation-row";

        row.dataset.conversationId = id;

    }


    // ========================================
    // GENERATE INDEX PAGE URL
    // ========================================

    const url = new URL("/Home/Index", window.location.origin);

    url.searchParams.set("Id", id);

    url.searchParams.set("ReciverId", userId);

    row.href = url.pathname + url.search;


    // ========================================
    // CREATE USER AVATAR
    // ========================================

    const avatar = document.createElement("div");

    avatar.className = "user-avatar";

    avatar.textContent = userName.charAt(0).toUpperCase();


    // ========================================
    // CREATE CONVERSATION INFORMATION
    // ========================================

    const info = document.createElement("div");

    info.className = "conversation-info";

    const heading = document.createElement("h5");

    heading.textContent = userName;

    const description = document.createElement("small");

    description.textContent = "New message";

    info.appendChild(heading);

    info.appendChild(description);


    // ========================================
    // CREATE DATE AND TIME
    // ========================================

    const dateContainer = document.createElement("div");

    dateContainer.className = "conversation-date";

    const dateText = document.createElement("span");

    const timeText = document.createElement("small");

    const date = new Date(createdAt);

    if (!isNaN(date.getTime())) {

        dateText.textContent = date.toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        });

        timeText.textContent = date.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit"
        });

    }

    dateContainer.append(dateText, timeText);


    // ========================================
    // UPDATE ROW CONTENT
    // ========================================

    row.replaceChildren(
        avatar,
        info,
        dateContainer
    );


    // ========================================
    // MOVE EXISTING OR NEW ROW TO TOP
    // ========================================

    chatList.prepend(row);


    // ========================================
    // HIGHLIGHT UPDATED CONVERSATION
    // ========================================

    row.classList.remove("new-message-highlight");

    // Restart CSS animation
    void row.offsetWidth;

    row.classList.add("new-message-highlight");

}


// ============================================
// 5. START SIGNALR CONNECTION
// ============================================

async function startConnection() {

    try {

        await connection.start();

        console.log("Conversation List SignalR connected!");

    }
    catch (error) {

        console.error("SignalR connection error:", error);

        setTimeout(startConnection, 5000);

    }

}

startConnection();
