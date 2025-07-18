// Global variable to store the file to be deleted
let currentFileToDelete = null;

document.addEventListener("DOMContentLoaded", async function () {
    const id = document.getElementById("user_id").value;
    document.getElementById("uploadForm").addEventListener("submit", uploadFile);

    // File input change event
    document.getElementById('fileInput').addEventListener('change', function(e) {
        const fileNameEl = document.getElementById('fileName');
        if (e.target.files[0]) {
            const fileName = e.target.files[0].name;
            fileNameEl.textContent = `📂 Selected: ${fileName}`;
            fileNameEl.style.display = 'block';
            document.getElementById('uploadButton').disabled = false;
        } else {
            fileNameEl.style.display = 'none';
            document.getElementById('uploadButton').disabled = true;
        }
    });

    // Upload area click event
    document.getElementById('uploadArea').addEventListener('click', function(event) {
        if (event.target.id !== 'fileInput') {
            document.getElementById('fileInput').click();
        }
    });


    // Drag and drop functionality
    const uploadArea = document.getElementById('uploadArea');

    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        uploadArea.addEventListener(eventName, highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, unhighlight, false);
    });

    function highlight() {
        uploadArea.style.borderColor = 'var(--fpt-orange)';
        uploadArea.style.backgroundColor = 'rgba(247, 148, 29, 0.05)';
    }

    function unhighlight() {
        uploadArea.style.borderColor = 'var(--gray-300)';
        uploadArea.style.backgroundColor = 'var(--gray-100)';
    }

    uploadArea.addEventListener('drop', handleDrop, false);

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length) {
            document.getElementById('fileInput').files = files;
            const fileName = files[0].name;
            const fileNameEl = document.getElementById('fileName');
            fileNameEl.textContent = `📂 Selected: ${fileName}`;
            fileNameEl.style.display = 'block';
            document.getElementById('uploadButton').disabled = false;
        }
    }

    // Fetch user data
    try {
        let response = await fetch(`/api/user/` + id);
        if (!response.ok) throw new Error("User not found");
        const user = await response.json();
        document.getElementById("profile_picture").src = user.avatar_path || '/placeholder.svg?height=128&width=128';
        document.getElementById("full_name").textContent = `${user.first_name} ${user.last_name}`;
        document.getElementById("email").textContent = user.email;
        document.getElementById("phone").textContent = user.phone_number || 'Not provided';
    } catch (error) {
        console.error("Error fetching user:", error);
    }

    // Fetch CV list
    await refreshCVList(id);

    // Set up delete confirmation modal event listeners
    document.getElementById('closeConfirmModal').addEventListener('click', function() {
        document.getElementById('confirmDeleteModal').style.display = 'none';
    });

    document.getElementById('cancelDeleteButton').addEventListener('click', function() {
        document.getElementById('confirmDeleteModal').style.display = 'none';
    });

    document.getElementById('confirmDeleteButton').addEventListener('click', function() {
        if (currentFileToDelete) {
            deleteFile(currentFileToDelete);
        }
    });

    // Close modal when clicking outside
    document.getElementById('confirmDeleteModal').addEventListener('click', function(e) {
        if (e.target === this) {
            this.style.display = 'none';
        }
    });
});

