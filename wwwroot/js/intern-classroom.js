// Global variables
var user_id = document.getElementById("user_id").value;
let internId = null;
let classId = null;

// Hàm hiển thị thông báo toàn cục
function showGlobalAlert(message, type = 'danger') {
    const alertContainer = document.getElementById('globalAlertContainer');
    if (!alertContainer) {
        console.error('Global alert container not found!');
        return;
    }

    // Ensure the container has the highest z-index
    alertContainer.style.zIndex = '9999';

    // Generate a unique ID for the alert
    const alertId = 'alert-' + Date.now();

    // Set icon based on type
    let icon = 'bi-exclamation-triangle-fill';
    if (type === 'success') icon = 'bi-check-circle-fill';
    else if (type === 'warning') icon = 'bi-exclamation-circle-fill';
    else if (type === 'info') icon = 'bi-info-circle-fill';

    // Create the alert HTML
    const alertHtml = `
    <div id="${alertId}" class="alert alert-${type} alert-dismissible fade show" role="alert" style="border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); border-left: 4px solid;">
      <i class="bi ${icon} me-2"></i>
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>
  `;

    // Append the alert to the container
    alertContainer.innerHTML += alertHtml;

    // Set border-left color based on type
    const alertElement = document.getElementById(alertId);
    if (alertElement) {
        const borderColor = {
            'danger': '#dc3545',
            'warning': '#ffc107',
            'success': '#28a745',
            'info': '#17a2b8'
        }[type] || '#dc3545';
        alertElement.style.borderLeftColor = borderColor;

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alertElement);
            bsAlert.close();
        }, 5000);

        // Scroll to the alert
        alertContainer.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
}


// Initialize the page
document.addEventListener("DOMContentLoaded", function() {
    // Get the intern ID from the hidden input
    const userIdElement = document.getElementById("user_id");
    const classroom =document.getElementById("class_Id")
    if (userIdElement && userIdElement.value) {
        internId = userIdElement.value;
        classId = classroom.value;
        loadClassData(classId);
    } else {
        // Fallback to example ID for testing
        internId = "203";

        // Load the intern's class with mock data
        loadInternClass();
    }
    fixTaskListDisplay();

    // Also call after a short delay to ensure all content is loaded
    setTimeout(fixTaskListDisplay, 1000);

    // Call again after tasks are loaded
    const originalFetchTasks = window.fetchTasks;
    if (originalFetchTasks) {
        window.fetchTasks = (classId) => {
            originalFetchTasks(classId).then(() => {
                setTimeout(fixTaskListDisplay, 500);
            });
        };
    }
    // Set up event listeners
    const sendMessageBtn = document.getElementById("sendMessageBtn");
    if (sendMessageBtn) {
        sendMessageBtn.addEventListener("click", sendMessage);
    }

    const messageInput = document.getElementById("message");
    if (messageInput) {
        messageInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") {
                e.preventDefault();
                sendMessage();
            }
        });
        ensureModalZIndex();
    }
    function ensureModalZIndex() {
        // Set global alert container to highest z-index
        const alertContainer = document.getElementById('globalAlertContainer');
        if (alertContainer) {
            alertContainer.style.zIndex = '1060';
        }

        // Adjust modal backdrop z-index
        const style = document.createElement('style');
        style.innerHTML = `
      .modal-backdrop {
        z-index: 1040 !important;
      }
      .modal {
        z-index: 1050 !important;
      }
      #globalAlertContainer {
        z-index: 9999 !important;
      }
    `;
        document.head.appendChild(style);
    }

    const submissionForm = document.getElementById("submission-form");
    if (submissionForm) {
        submissionForm.addEventListener("submit", (e) => {
            e.preventDefault();
            submitTask();
        });
    }

    // Add this new function call
    fixModalZIndexes();

    // Existing code...
});

// Add this new function
function fixModalZIndexes() {
    // Add a style element to ensure proper z-index hierarchy
    const style = document.createElement('style');
    style.textContent = `
    .modal-backdrop {
      z-index: 1040 !important;
    }
    .modal {
      z-index: 1050 !important;
    }
    #globalAlertContainer {
      z-index: 9999 !important;
    }
  `;
    document.head.appendChild(style);

    // Also set the z-index directly on the container element
    const alertContainer = document.getElementById('globalAlertContainer');
    if (alertContainer) {
        alertContainer.style.zIndex = '9999';
    }
}

