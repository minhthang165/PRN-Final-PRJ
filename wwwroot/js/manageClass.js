// Hàm sử dụng UserModal
document.addEventListener("DOMContentLoaded", function () {
    // Prevent multiple initializations
    if (window.manageClassInitialized) {
        console.warn('manageClass.js already initialized!');
        return;
    }
    window.manageClassInitialized = true;

    loadPage(0, 5);
    const searchInput = document.getElementById("searchInput");
    if (searchInput) {
        searchInput.addEventListener('keyup', filterTable);
    }
    // Bắt sự kiện click vào từng row (trừ nút action)
    const tableContent = document.getElementById("table-content");
    if (tableContent) {
        tableContent.addEventListener("click", function (event) {
            let row = event.target.closest("tr");
            if (!row || event.target.closest("td:last-child")) return;

            openUserModal(row);
        });
    }

    // Handle new class form submission
    const newClassFormSubmit = document.getElementById("newClassFormSubmit");
    if (newClassFormSubmit) {
        newClassFormSubmit.addEventListener("click", function (event) {
            event.preventDefault();
            event.stopPropagation();

            let className = document.getElementById("newClassName").value;
            let numberOfIntern = document.getElementById("newNumberOfInterns").value;
            let status = document.getElementById("newStatus").value;
            let managerId = document.getElementById("newManager").value;

            // Validate form
            if (!className || !numberOfIntern || !status || !managerId) {
                showToast("Please fill all required fields", "warning");
                return;
            }

            // Disable button to prevent double submission
            newClassFormSubmit.disabled = true;
            const originalText = newClassFormSubmit.textContent;
            newClassFormSubmit.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Creating...';

            let conversationData = {
                ConversationName: className + " - Group Chat",
                ConversationAvatar: "assets/img/users/default-avatar.png",
                Type: "Group"
            };

            // Bước 1: Tạo conversation
            fetch(`/api/conversation/group/create`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(conversationData)
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(text); });
                    }
                    return response.json();
                })
                .then(conversation => {
                    console.log("Conversation created:", conversation);
                    
                    // Use conversation_id instead of id
                    const conversationId = conversation.conversation_id;
                    
                    if (!conversationId) {
                        throw new Error("Conversation ID not found in response");
                    }
                    
                    let conversationUserData = {
                        conversation_id: conversationId,
                        user_id: parseInt(managerId),
                        admin: true
                    };

                    return fetch(`/api/conversation-user/add-user`, {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(conversationUserData)
                    })
                        .then(response => {
                            if (!response.ok) {
                                return response.text().then(text => { throw new Error(text); });
                            }
                            return response.json();
                        })
                        .then(() => conversationId);
                })
                .then(conversationId => {
                    // Fix: Use correct field names matching the Class entity
                    let requestData = {
                        class_name: className,
                        number_of_interns: parseInt(numberOfIntern),
                        mentor_id: parseInt(managerId),
                        status: status,
                        conversation_id: conversationId,
                        is_active: true,
                        created_at: new Date().toISOString()
                    };

                    return fetch(`/api/class`, {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(requestData)
                    });
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(text); });
                    }
                    return response.json();
                })
                .then(newClass => {
                    showToast("Created a new class successfully!", "success");
                    let newClassModal = bootstrap.Modal.getInstance(document.getElementById("newClassModal"));
                    newClassModal.hide();
                    document.getElementById("newClassForm").reset();
                    
                    // Reload the page to show the new class
                    setTimeout(() => {
                        location.reload();
                    }, 1000);
                })
                .catch(error => {
                    console.error("Error creating class:", error);
                    showToast("Error Create class: " + error.message, "error");
                    
                    // Re-enable button
                    newClassFormSubmit.disabled = false;
                    newClassFormSubmit.textContent = originalText;
                });
        });
    }

    // Handle edit class form submission
    const editClassFormSubmit = document.getElementById("editClassFormSubmit");
    if (editClassFormSubmit) {
        editClassFormSubmit.addEventListener("click", function (event) {
            event.preventDefault();
            event.stopPropagation();

            let classId = document.getElementById("editClassId").value;
            let className = document.getElementById("editClassName").value;
            let numberOfInterns = document.getElementById("editNumberOfInterns").value;
            let status = document.getElementById("editStatus").value;
            let managerId = document.getElementById("editManager").value;

            // Validate form
            if (!className || !numberOfInterns || !status || !managerId) {
                showToast("Please fill all required fields", "warning");
                return;
            }

            // Disable button to prevent double submission
            editClassFormSubmit.disabled = true;
            const originalText = editClassFormSubmit.textContent;
            editClassFormSubmit.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Updating...';

            // Fix: Use correct field names matching the Class entity
            let requestData = {
                class_name: className,
                number_of_interns: parseInt(numberOfInterns),
                status: status,
                mentor_id: parseInt(managerId)
            };

            fetch(`/api/class/update/${classId}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(requestData)
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(text); });
                    }
                    return response.json();
                })
                .then(updatedClass => {
                    showToast("Class updated successfully!", "success");
                    let editClassModal = bootstrap.Modal.getInstance(document.getElementById("editClassModal"));
                    editClassModal.hide();
                    
                    setTimeout(() => {
                        location.reload();
                    }, 1000);
                })
                .catch(error => {
                    console.error("Error updating class:", error);
                    showToast("Error Update class: " + error.message, "warning");
                    
                    // Re-enable button
                    editClassFormSubmit.disabled = false;
                    editClassFormSubmit.textContent = originalText;
                });
        });
    }
});


function openUserModal(row) {
    // Lấy dữ liệu từ hàng được nhấp
    const classId = row.getAttribute('data-class-id');
    const className = row.cells[0].textContent;
    const managerName = row.cells[2].textContent;

    // Cập nhật tiêu đề modal
    document.getElementById("userModalTitle").textContent = `${className} - MANAGER BY: ${managerName}`;

    // Tải danh sách người dùng
    loadUsers(classId);

    // Lấy tham chiếu đến modal
    const modalElement = document.getElementById("userModal");

    // Đảm bảo modal có thuộc tính data-bs-backdrop="static" để hiển thị backdrop đen
    modalElement.setAttribute('data-bs-backdrop', 'static');

    // Khởi tạo và hiển thị modal với các tùy chọn
    const userModal = new bootstrap.Modal(modalElement, {
        backdrop: true,
        keyboard: true,
        focus: true
    });

    // Hiển thị modal
    userModal.show();

    // Thêm class để căn giữa modal sau khi nó được hiển thị
    modalElement.addEventListener('shown.bs.modal', function () {
        document.querySelector('.modal-dialog').classList.add('modal-dialog-centered');
    }, { once: true });
}

function closeModal() {
    const userModal = bootstrap.Modal.getInstance(document.getElementById("userModal"));
    if (userModal) {
        userModal.hide();

        // Đảm bảo xóa backdrop nếu còn sót lại
        setTimeout(() => {
            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove());
            document.body.classList.remove('modal-open');
        }, 300); // Đợi hiệu ứng đóng modal hoàn tất
    }
}

function loadUsers(classId) {
    const tableBody = document.getElementById("userTableBody");
    tableBody.innerHTML = '<tr><td colspan="4" class="text-center">Loading users...</td></tr>';

    fetch(`/api/user/classroom/${classId}`)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(`Failed to fetch users: ${response.status} - ${text}`);
                });
            }
            return response.json();
        })
        .then(data => {
            // Xử lý trường hợp data có thể là object chứa array hoặc trực tiếp là array
            let users = Array.isArray(data) ? data : (data.items || data.users || data.data || []);
            
            displayUsers(users);
        })
        .catch(error => {
            console.error("Error fetching users:", error);
            tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Error loading users</td></tr>';
        });
}

function displayUsers(users) {
    const tableBody = document.getElementById("userTableBody");
    
    // Validate input
    if (!tableBody) {
        console.error("userTableBody element not found");
        return;
    }
    
    // Clear existing content
    tableBody.innerHTML = "";

    // Check if users is valid array
    if (!Array.isArray(users) || users.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center">No Interns found in this class</td></tr>';
        return;
    }

    // Render users
    users.forEach(user => {
        if (!user) return; // Skip null/undefined users
        
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${user.first_name || ''} ${user.last_name || ''}</td>
            <td>${user.email || 'N/A'}</td>
            <td>${user.phone_number || 'N/A'}</td>
            <td>${user.gender || 'N/A'}</td>
        `;
        tableBody.appendChild(row);
    });
}

document.addEventListener("click", function (event) {

    let icon = event.target.closest(".flaticon-padlock");
    if (!icon) return;

    event.preventDefault();
    event.stopPropagation();

    let classId = icon.getAttribute("data-id");
    if (!classId) {
        showModal("Error", "Missing class ID!");
        return;
    }

    let isUnlock = icon.classList.contains("inactive");
    let action = isUnlock ? "unlock" : "lock";
    let message = isUnlock ? "Are you sure you want to unlock this class?" : "Are you sure you want to lock this class?";

    showModal("Confirm", message, true, function () {
        toggleClassStatus(classId, action);
    });
});
function toggleClassStatus(classId, action) {
    let url = action === "unlock" ? `/api/Class/setIsActiveTrue/${classId}` : `/api/class/delete/${classId}`;
    let method = action === "unlock" ? "PATCH" : "DELETE";

    fetch(url, { method: method })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.json();
        })
        .then(() => {
            let successMessage = action === "unlock" ? "Class has been unlocked!" : "Class has been locked!";
            showModal("Success", successMessage);

            let icon = document.querySelector(`.flaticon-padlock[data-id="${classId}"]`);
            if (icon) {
                icon.classList.toggle("inactive");
                icon.classList.toggle("active");
            }
        })
        .catch(error => showModal("Error", "Error: " + error.message));
}
function toggleActiveStatus(icon, event) {
    event.preventDefault();
    event.stopPropagation();

    let element = icon;
    let classId = null;

    // Duyệt ngược lên cha để tìm data-id
    while (element) {
        if (element.hasAttribute("data-id")) {
            classId = element.getAttribute("data-id");
            break;
        }
        element = element.parentElement; // Duyệt lên thẻ cha
    }

    if (!classId) {
        showModal("Error", "Missing class ID!");
        return;
    }

    let isUnlock = icon.classList.contains("inactive");
    let action = isUnlock ? "unlock" : "lock";
    let message = isUnlock
        ? "Are you sure you want to unlock this class?"
        : "Are you sure you want to lock this class?";

    showModal("Confirm", message, true, function () {
        let url = action === "unlock"
            ? `/api/Class/setIsActiveTrue/${classId}`
            : `/api/Class/${classId}`;
        let method = action === "unlock" ? "PATCH" : "DELETE";

        fetch(url, { method: method })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }
                return response.json();
            })
            .then(() => {
                let successMessage = action === "unlock"
                    ? "Class has been unlocked!"
                    : "Class has been locked!";
                showModal("Success", successMessage);

                // Cập nhật trạng thái icon trên UI
                icon.classList.toggle("inactive");
                icon.classList.toggle("active");
            })
            .catch(error => showModal("Error", "Error: " + error.message));
    });
}


