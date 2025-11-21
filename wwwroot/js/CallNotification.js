document.addEventListener("DOMContentLoaded", () => {
    // Inject CSS styles
    const styles = document.createElement("style")
    styles.textContent = `
    @keyframes fade-in {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }

    @keyframes slide-in {
      from {
        transform: translateX(100%);
      }
      to {
        transform: translateX(0);
      }
    }

    @keyframes pulse {
      0% {
        transform: scale(1);
      }
      50% {
        transform: scale(1.05);
      }
      100% {
        transform: scale(1);
      }
    }

    .animate-fade-in {
      animation: fade-in 0.3s ease-out;
    }

    .animate-slide-in {
      animation: slide-in 0.4s ease-out;
    }

    .animate-pulse {
      animation: pulse 1.5s infinite;
    }

    /* Utility classes for styling */
    .fixed { position: fixed; }
    .top-0 { top: 0; }
    .left-0 { left: 0; }
    .bottom-4 { bottom: 1rem; }
    .right-4 { right: 1rem; }
    .w-full { width: 100%; }
    .h-full { height: 100%; }
    .w-80 { width: 20rem; }
    .w-20 { width: 5rem; }
    .h-20 { height: 5rem; }
    .w-5 { width: 1.25rem; }
    .h-5 { height: 1.25rem; }
    .w-6 { width: 1.5rem; }
    .h-6 { height: 1.5rem; }
    .max-w-md { max-width: 28rem; }
    .flex { display: flex; }
    .flex-1 { flex: 1 1 0%; }
    .flex-col { flex-direction: column; }
    .flex-shrink-0 { flex-shrink: 0; }
    .items-center { align-items: center; }
    .items-start { align-items: flex-start; }
    .justify-center { justify-content: center; }
    .overflow-hidden { overflow: hidden; }
    .rounded-lg { border-radius: 0.5rem; }
    .rounded-full { border-radius: 9999px; }
    .rounded-md { border-radius: 0.375rem; }
    .border { border-width: 1px; }
    .border-t { border-top-width: 1px; }
    .border-transparent { border-color: transparent; }
    .border-gray-200 { border-color: rgba(229, 231, 235, 1); }
    .border-gray-300 { border-color: rgba(209, 213, 219, 1); }
    .bg-white { background-color: white; }
    .bg-black { background-color: black; }
    .bg-blue-600 { background-color: rgba(37, 99, 235, 1); }
    .bg-red-100 { background-color: rgba(254, 226, 226, 1); }
    .bg-green-100 { background-color: rgba(220, 252, 231, 1); }
    .bg-opacity-50 { background-color: rgba(0, 0, 0, 0.5); }
    .p-4 { padding: 1rem; }
    .p-6 { padding: 1.5rem; }
    .p-2 { padding: 0.5rem; }
    .py-3 { padding-top: 0.75rem; padding-bottom: 0.75rem; }
    .px-4 { padding-left: 1rem; padding-right: 1rem; }
    .px-3 { padding-left: 0.75rem; padding-right: 0.75rem; }
    .py-2 { padding-top: 0.5rem; padding-bottom: 0.5rem; }
    .text-center { text-align: center; }
    .text-white { color: white; }
    .text-gray-500 { color: rgba(107, 114, 128, 1); }
    .text-gray-400 { color: rgba(156, 163, 175, 1); }
    .text-gray-700 { color: rgba(55, 65, 81, 1); }
    .text-red-600 { color: rgba(220, 38, 38, 1); }
    .text-green-600 { color: rgba(22, 163, 74, 1); }
    .text-blue-600 { color: rgba(37, 99, 235, 1); }
    .text-lg { font-size: 1.125rem; }
    .text-sm { font-size: 0.875rem; }
    .text-xl { font-size: 1.25rem; }
    .font-semibold { font-weight: 600; }
    .font-bold { font-weight: 700; }
    .font-medium { font-weight: 500; }
    .leading-4 { line-height: 1rem; }
    .shadow-lg { box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05); }
    .shadow-sm { box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05); }
    .ring-4 { box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.5); }
    .ring-blue-500 { --tw-ring-color: rgba(59, 130, 246, 1); }
    .z-50 { z-index: 50; }
    .ml-3 { margin-left: 0.75rem; }
    .ml-4 { margin-left: 1rem; }
    .mr-2 { margin-right: 0.5rem; }
    .mt-1 { margin-top: 0.25rem; }
    .mt-2 { margin-top: 0.5rem; }
    .mb-4 { margin-bottom: 1rem; }
    .inline-flex { display: inline-flex; }
    .sr-only { position: absolute; width: 1px; height: 1px; padding: 0; margin: -1px; overflow: hidden; clip: rect(0, 0, 0, 0); white-space: nowrap; border-width: 0; }
    .object-cover { object-fit: cover; }
    .transition-colors { transition-property: color, background-color, border-color, text-decoration-color, fill, stroke; transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1); transition-duration: 150ms; }
    .duration-200 { transition-duration: 200ms; }
    .hover\\:bg-red-200:hover { background-color: rgba(254, 202, 202, 1); }
    .hover\\:bg-green-200:hover { background-color: rgba(187, 247, 208, 1); }
    .hover\\:bg-blue-700:hover { background-color: rgba(29, 78, 216, 1); }
    .hover\\:bg-gray-50:hover { background-color: rgba(249, 250, 251, 1); }
    .hover\\:text-gray-500:hover { color: rgba(107, 114, 128, 1); }
    .focus\\:outline-none:focus { outline: 2px solid transparent; outline-offset: 2px; }
    .focus\\:ring-2:focus { box-shadow: 0 0 0 2px var(--tw-ring-color); }
    .focus\\:ring-offset-2:focus { box-shadow: 0 0 0 2px white; }
    .focus\\:ring-blue-500:focus { --tw-ring-color: rgba(59, 130, 246, 1); }

    /* Dark mode */
    @media (prefers-color-scheme: dark) {
      .dark\\:bg-gray-800 { background-color: rgba(31, 41, 55, 1); }
      .dark\\:border-gray-700 { border-color: rgba(55, 65, 81, 1); }
      .dark\\:text-white { color: white; }
      .dark\\:text-gray-400 { color: rgba(156, 163, 175, 1); }
    }
  `
    document.head.appendChild(styles)

    // Get userId from hidden input or data attribute
    const userIdElement = document.getElementById("user_id") || document.querySelector("[data-user-id]");
    const userId = userIdElement?.value || userIdElement?.getAttribute("data-user-id");

    if (!userId) {
        console.error("❌ User ID not found!");
        return;
    }

    console.log("User ID:", userId)

    let connection = null
    let callTimer = null
    let activeCallPopup = null

    function initializeSignalR() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .build();

        connection.start().then(function () {
            console.log("✅ SignalR connected for call notifications");

            // Join user channel
            connection.invoke("JoinUserChannel", userId);

            // Listen for incoming calls
            connection.on("IncomingCall", function (data) {
                console.log("📞 Incoming call received:", data);
                showCallNotification(data.roomID, data.callerId);
            });

            // Listen for call accepted
            connection.on("CallAccepted", function (data) {
                console.log("✅ Call accepted:", data);
                // Handle call accepted if needed
            });

            // Listen for call rejected  
            connection.on("CallRejected", function (data) {
                console.log("❌ Call rejected:", data);
                // Handle call rejected if needed
            });

        }).catch(function (err) {
            console.error("❌ SignalR connection error:", err);
        });
    }

    function showCallNotification(roomID, callerID) {
        // Clear any existing call popups and timers
        if (activeCallPopup) {
            activeCallPopup.remove()
            clearTimeout(callTimer)
        }

        fetch(`/api/user/${callerID}`)
            .then((response) => response.json())
            .then((user) => {
                const callerName = `${user.first_name || ""} ${user.last_name || ""}`.trim() || "Unknown"
                displayCallPopup(roomID, callerID, callerName, user.avatar_path)
            })
            .catch((error) => {
                console.error("❌ Fetch error:", error)
                displayCallPopup(roomID, callerID, "Unknown")
            })
    }

    function displayCallPopup(roomID, callerID, callerName, profileImage = null) {
        // Create call popup container
        const callDialog = document.createElement("div")
        callDialog.id = "call-popup-container"

        // Default avatar if no profile image
        const avatarUrl = profileImage || "/placeholder.svg?height=80&width=80"

        callDialog.innerHTML = `
            <div id="call-popup" class="fixed top-0 left-0 w-full h-full flex items-center justify-center z-50 bg-black bg-opacity-50">
                <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg overflow-hidden w-80 max-w-md animate-fade-in">
                    <!-- Call header -->
                    <div class="bg-blue-600 text-white p-4 text-center">
                        <h3 class="text-lg font-semibold">Incoming Call</h3>
                        <div class="call-timer mt-1 text-sm" id="call-timer">00:30</div>
                    </div>
                    
                    <!-- Caller info -->
                    <div class="p-6 flex flex-col items-center">
                        <div class="w-20 h-20 rounded-full overflow-hidden mb-4 ring-4 ring-blue-500 animate-pulse">
                            <img src="${avatarUrl}" alt="${callerName}" class="w-full h-full object-cover">
                        </div>
                        <h4 class="text-xl font-bold dark:text-black">${callerName}</h4>
                        <p class="text-gray-500 dark:text-black-400">is calling</p>
                    </div>
                    
                    <!-- Call actions -->
                    <div class="flex border-t border-gray-200 dark:border-gray-700">
                        <button id="decline-call" class="flex-1 py-3 px-4 bg-red-100 hover:bg-red-200 text-red-600 font-medium transition-colors duration-200">
                            <div class="flex items-center justify-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                    <line x1="18" y1="6" x2="6" y2="18"></line>
                                    <line x1="6" y1="6" x2="18" y2="18"></line>
                                </svg>
                                Decline
                            </div>
                        </button>
                        <button id="accept-call" class="flex-1 py-3 px-4 bg-green-100 hover:bg-green-200 text-green-600 font-medium transition-colors duration-200">
                            <div class="flex items-center justify-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                    <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
                                </svg>
                                Accept
                            </div>
                        </button>
                    </div>
                </div>
            </div>
        `

        document.body.appendChild(callDialog)
        activeCallPopup = callDialog

        // Add event listeners for buttons
        document.getElementById("accept-call").addEventListener("click", () => {
            clearTimeout(callTimer)
            // Notify caller that call was accepted
            if (connection) {
                connection.invoke("AcceptCall", parseInt(callerID), parseInt(userId), parseInt(roomID));
            }
            // Open video call
            window.open(`/video-call?room=${roomID}&user=${userId}`, "_blank")
            callDialog.remove()
            activeCallPopup = null
        })

        document.getElementById("decline-call").addEventListener("click", () => {
            clearTimeout(callTimer)
            // Notify caller that call was rejected
            if (connection) {
                connection.invoke("RejectCall", parseInt(callerID), parseInt(userId), parseInt(roomID));
            }
            callDialog.remove()
            activeCallPopup = null
        })

        // Start 30-second timer
        let timeLeft = 30
        const timerElement = document.getElementById("call-timer")

        function updateTimer() {
            timeLeft -= 1
            const seconds = timeLeft % 60
            timerElement.textContent = `00:${seconds < 10 ? "0" : ""}${seconds}`

            if (timeLeft > 0) {
                setTimeout(updateTimer, 1000)
            }
        }

        updateTimer()

        // Set timeout for missed call notification
        callTimer = setTimeout(() => {
            callDialog.remove()
            activeCallPopup = null
            showMissedCallNotification(callerName, callerID)
        }, 30000)
    }

    function showMissedCallNotification(callerName, callerID) {
        const missedCallNotif = document.createElement("div")
        missedCallNotif.id = "missed-call-notification"
        missedCallNotif.innerHTML = `
            <div class="fixed bottom-4 right-4 bg-white dark:bg-gray-800 rounded-lg shadow-lg p-4 w-80 animate-slide-in z-50">
                <div class="flex items-start">
                    <div class="flex-shrink-0 bg-red-100 rounded-full p-2">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                    </div>
                    <div class="ml-3 w-0 flex-1">
                        <p class="text-sm font-medium text-gray-900 dark:text-white">Missed Call</p>
                        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            You missed a call from <strong>${callerName}</strong>
                        </p>
                        <div class="mt-2 flex">
                            <button id="call-back-btn" class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                                Call back
                            </button>
                            <button id="dismiss-missed-call" class="ml-3 inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                                Dismiss
                            </button>
                        </div>
                    </div>
                    <div class="ml-4 flex-shrink-0 flex">
                        <button id="close-missed-call" class="bg-white dark:bg-gray-800 rounded-md inline-flex text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                            <span class="sr-only">Close</span>
                            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `

        document.body.appendChild(missedCallNotif)

        // Add event listeners
        document.getElementById("call-back-btn").addEventListener("click", () => {
            // Generate new room ID for callback
            let roomID = Math.floor(Math.random() * 10000) + "";
            fetch(`/calls/create/${userId}/${callerID}/${roomID}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.roomID) {
                        window.open(`/video-call?room=${data.roomID}&user=${userId}`, "_blank");
                    }
                })
                .catch(error => console.error("Error calling back:", error));
            missedCallNotif.remove()
        })

        document.getElementById("dismiss-missed-call").addEventListener("click", () => {
            missedCallNotif.remove()
        })

        document.getElementById("close-missed-call").addEventListener("click", () => {
            missedCallNotif.remove()
        })

        // Auto-dismiss after 10 seconds
        setTimeout(() => {
            if (document.getElementById("missed-call-notification")) {
                missedCallNotif.remove()
            }
        }, 10000)
    }

    // Initialize SignalR
    initializeSignalR()
})