// Function to load students for a class
function loadStudents(classId) {


    fetch(`/api/class/${classId}/users`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Không thể lấy danh sách sinh viên từ server');
            }
            return response.json();
        })
        .then(students => {

            displayStudents(students);
        })
        .catch(error => {
            console.error("Lỗi khi lấy danh sách sinh viên:", error);
            // Fall back to mock data if API fails
        });
}

// Function to fetch tasks for the class
async function fetchTasks(classId) {
    try {

        const taskList = document.getElementById("task-list");
        // Show loading indicator in the task list
        if (taskList) {
            taskList.innerHTML = `
                <div class="d-flex align-items-center">
                    <div class="spinner-border spinner-border-sm text-warning me-2" role="status"></div>
                    <span>Đang tải danh sách task...</span>
                </div>
            `;
        }

        // Fetch tasks from API
        const response = await fetch(`/api/class/task/${classId}`);

        // Check if the request was successful
        if (!response.ok) {
            throw new Error(`Failed to fetch tasks: ${response.status} ${response.statusText}`);
        }

        // Parse the JSON response
        const tasks = await response.json();


        // Render the tasks
        renderTasksNewDesign(tasks);

    } catch (error) {
        console.error("Error fetching tasks:", error);
        const taskList = document.getElementById("task-list");
        if (taskList) {
            taskList.innerHTML = `
                <div class="alert alert-danger">
                    Không thể tải danh sách task. Vui lòng thử lại sau.
                </div>
            `;
        }

        // Fall back to mock data if API fails
        setTimeout(() => {
            renderTasksNewDesign(mockTasks);
        }, 1000);
    }
}

// Function to fix the task list display
function fixTaskListDisplay() {
    const taskList = document.getElementById("task-list");
    const contentCard = document.querySelector(".content-card");
    const classDetails = document.getElementById("classDetails");

    if (taskList && contentCard && classDetails) {
        // Remove fixed height constraints
        contentCard.style.maxHeight = "none";
        contentCard.style.height = "auto";
        contentCard.style.overflow = "visible";

        // Ensure class details has proper height
        classDetails.style.minHeight = "600px";
        classDetails.style.height = "auto";

        // Ensure task list has proper scrolling
        taskList.style.maxHeight = "500px";
        taskList.style.overflowY = "auto";
        taskList.style.paddingBottom = "30px";

        // Add a spacer at the end of the task list if it doesn't exist
        if (!taskList.querySelector('.task-list-spacer')) {
            const spacer = document.createElement("div");
            spacer.className = "task-list-spacer";
            spacer.style.height = "20px";
            spacer.style.width = "100%";
            taskList.appendChild(spacer);
        }

    }
}

// Function to render tasks with the new design