// Upload file function
async function uploadFile(event) {
    event.preventDefault();
    const userId = document.getElementById("user_id").value;
    var fileInput = document.getElementById("fileInput");
    const messageEl = document.getElementById("message");
    const loadingEl = document.getElementById("loading");
    const fileNameEl = document.getElementById("fileName");
    const progressContainer = document.getElementById("progressContainer");
    const progressBar = document.getElementById("progressBar");
    const cv_list = document.getElementById("cv_list");

    // Check required elements
    if (!cv_list || !messageEl) {
        console.error("❌ One or more UI elements don't exist!");
        return;
    }

    // Set up initial UI
    messageEl.textContent = "";
    messageEl.className = "";
    messageEl.style.display = "none";
    loadingEl.style.display = "block";
    progressContainer.style.display = "block";
    progressBar.style.width = "0%";

    // Simulate progress bar
    let progress = 0;
    const progressInterval = setInterval(() => {
        progress += Math.random() * 10;
        if (progress > 90) progress = 90;
        progressBar.style.width = progress + "%";
    }, 300);

    // Check file
    if (!fileInput.files.length) {
        messageEl.textContent = "❌ Please select a file!";
        messageEl.className = "message-error";
        messageEl.style.display = "block";
        loadingEl.style.display = "none";
        progressContainer.style.display = "none";
        clearInterval(progressInterval);
        return;
    }

    const file = fileInput.files[0];
    const fileName = file.name;
    fileNameEl.textContent = `📂 Uploading: ${fileName}`;

    const formData = new FormData();
    formData.append("file", file);
    formData.append("Filetype", "CV");

    if (userId && !isNaN(userId)) {
        formData.append("userId", userId);
    } else {
        messageEl.textContent = "User ID not found!";
        messageEl.className = "message-error";
        messageEl.style.display = "block";
        loadingEl.style.display = "none";
        progressContainer.style.display = "none";
        clearInterval(progressInterval);
        return;
    }

    try {
        const response = await fetch("/api/File", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        progressBar.style.width = "100%";
        clearInterval(progressInterval);

        const data = await response.json();
        let message = data.success ? " " + data.message : " " + data.message;
        messageEl.textContent = message;
        messageEl.className = data.success ? "message-success" : "message-error";
        messageEl.style.display = "block";

        if (data.success && data.data && data.data.url) {
            let fileUrl = data.data.url;

            // Add new file to temporary list
            if (cv_list.innerHTML.includes("No CV documents found")) {
                cv_list.innerHTML = "";
            }
            let listItem = document.createElement("li");
            listItem.className = "cv-list-item";
            listItem.style.backgroundColor = "rgba(247, 148, 29, 0.1)";
            listItem.style.borderLeft = "4px solid var(--fpt-orange)";

            let fileIcon = document.createElement("i");
            const fileExt = fileName.split('.').pop().toLowerCase();
            if (fileExt === 'pdf') {
                fileIcon.className = "fas fa-file-pdf file-icon";
                fileIcon.style.color = "#e74c3c";
            } else if (['doc', 'docx'].includes(fileExt)) {
                fileIcon.className = "fas fa-file-word file-icon";
                fileIcon.style.color = "#ff923a";
            } else {
                fileIcon.className = "fas fa-file-alt file-icon";
            }

            let nameSpan = document.createElement("span");
            nameSpan.className = "file-name";
            nameSpan.textContent = fileName;

            let buttonContainer = document.createElement("div");
            buttonContainer.style.display = "flex";
            buttonContainer.style.gap = "8px";

            let viewButton = document.createElement("button");
            viewButton.className = "view-button";
            viewButton.innerHTML = '<i class="fas fa-eye mr-1"></i> View';
            viewButton.onclick = (e) => {
                e.preventDefault();
                showPreview(fileUrl);
            };

            buttonContainer.appendChild(viewButton);
            listItem.appendChild(fileIcon);
            listItem.appendChild(nameSpan);
            listItem.appendChild(buttonContainer);

            if (cv_list.firstChild) {
                cv_list.insertBefore(listItem, cv_list.firstChild);
            } else {
                cv_list.appendChild(listItem);
            }

            setTimeout(() => {
                listItem.style.transition = "background-color 1s ease";
                listItem.style.backgroundColor = "";
            }, 1000);


            await refreshCVList(userId);
        }
    } catch (error) {
        messageEl.textContent = "Upload error: " + error.message;
        messageEl.className = "message-error";
        messageEl.style.display = "block";
        clearInterval(progressInterval);
    } finally {
        loadingEl.style.display = "none";
        setTimeout(() => {
            progressContainer.style.display = "none";
            fileNameEl.textContent = "";
        }, 500);
    }
}

// Refresh CV list function
async function refreshCVList(userId) {
    try {
        const cvResponse = await fetch(`/api/File/user/${userId}`);
        if (!cvResponse.ok) {
            throw new Error(`HTTP error! Status: ${cvResponse.status}`);
        }
        const cvData = await cvResponse.json();

        const cv_list = document.getElementById("cv_list");
        if (!cv_list) {
            throw new Error("CV list element doesn't exist in the DOM!");
        }

        cv_list.innerHTML = "";

        const cvArray = Array.isArray(cvData) ? cvData : (cvData.data || []);
        if (cvArray.length > 0) {
            cvArray.forEach(cv => {
                const listItem = document.createElement("li");
                listItem.className = "cv-list-item";
                listItem.dataset.fileId = cv.id || ''; // Store file ID for deletion

                const fileIcon = document.createElement("i");
                const fileExt = cv.display_name ? cv.display_name.split('.').pop().toLowerCase() : '';
                if (fileExt === 'pdf') {
                    fileIcon.className = "fas fa-file-pdf file-icon";
                    fileIcon.style.color = "#e74c3c";
                } else if (['doc', 'docx'].includes(fileExt)) {
                    fileIcon.className = "fas fa-file-word file-icon";
                    fileIcon.style.color = "#3498db";
                } else {
                    fileIcon.className = "fas fa-file-alt file-icon";
                }

                const nameSpan = document.createElement("span");
                nameSpan.className = "file-name";
                nameSpan.textContent = cv.display_name;

                const buttonContainer = document.createElement("div");
                buttonContainer.style.display = "flex";
                buttonContainer.style.gap = "8px";

                const viewButton = document.createElement("button");
                viewButton.className = "view-button";
                viewButton.innerHTML = '<i class="fas fa-eye mr-1"></i> View';
                viewButton.onclick = (e) => {
                    e.preventDefault();
                    showPreview(cv.url || cv.path);
                };

                const deleteButton = document.createElement("button");
                deleteButton.className = "delete-button";
                deleteButton.innerHTML = '<i class="fas fa-times"></i>';
                deleteButton.onclick = (e) => {
                    e.preventDefault();
                    showDeleteConfirmation(cv);
                };

                buttonContainer.appendChild(viewButton);
                buttonContainer.appendChild(deleteButton);

                listItem.appendChild(fileIcon);
                listItem.appendChild(nameSpan);
                listItem.appendChild(buttonContainer);
                cv_list.appendChild(listItem);
            });
        } else {
            cv_list.innerHTML = '<li class="cv-list-item text-gray-500 italic">No CV documents found</li>';
        }
    } catch (error) {
        console.error(error);
        // cv_list.innerHTML = '<li class="cv-list-item text-red-500">Error loading CV list</li>';
    }
}

// Function to show delete confirmation
function showDeleteConfirmation(file) {
    currentFileToDelete = file;
    const modal = document.getElementById('confirmDeleteModal');
    const fileNameSpan = document.getElementById('fileToDeleteName');
    const confirmButton = document.getElementById('confirmDeleteButton');

    fileNameSpan.textContent = file.display_name;
    confirmButton.disabled = false;
    confirmButton.textContent = 'Delete';

    modal.style.display = 'flex';
}

// Function to delete file
async function deleteFile(file) {
    const confirmButton = document.getElementById('confirmDeleteButton');
    confirmButton.disabled = true;
    confirmButton.textContent = 'Deleting...';

    try {
        // Replace with your actual delete API endpoint
        const response = await fetch(`/file/fully-delete/${file.id}`, {
            method: 'DELETE',
        });

        if (!response.ok) {
            throw new Error('Failed to delete file');
        }

        // Close modal
        document.getElementById('confirmDeleteModal').style.display = 'none';

        // Show success message
        const messageEl = document.getElementById('message');
        messageEl.textContent = '✅ File deleted successfully';
        messageEl.className = 'message-success';
        messageEl.style.display = 'block';

        // Refresh CV list
        const userId = document.getElementById('user_id').value;
        await refreshCVList(userId);

        // Hide message after 3 seconds
        setTimeout(() => {
            messageEl.style.display = 'none';
        }, 3000);
        setTimeout(() => {
            window.location.reload(); // Load lại trang
        }, 1500);
    } catch (error) {
        console.error('Error deleting file:', error);

        // Show error message
        const messageEl = document.getElementById('message');
        messageEl.textContent = '❌ Error deleting file: ' + error.message;
        messageEl.className = 'message-error';
        messageEl.style.display = 'block';

        // Close modal
        document.getElementById('confirmDeleteModal').style.display = 'none';
    }
}

// Function to show file preview in iframe (popup-style)
function showPreview(filePath) {
    // Remove existing preview container if it exists
    let existingPreview = document.getElementById("previewContainer");
    if (existingPreview) {
        existingPreview.remove();
    }

    let previewContainer = document.createElement("div");
    previewContainer.id = "previewContainer";
    previewContainer.style.position = "fixed";
    previewContainer.style.top = "0";
    previewContainer.style.left = "0";
    previewContainer.style.width = "100%";
    previewContainer.style.height = "100%";
    previewContainer.style.background = "rgba(0, 0, 0, 0.8)";
    previewContainer.style.display = "flex";
    previewContainer.style.justifyContent = "center";
    previewContainer.style.alignItems = "center";
    previewContainer.style.zIndex = "1000";

    // Close button
    let closeButton = document.createElement("span");
    closeButton.innerHTML = "&times;";
    closeButton.style.position = "absolute";
    closeButton.style.top = "20px";
    closeButton.style.right = "30px";
    closeButton.style.fontSize = "40px";
    closeButton.style.color = "white";
    closeButton.style.cursor = "pointer";
    closeButton.style.zIndex = "1001";
    closeButton.onclick = () => {
        previewContainer.remove(); // Remove the popup
    };

    // Preview Box
    let previewDiv = document.createElement("div");
    previewDiv.style.width = "90%";
    previewDiv.style.maxWidth = "1000px";
    previewDiv.style.height = "90%";
    previewDiv.style.maxHeight = "800px";
    previewDiv.style.background = "#fff";
    previewDiv.style.borderRadius = "8px";
    previewDiv.style.boxShadow = "0px 0px 20px rgba(255, 255, 255, 0.3)";
    previewDiv.style.position = "relative";
    previewDiv.style.overflow = "hidden";

    // Header for the preview
    let previewHeader = document.createElement("div");
    previewHeader.style.padding = "15px 20px";
    previewHeader.style.borderBottom = "1px solid #eee";
    previewHeader.style.display = "flex";
    previewHeader.style.justifyContent = "space-between";
    previewHeader.style.alignItems = "center";
    previewHeader.style.backgroundColor = "var(--fpt-orange)";
    previewHeader.style.color = "white";
    previewHeader.style.background = "linear-gradient(90deg, #F7941D, #3437db)";

    let previewTitle = document.createElement("h3");
    previewTitle.textContent = "Document Preview";
    previewTitle.style.color = "white";
    previewTitle.style.margin = "0";
    previewTitle.style.fontSize = "18px";
    previewTitle.style.fontWeight = "600";

    let closeX = document.createElement("button");
    closeX.innerHTML = "&times;";
    closeX.style.background = "none";
    closeX.style.border = "none";
    closeX.style.fontSize = "24px";
    closeX.style.cursor = "pointer";
    closeX.style.color = "white";
    closeX.onclick = () => {
        previewContainer.remove();
    };

    previewHeader.appendChild(previewTitle);
    previewHeader.appendChild(closeX);

    let iframe = document.createElement("iframe");
    iframe.src = filePath;
    iframe.style.width = "100%";
    iframe.style.height = "calc(100% - 56px)";
    iframe.style.border = "none";
    iframe.style.display = "block";

    previewDiv.appendChild(previewHeader);
    previewDiv.appendChild(iframe);
    previewContainer.appendChild(closeButton);
    previewContainer.appendChild(previewDiv);

    // Append modal only when needed
    document.body.appendChild(previewContainer);

    previewContainer.onclick = (e) => {
        if (e.target === previewContainer) {
            previewContainer.remove();
        }
    };
}

// Handle delete button click
deleteButton.addEventListener('click', () => {
    resetForm();
});

// Reset form
function resetForm() {
    fileInput.value = '';
    fileName.textContent = '';
    fileName.style.display = 'none';
    progressContainer.style.display = 'none';
    progressBar.style.width = '0%';
    uploadButton.disabled = true;
    loading.style.display = 'none';
    message.textContent = '';
    message.className = '';
    deleteButton.style.display = 'none';
}