function showModal(title, message, isConfirm = false, confirmCallback = null) {

    let modalTitle = document.getElementById("modalTitle");
    let modalMessage = document.getElementById("modalMessage");
    let confirmBtn = document.getElementById("modalConfirmBtn");
    let cancelBtn = document.getElementById("modalCancelBtn");
    let okBtn = document.getElementById("modalOkBtn");

    modalTitle.innerText = title;
    modalMessage.innerText = message;

    if (isConfirm) {
        confirmBtn.classList.remove("d-none");
        cancelBtn.classList.remove("d-none");
        okBtn.classList.add("d-none");

        confirmBtn.onclick = function () {
            confirmCallback();
            let modalInstance = bootstrap.Modal.getInstance(document.getElementById("genericModal"));
            modalInstance.hide();
        };
    } else {
        confirmBtn.classList.add("d-none");
        cancelBtn.classList.add("d-none");
        okBtn.classList.remove("d-none");
    }

    let modal = new bootstrap.Modal(document.getElementById("genericModal"));
    modal.show();
}

// XỬ lý khi click vào nút newCLass
document.addEventListener("click", function (event) {
    let newClassButton = event.target.closest(".new_class");
    if (!newClassButton) return;

    event.preventDefault();

    let addNewClassModal = new bootstrap.Modal(document.getElementById("newClassModal"));
    addNewClassModal.show();
});