function renderTasksNewDesign(tasks) {
    const taskListElement = document.getElementById("task-list");
    if (!taskListElement) {
        console.error("Task list element not found");
        return;
    }

    taskListElement.innerHTML = "";

    if (!tasks || tasks.length === 0) {
        taskListElement.innerHTML = '<div class="text-muted">No Tasks Available now</div>';
        return;
    }

    // Create a wrapper div to ensure proper spacing
    const tasksWrapper = document.createElement("div");
    tasksWrapper.className = "tasks-wrapper";

    // Process each task
    tasks.forEach(task => {
        try {
            // Format dates
            const startTime = new Date(task.startTime);
            const endTime = new Date(task.endTime);
            const currentTime = new Date();

            // Check if task has expired
            const isExpired = currentTime > endTime;

            const formattedStartTime = startTime.toLocaleString("vi-VN", {
                hour: '2-digit',
                minute: '2-digit',
                day: '2-digit',
                month: '2-digit',
                year: 'numeric'
            });

            const formattedEndTime = endTime.toLocaleString("vi-VN", {
                hour: '2-digit',
                minute: '2-digit',
                day: '2-digit',
                month: '2-digit',
                year: 'numeric'
            });

            // Create task element
            const taskElement = document.createElement("div");
            taskElement.className = `task-item ${isExpired ? 'expired' : ''}`;
            taskElement.dataset.taskId = task.id;
            taskElement.dataset.expired = isExpired;

            // Add expired indicator if needed
            let statusIndicator = '';
            if (isExpired) {
                statusIndicator = '<div class="expired-indicator">No more Available</div>';
            }

            // Add the task content first (we'll check submission status later)
            taskElement.innerHTML = `
        ${statusIndicator}
        <div class="task-title">${task.taskName || 'Unnamed Task'}</div>
        <div class="task-description">${task.description || 'No Description'}</div>
        <div class="task-meta">
          <div>Start At: ${formattedStartTime}</div>
          <div class="task-deadline">Deadline: ${formattedEndTime}</div>
        </div>
      `;

            // Add click event to open task detail modal
            taskElement.addEventListener("click", () => {
                openTaskDetailModal(task);
            });

            // Add to the wrapper
            tasksWrapper.appendChild(taskElement);

            // Now check submission status asynchronously and update the task element if needed
            if (!isExpired) {
                // Use a self-executing async function to handle the submission check
                (async () => {
                    try {
                        // Load submission history for this task
                        const submission = await loadSubmissionHistory2(task.id);

                        if (submission) {
                            let notificationHTML = '';

                            // Check if there's a comment but no mark
                            if (submission.comment && submission.mark === null) {
                                notificationHTML = `
                  <div class="expired-indicator" style="
                    background-color: #f38321;
                    color: white;
                  ">You have new comment</div>`;
                            }
                            // Check if there's a mark (task has been graded)
                            else if (submission.mark !== null && submission.mark !== undefined) {
                                notificationHTML = `
                  <div class="expired-indicator" style="
                    background-color: #2ecc71;
                    color: white;
                  ">Your mark has been graded</div>`;
                            }

                            // If we have a notification, add it to the task element
                            if (notificationHTML) {
                                // Insert at the beginning of the task element
                                taskElement.insertAdjacentHTML('afterbegin', notificationHTML);
                            }
                        }
                    } catch (error) {
                        console.error(`Error checking submission for task ${task.id}:`, error);
                    }
                })();
            }
        } catch (error) {
            console.error("Error rendering task:", error, task);
        }
    });

    // Add the wrapper to the task list
    taskListElement.appendChild(tasksWrapper);

    // Add a spacer element at the end to ensure the last task is fully visible
    const spacer = document.createElement("div");
    spacer.style.height = "20px";
    taskListElement.appendChild(spacer);
}

// Add CSS for the notification styles if not already present
function addNotificationStyles() {
    // Check if styles already exist
    if (!document.getElementById('task-notification-styles')) {
        const style = document.createElement('style');
        style.id = 'task-notification-styles';
        style.textContent = `
      .expired-indicator {
          position: absolute;
          top: -10px;
          right: 10px;
          background-color: #dc3545;
          color: white;
          padding: 4px 12px;
          border-radius: 12px;
          font-size: 14px;
          font-weight: bold;
          z-index: 1;
          box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
      }

      @keyframes pulse {
        0% {
          transform: scale(1);
          opacity: 1;
        }
        50% {
          transform: scale(1.1);
          opacity: 0.8;
        }
        100% {
          transform: scale(1);
          opacity: 1;
        }
      }
    `;
        document.head.appendChild(style);
    }
}

// Call this function when the page loads
addNotificationStyles();

// Function to load the intern's class with mock data
function loadInternClass() {


    // Get the intern ID from the hidden input
    const classroom = document.getElementById("class_Id");
    const userIdElement = document.getElementById("user_id");
    if (userIdElement && userIdElement.value) {
        internId = userIdElement.value;
        classId = classroom.value;

    } else {
        console.error("No intern ID found in input");
        const errorMessage = document.getElementById("error-message");
        if (errorMessage) {
            errorMessage.style.display = "block";
            errorMessage.textContent = "Không tìm thấy ID thực tập sinh. Vui lòng đăng nhập lại.";
        }
    }
}

function loadClassData(classId) {
    // Show loading indicator
    const loadingIndicator = document.getElementById("loading-indicator");
    const errorMessage = document.getElementById("error-message");
    const classContent = document.getElementById("class-content");

    if (loadingIndicator) loadingIndicator.style.display = "block";
    if (errorMessage) errorMessage.style.display = "none";
    if (classContent) classContent.style.display = "none";

    fetch(`/api/class/${classId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Không thể lấy dữ liệu lớp học từ server');
            }
            return response.json();
        })
        .then(classData => {
            if (classData && classData.conversation && classData.conversation.id) {
                conversationId = classData.conversation.id;
            } else {
                console.warn("No conversation ID found in class data");
                conversationId = "default-conversation";
            }

            // Display class details
            displayClassDetails(classData);

            // Load students
            loadStudents(classId);

            // Load tasks
            fetchTasks(classId);

            // Load chat messages bằng cách gọi loadClassroomConversation với dữ liệu thực tế
            loadClassroomConversation(classData.id, classData.className);

            // Hide loading indicator and show content
            if (loadingIndicator) loadingIndicator.style.display = "none";
            if (classContent) classContent.style.display = "block";
        })
        .catch(error => {
            console.error("Lỗi khi lấy dữ liệu lớp học:", error);
            // Xử lý lỗi mà không quay lại mock data
            if (loadingIndicator) loadingIndicator.style.display = "none";
            if (errorMessage) {
                errorMessage.style.display = "block";
                errorMessage.textContent = "Không thể tải dữ liệu lớp học. Vui lòng thử lại sau.";
            }
        });
}

// Function to display class details
function displayClassDetails(classData) {
    const className = document.getElementById("className");
    const classTime = document.getElementById("classTime");
    const mentorName = document.getElementById("mentorName");
    const mentorAvatar = document.getElementById("mentorAvatar");
    const mentorInitials = document.getElementById("mentorInitials");
    const chatTitle = document.getElementById("chatTitle");

    if (className) className.textContent = classData.className || "Unnamed Class";

    // Format date and time
    if (classTime) {
        try {
            const createdAt = new Date(classData.createdAt);
            const formattedDate = createdAt.toLocaleDateString("vi-VN", { day: '2-digit', month: '2-digit', year: 'numeric' });
            const formattedTime = createdAt.toLocaleTimeString("vi-VN", { hour: '2-digit', minute: '2-digit' });
            classTime.textContent = `${formattedDate} - ${formattedTime}`;
        } catch (error) {
            console.error("Error formatting date:", error);
            classTime.textContent = "Invalid Date";
        }
    }

    // Set mentor information
    if (mentorName && classData.manager) {
        const mentorFullName = `${classData.manager.first_name || ''} ${classData.manager.last_name || ''}`.trim();
        mentorName.textContent = mentorFullName || "Unknown Mentor";
    }

    // Get mentor ID and fetch avatar
    if (classData.manager && classData.manager.id) {
        const mentorId = classData.manager.id;

        // Fetch mentor avatar using the mentor ID
        fetchMentorAvatar(mentorId, mentorAvatar, mentorInitials);
    } else {
        // If no mentor ID, show initials
        if (mentorInitials && classData.manager) {
            const firstInitial = classData.manager.first_name ? classData.manager.first_name.charAt(0) : '';
            const lastInitial = classData.manager.last_name ? classData.manager.last_name.charAt(0) : '';
            mentorInitials.textContent = (firstInitial + lastInitial) || "?";
            mentorInitials.style.display = "block";
        }
    }

    // Set chat title
    if (chatTitle) chatTitle.textContent = classData.className || "Class Chat";
}

// fetch Mentor avatar
function fetchMentorAvatar(mentorId, avatarElement, initialsElement) {
    if (!mentorId || !avatarElement || !initialsElement) {
        console.error("Missing required parameters for fetchMentorAvatar");
        return;
    }

    // Make API call to get user details including avatar
    fetch(`/api/user/${mentorId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch mentor details');
            }
            return response.json();
        })
        .then(userData => {
            // Check if avatar URL exists
            if (userData && userData.avatar_path) {
                // Create and set image
                const img = document.createElement('img');
                img.src = userData.avatar_path;
                img.alt = "Mentor Avatar";
                img.style.width = "7%";
                img.style.height = "7%";
                img.style.objectFit = "cover";
                img.style.borderRadius = "50%"; // Make the image itself circular
                img.onerror = function() {
                    // If image fails to load, show initials instead
                    this.style.display = 'none';
                    initialsElement.style.display = 'block';
                };

                // Clear avatar container and add image
                avatarElement.innerHTML = '';
                avatarElement.appendChild(img);
                initialsElement.style.display = 'none';
            } else {
                // If no avatar, ensure initials are visible
                initialsElement.style.display = 'block';
            }
        })
        .catch(error => {
            console.error("Error fetching mentor avatar:", error);
            // Show initials as fallback
            initialsElement.style.display = 'block';
        });
}