// Function to open edit modal
document.addEventListener("click", function (event) {
    let editButton = event.target.closest(".edit-class-status");
    if (!editButton) return;

    event.preventDefault();
    let classId = editButton.getAttribute("data-class-id");
    if (!classId) {
        showModal("Error", "Missing class ID!");
        return;
    }

    // Open edit modal
    openEditModal(classId);
});


function changePageSize(select) {
    pageSize = parseInt(select.value);
    loadPage(0, pageSize);
}

function loadPage(page, size) {
    fetch(`/api/Class`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Received data:", data);
            
            // Handle pagination on client side
            if (Array.isArray(data)) {
                totalPages = Math.ceil(data.length / size);
                currentPage = page;
                pageSize = size;
                
                // Get the slice of data for current page
                const startIndex = page * size;
                const endIndex = startIndex + size;
                const pageData = data.slice(startIndex, endIndex);
                
                renderTable(pageData);
                renderPagination();
            } else {
                console.error("Unexpected data format:", data);
                showToast("Error loading classes", "error");
            }
        })
        .catch(error => {
            console.error("Error fetching data:", error);
            showToast("Error loading classes: " + error.message, "error");
        });
}

function renderTable(classes) {
    let tableContent = document.getElementById("table-content");
    tableContent.innerHTML='';
    tableContent.innerHTML = classes.map(c => {
        // Handle null mentor safely
        const mentorName = c.mentor 
            ? `${c.mentor.first_name || ''} ${c.mentor.last_name || ''}`.trim() 
            : 'N/A';
        const mentorId = c.mentor ? c.mentor.id : 'unknown';
        
        return `
            <tr id="manager-${mentorId}" data-class-id="${c.id}">
                <td>${c.class_name || 'N/A'}</td>
                <td>${c.number_of_interns || 0}</td>
                <td>${mentorName}</td>
                <td>${c.status || 'N/A'}</td>
                <td>
                    <a href="#" class="toggle-class-status open-modal" data-id="${c.id}">
                        <i class="${c.is_active ? 'flaticon-padlock active' : 'flaticon-padlock inactive'}" onclick="toggleActiveStatus(this, event)"></i>
                    </a>
                    <a href="#" class="edit-class-status" data-class-id="${c.id}" onclick="openEditModal(${c.id}, event)">
                        <i class="flaticon-edit edit-class" style="cursor: pointer; color: blue;"></i>
                    </a>
                </td>
            </tr>
        `;
    }).join('');
}