// Function to display students
function displayStudents(students) {
    const studentContainer = document.getElementById("student-container");
    if (!studentContainer) {
        console.error("Student container element not found");
        return;
    }

    studentContainer.innerHTML = "";

    if (!students || students.length === 0) {
        studentContainer.innerHTML = '<div class="text-muted">Không có sinh viên nào trong lớp</div>';
        return;
    }

    students.forEach(student => {
        try {
            const studentBadge = document.createElement("div");
            studentBadge.className = "student-badge";

            // Format the student name
            const studentName = `${student.first_name || ''} ${student.last_name || ''}`.trim();
            studentBadge.textContent = studentName || "Unnamed Student";

            // Highlight the current intern
            if (student.id === internId) {
                studentBadge.style.backgroundColor = "#e3f2fd";
                studentBadge.style.borderColor = "#007bff";
                studentBadge.style.fontWeight = "bold";
            }

            studentContainer.appendChild(studentBadge);
        } catch (error) {
            console.error("Error rendering student:", error, student);
        }
    });
}

async function loadClassroomConversation(classId, className) {
    try {
        // Fetch class details to get conversation_id
        const classResponse = await fetch(`/api/class/${classId}`);
        if (!classResponse.ok) {
            throw new Error("Failed to fetch class details");
        }
        const classData = await classResponse.json();

        // Get chat container
        const chatContainer = document.getElementById("chatContainer");

        // Set up chat container with header and message area
        chatContainer.innerHTML = `
            <div class="chat-box">
                <!-- Chat Header -->
                <div class="chat-header p-3 border-bottom">
                    <h5 class="mb-0">${className}</h5>
                </div>

                <!-- Chat Messages -->
                <div class="chat-container" id="chat">
                    <div class="text-center p-4">
                        <div class="spinner-border text-warning" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </div>
                </div>

                <!-- Chat Input -->
                <div class="chat-input">
                    <div class="input-group">
                        <input type="text" class="form-control" id="message" placeholder="Enter...">
                        <button class="btn btn-warning" onclick="sendMessage()">
                            <i class="bi bi-send"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        // Set conversation ID and name
        conversationId = classData.conversation.id;
        conversationName = className;

        await loadMember(conversationId);
        // Load existing messages
        await loadMessages(conversationId);

        // Set up message input and send button
        const messageInput = chatContainer.querySelector('#message');
        const sendButton = chatContainer.querySelector('.btn-warning');

        // Add event listeners
        messageInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                sendMessage();
            }
        });

        sendButton.addEventListener('click', function() {
            sendMessage();
        });

    } catch (error) {
        console.error("Error loading classroom conversation:", error);
        document.getElementById("chatContainer").innerHTML = `
            <div class="alert alert-danger">
                Không thể tải tin nhắn. Vui lòng thử lại sau.
            </div>
        `;
    }
}

// Function to open task detail modal
function openTaskDetailModal(task) {
    const taskName = document.getElementById("task-name");
    const taskDescription = document.getElementById("task-description");
    const taskStartTime = document.getElementById("task-start-time");
    const taskEndTime = document.getElementById("task-end-time");
    const fileContainer = document.getElementById("task-file-container");
    const fileName = document.getElementById("task-file-name");
    const fileLink = document.getElementById("task-file-link");
    const taskDetailModal = document.getElementById("taskDetailModal");
    const submitButton = document.getElementById("submit-task-btn");
    const taskLinkContainer = document.getElementById("task-link-container");
    const taskLink = document.getElementById("task-link");

    // Kiểm tra các phần tử HTML
    if (
        !taskName ||
        !taskDescription ||
        !taskStartTime ||
        !taskEndTime ||
        !fileContainer ||
        !fileName ||
        !fileLink ||
        !taskDetailModal ||
        !submitButton ||
        !taskLinkContainer ||
        !taskLink
    ) {
        console.error("One or more task modal elements not found");
        return;
    }

    // Set task details
    taskName.textContent = task.taskName || "Unnamed Task";
    taskDescription.textContent = task.description || "No Description";

    // Handle task link
    if (task.link) {
        taskLinkContainer.style.display = "block";
        taskLink.innerHTML = `<a href="${task.link}" target="_blank">${task.link}</a>`;
    } else {
        taskLinkContainer.style.display = "none";
    }

    // Format dates
    try {
        const startTime = new Date(task.startTime);
        const endTime = new Date(task.endTime);
        const currentTime = new Date();

        // Check if task has expired
        const isExpired = currentTime > endTime;

        const formattedStartTime = startTime.toLocaleString("vi-VN", {
            hour: "2-digit",
            minute: "2-digit",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });

        const formattedEndTime = endTime.toLocaleString("vi-VN", {
            hour: "2-digit",
            minute: "2-digit",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });

        taskStartTime.textContent = formattedStartTime;

        // Apply styling to end time based on expiration
        if (isExpired) {
            taskEndTime.innerHTML = `<span class="text-danger fw-bold">${formattedEndTime}</span> <span class="badge bg-danger">Đã hết hạn</span>`;
            submitButton.disabled = true;
            submitButton.title = "Đã hết hạn nộp bài";
        } else {
            taskEndTime.textContent = formattedEndTime;
            submitButton.disabled = false;
            submitButton.title = "";
        }

        // Store expired status in modal
        taskDetailModal.dataset.expired = isExpired;
    } catch (error) {
        console.error("Error formatting task dates:", error);
        taskStartTime.textContent = "Invalid Date";
        taskEndTime.textContent = "Invalid Date";
    }

    // Xử lý fileData
    if (task.fileData) {
        fileContainer.style.display = "block";
        const extractedFileName = task.fileData.split("/").pop() || "file.pdf";
        fileName.textContent = extractedFileName;
        fileLink.href = "#";
        fileLink.onclick = () => {
            window.open(task.fileData, "_blank");
            return false;
        };
    } else {
        fileContainer.style.display = "none";
        console.warn("No fileData provided for task");
    }

    // Load submission history
    loadSubmissionHistory(task.id);

    // Store task ID
    taskDetailModal.setAttribute("data-task-id", task.id);

    // Show modal
    const modal = new bootstrap.Modal(taskDetailModal);
    modal.show();
}

// Function to load submission history
function loadSubmissionHistory(taskId) {
    const submissionList = document.getElementById("submission-list");
    const submitButton = document.getElementById("submit-task-btn"); // Reference to submit button

    if (!submissionList) {
        console.error("Submission list element not found");
        return;
    }

    if (!submitButton) {
        console.error("Submit button element not found");
        return;
    }

    submissionList.innerHTML = `
      <div class="text-center py-2">
          <div class="spinner-border spinner-border-sm text-primary" role="status"></div>
          <span>Loading Submit history...</span>
      </div>
  `;

    fetch(`/api/completed-tasks/find/${taskId}/${internId}/${classId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Cannot fetch History Submit from server');
            }
            return response.json();
        })
        .then(submissions => {

            // Check if submissions.mark is not null
            if (submissions.mark !== null) {
                submitButton.disabled = true;
                submitButton.title = "The Task has been Graded and cannot be resubmitted.";
            } else {
                // Ensure button is enabled if mark is null and task is not expired
                const taskDetailModal = document.getElementById("taskDetailModal");
                const isExpired = taskDetailModal.dataset.expired === "true";
                submitButton.disabled = isExpired;
                submitButton.title = isExpired ? "Submission deadline has passed." : "";
            }

            // Display submission history
            displaySubmissionHistory(submissions);
        })
        .catch(error => {
            console.error("Fall to list History:", error);
            // Show empty state
            submissionList.innerHTML = `
          <div class="alert alert-info">
               You not submit any yet.
          </div>
      `;
        });
}

// Function to display submission history
function displaySubmissionHistory(submissions) {
    const submissionList = document.getElementById("submission-list")
    const submitButton = document.getElementById("submit-task-btn")
    const taskDetailModal = document.getElementById("taskDetailModal")

    if (!submissionList || !submitButton || !taskDetailModal) {
        console.error("Submission list or submit button element not found")
        return
    }

    submissionList.innerHTML = ""

    if (!submissions) {
        submissionList.innerHTML = `
            <div class="alert alert-info">
             You have no submissions for this task yet.
            </div>
        `
        return
    }

    const submissionArray = Array.isArray(submissions) ? submissions : [submissions]

    if (submissionArray.length === 0) {
        submissionList.innerHTML = `
            <div class="alert alert-info">
          You have no submissions for this task yet.
            </div>
        `
        return
    }

    // Check if task is expired
    const isExpired = taskDetailModal.dataset.expired === "true"

    let hasCompletedSubmission = false

    submissionArray.forEach((submission) => {
        try {
            const submissionDate = new Date(submission.createdAt)
            const formattedDate = submissionDate.toLocaleString("vi-VN")

            // Check if this submission is completed
            const isCompleted = submission.status === "PENDING"
            if (isCompleted) {
                hasCompletedSubmission = true
            }

            const submissionItem = document.createElement("div")
            submissionItem.className = "card mb-2"

            // Create mentor comment section if status is COMPLETED
            let mentorCommentHTML = ""
            if (isCompleted && submission.comment) {
                mentorCommentHTML = `
            <div class="mentor-comment mt-2 p-2 border-start border-4 border-info bg-light rounded">
              <div class="mentor-comment-header fw-bold mb-1">
              Comment from mentor:
              </div>
              <div class="ps-2">${submission.comment || "No comment from Mentor"}</div>
            </div>
          `
            }

            // Create mark/score section if it exists
            let markHTML = ""
            if (submission.mark !== null && submission.mark !== undefined) {
                markHTML = `
            <div class="submission-mark mt-2 mb-2">
              <div class="d-flex align-items-center">

                <span class="fw-bold">Score:</span>
                <span class="ms-2 badge bg-success fs-6">${submission.mark}</span>
              </div>
            </div>
          `
            }

            submissionItem.innerHTML = `
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong>Submit at: ${formattedDate}</strong>
                        <span class="badge bg-${submission.status === "PENDING" ? "warning" : "success"}">${submission.status}</span>
                    </div>
                    <div class="mb-2">
                        <span class="fw-bold">Attachment File:</span>
                        <a href="${submission.file || "#"}" target="_blank" class="text-decoration-none">
                          ${submission.file ? submission.file.split("/").pop() : "File Attachments"}
                        </a>
                    </div>
                    ${markHTML}
                    ${mentorCommentHTML}
                </div>
            `

            submissionList.appendChild(submissionItem)
        } catch (error) {
            console.error("Error rendering submission:", error, submission)
        }
    })
}