// Add new function to open edit modal with data loading
function openEditModal(classId, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    
    console.log("Opening edit modal for class ID:", classId);
    
    // Show loading in form
    const form = document.getElementById("editClassForm");
    if (form) {
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => input.disabled = true);
    }
    
    // Show modal
    const editClassModal = new bootstrap.Modal(document.getElementById("editClassModal"));
    editClassModal.show();
    
    // Fetch class data
    fetch(`/api/Class/${classId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch class data');
            }
            return response.json();
        })
        .then(classData => {
            console.log("Loaded class data:", classData);
            
            // Populate form fields
            document.getElementById("editClassId").value = classData.id || '';
            document.getElementById("editClassName").value = classData.class_name || '';
            document.getElementById("editNumberOfInterns").value = classData.number_of_interns || 0;
            document.getElementById("editStatus").value = classData.status || 'NOT_STARTED';
            
            // Set mentor/manager
            if (classData.mentor_id) {
                document.getElementById("editManager").value = classData.mentor_id;
            }
            
            // Re-enable form
            if (form) {
                const inputs = form.querySelectorAll('input, select');
                inputs.forEach(input => input.disabled = false);
            }
        })
        .catch(error => {
            console.error("Error loading class data:", error);
            showToast("Error loading class data: " + error.message, "error");
            editClassModal.hide();
        });
}

// ...existing code...

function renderPagination() {
    let pagination = document.getElementById("pagination-list");
    pagination.innerHTML = '';

    if (currentPage > 0) {
        pagination.innerHTML += `<li class="pagination-item"><a href="#" onclick="loadPage(${currentPage - 1}, ${pageSize})" class="pagination-link pagination-arrow">&#8249;</a></li>`;
    }

    for (let i = 0; i < totalPages; i++) {
        if (i === 0 || i === totalPages - 1 || (i >= currentPage - 2 && i <= currentPage + 2)) {
            pagination.innerHTML += `
            <li class="pagination-item ${i === currentPage ? 'active' : ''}">
                <a href="#" onclick="loadPage(${i}, ${pageSize})" class="pagination-link">${i + 1}</a>
            </li>`;
        }
    }

    if (currentPage < totalPages - 1) {
        pagination.innerHTML += `<li class="pagination-item"><a href="#" onclick="loadPage(${currentPage + 1}, ${pageSize})" class="pagination-link pagination-arrow">&#8250;</a></li>`;
    }
}