// Function to submit a task
function submitTask() {
    const fileInput = document.getElementById("submission-file");
    const submitButton = document.getElementById("submit-task-btn");
    const taskDetailModal = document.getElementById("taskDetailModal");

    console.log('submitTask called. Elements:', {
        fileInput: !!fileInput,
        submitButton: !!submitButton,
        taskDetailModal: !!taskDetailModal
    });

    if (!fileInput || !submitButton || !taskDetailModal) {
        console.error("One or more submission form elements not found");
        return;
    }

    const taskId = taskDetailModal.getAttribute("data-task-id");
    console.log('Task ID from modal:', taskId);

    const isExpired = taskDetailModal.dataset.expired === "true";
    console.log('Is task expired:', isExpired);
    if (isExpired) {
        showGlobalAlert("You cannot submit because the deadline has passed!", "danger");
        return;
    }

    if (fileInput.files.length === 0) {
        console.log('No file selected for upload');
        showGlobalAlert("Please choose right File Attachments!", "warning");
        return;
    }

    submitButton.disabled = true;
    submitButton.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Submitting...`;
    console.log('Submit button disabled and set to loading state');

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);
    console.log('Uploading file to Cloudinary:', fileInput.files[0]);

    fetch('/cloudinary/upload/uploadFile', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            console.log('Cloudinary upload response status:', response.status);
            if (!response.ok) {
                throw new Error('Không thể upload file lên Cloudinary');
            }
            return response.json();
        })
        .then(data => {
            const fileUrl = data.secure_url;
            console.log('File uploaded to Cloudinary. URL:', fileUrl);

            console.log('Checking for existing CompletedTask with taskId:', taskId, 'userId:', internId, 'classId:', classId);
            return fetch(`/api/completed-tasks/find/${taskId}/${internId}/${classId}`, {
                method: 'GET'
            })
                .then(findResponse => {
                    console.log('Find CompletedTask response status:', findResponse.status);
                    if (findResponse.status === 404) {
                        console.log('No existing CompletedTask found, creating a new one');
                        const newTask = {
                            id: {
                                taskId: parseInt(taskId),
                                userId: parseInt(internId),
                                classId: parseInt(classId)
                            },
                            file: fileUrl,
                            status: "PENDING",
                            createdAt: new Date().toISOString(),
                            isActive: true
                        };
                        console.log('New CompletedTask object:', newTask);
                        console.log('POST request body:', JSON.stringify(newTask));

                        return fetch('/api/completed-tasks/create', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(newTask)
                        });
                    } else if (findResponse.ok) {
                        console.log('Existing CompletedTask found, updating file');
                        return findResponse.json().then(existingTask => {
                            console.log('Existing CompletedTask:', existingTask);
                            return fetch(`/api/completed-tasks/${taskId}/${internId}/${classId}/File`, {
                                method: 'PATCH',
                                headers: {
                                    'Content-Type': 'application/json'
                                },
                                body: fileUrl
                            });
                        });
                    } else {
                        throw new Error('Error checking for existing CompletedTask');
                    }
                });
        })
        .then(response => {
            console.log('Final API response status:', response.status);
            if (!response.ok) {
                throw new Error('Cannot save CompletedTask');
            }
            return response.json();
        })
        .then(result => {
            console.log('CompletedTask saved successfully:', result);
            document.getElementById("submission-form").reset();
            console.log('Submission form reset');
            console.log('Reloading submission history for taskId:', taskId);
            loadSubmissionHistory(taskId);
            showGlobalAlert("Submit successfully!", "success");
        })
        .catch(error => {
            console.error('Error in submitTask:', error);
            showGlobalAlert("Cannot submit. Try again.", "warning");
        })
        .finally(() => {
            submitButton.disabled = false;
            submitButton.textContent = "Submit!";
            console.log('Submit button reset');
        });
}
async function loadSubmissionHistory2(taskId) {

    const submissionList = document.getElementById("submission-list");
    if (!submissionList) {
        console.error("Submission list element not found");
        throw new Error("Submission list element not found");
    }

    submissionList.innerHTML = `
        <div class="text-center py-2">
            <div class="spinner-border spinner-border-sm text-primary" role="status"></div>
            <span>Đang tải lịch sử nộp bài...</span>
        </div>
    `;

    try {
        const response = await fetch(`/api/completed-tasks/find/${taskId}/${internId}/${classId}`);
        if (!response.ok) {
            throw new Error('Cannot fetch Submit history from server');
        }
        const submissions = await response.json();
        displaySubmissionHistory(submissions);
        return submissions; // Trả về danh sách submissions
    } catch (error) {
        // Show empty state
        submissionList.innerHTML = `
            <div class="alert alert-info">
              You have no submissions for this task yet.
            </div>
        `;
        throw error;
    }
}