function filterTable() {
    let input = document.getElementById("searchInput").value.toLowerCase();
    let table = document.getElementById("table-content");
    let rows = table.getElementsByTagName("tr");

    for (let i = 0; i < rows.length; i++) {
        let cells = rows[i].getElementsByTagName("td");
        let match = false;

        for (let j = 0; j < cells.length; j++) {
            if (cells[j].textContent.toLowerCase().includes(input)) {
                match = true;
                break;
            }
        }
        rows[i].style.display = match ? "" : "none";
    }
}

function sortTable(columnIndex) {
    let table = document.getElementById("table-content");
    let rows = Array.from(table.rows).slice(0);
    let isAscending = table.getAttribute("data-sort") !== "asc";

    rows.sort((rowA, rowB) => {
        let cellA = rowA.cells[columnIndex].textContent.trim().toLowerCase();
        let cellB = rowB.cells[columnIndex].textContent.trim().toLowerCase();

        if (!isNaN(cellA) && !isNaN(cellB)) {
            return isAscending ? cellA - cellB : cellB - cellA;
        } else {
            return isAscending ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA);
        }
    });

    rows.forEach(row => table.appendChild(row));
    table.setAttribute("data-sort", isAscending ? "asc" : "desc");
}

//Toast thông báo
function showToast(message, type = 'success') {
    const toast = document.getElementById('toastNotification');
    const toastMessage = document.getElementById('toastMessage');
    const toastIcon = toast.querySelector('.toast-icon');

    // Set message
    toastMessage.textContent = message;

    // Set appropriate icon and color based on type
    if (type === 'success') {
        toastIcon.className = 'fas fa-check-circle toast-icon';
        toast.style.borderLeftColor = '#28a745';
        toastIcon.style.color = '#28a745';
    } else if (type === 'error') {
        toastIcon.className = 'fas fa-exclamation-circle toast-icon';
        toast.style.borderLeftColor = '#dc3545';
        toastIcon.style.color = '#dc3545';
    } else if (type === 'warning') {
        toastIcon.className = 'fas fa-exclamation-triangle toast-icon';
        toast.style.borderLeftColor = '#fd7e14';
        toastIcon.style.color = '#fd7e14';
    }

    // Show the toast
    toast.style.display = 'block';

    // Reset the animation
    toast.style.animation = 'none';
    toast.offsetHeight; // Trigger reflow
    toast.style.animation = 'slideIn 0.3s, fadeOut 0.5s 2.5s forwards';

    // Hide the toast after 3 seconds
    setTimeout(() => {
        toast.style.display = 'none';
    }, 3000);
}