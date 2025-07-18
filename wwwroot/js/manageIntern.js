document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById("fileInput");
    const dropZone = document.getElementById("dropZone");
    const fileNameText = document.getElementById("fileName");
    const uploadBtn = document.getElementById("uploadFile");
    const downloadTemplateBtn = document.getElementById("downloadTemplate");
    const notificationToastEl = document.getElementById("notificationToast");
    const notificationMessage = document.getElementById("notificationMessage");
    const toastBootstrap = new bootstrap.Toast(notificationToastEl, {delay: 3000});

    let selectedFile = null;

    loadPage(0, 10);

    // Click to select file
    fileInput.addEventListener("change", function (event) {
        selectedFile = event.target.files[0];
        fileNameText.textContent = selectedFile ? `File đã chọn: ${selectedFile.name}` : "";
    });

    // Drag & Drop file
    dropZone.addEventListener("dragover", function (event) {
        event.preventDefault();
        dropZone.classList.add("border-primary");
    });

    dropZone.addEventListener("dragleave", function () {
        dropZone.classList.remove("border-primary");
    });

    dropZone.addEventListener("drop", function (event) {
        event.preventDefault();
        dropZone.classList.remove("border-primary");

        if (event.dataTransfer.files.length) {
            selectedFile = event.dataTransfer.files[0];
            fileNameText.textContent = `File đã chọn: ${selectedFile.name}`;
        }
    });

    // Upload file
    uploadBtn.addEventListener("click", function () {
        if (!selectedFile) {
            alert("Please choose a file before uploading");
            return;
        }

        const formData = new FormData();
        formData.append("file", selectedFile);

        fetch("/api/excel/import", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                notificationMessage.textContent = data.message;
                toastBootstrap.show();

                if (data.status === 200 && data.result !== "No new data added") {
                    setTimeout(() => window.location.reload(), 2000);
                }
            })
            .catch(error => {
                console.error("Upload error:", error);
                alert("Error occurs when uploading file: " + error.message);
            });
    });

    // Download template file
    downloadTemplateBtn.addEventListener("click", function () {
        window.location.href = "/assets/excel-template/Template-Employee.xlsx";
    });
});

document.addEventListener("click", async function (event) {
    let icon = event.target.closest(".flaticon-padlock"); // Get the clicked padlock icon
    if (!icon) return;

    event.preventDefault();
    event.stopPropagation();

    let userId = icon.getAttribute("data-id");
    let userEmail = icon.getAttribute("data-user-email"); // Get user email

    if (!userId) {
        showModal("Error", "Missing user ID!");
        return;
    }

    let isUnlock = icon.classList.contains("inactive");
    let action = isUnlock ? "unlock" : "lock";
    let duration;
    let reason;
    if (isUnlock) {
        let response = await fetch(`/api/user/get-status/${userId}`);
        let data = await response.json();
        duration = data.duration;
        reason = data.reason;
    }

    let message = isUnlock
        ? `Are you sure you want to unlock this user? \nThis user has been banned for ${duration/3600/24} days.\nReason: ${reason}`
        : `Provide details for locking this user.`;


    if (isUnlock) {
        showModal("Unlock User", message, true, function() {
            fetch(`/api/user/unban/${userId}`, {
                method: "DELETE",
                headers: {"Content-Type": "application/json"},
            })
                .then(response => {
                    if (response.ok) {
                        showModal("Success", "User has been unlocked!");
                        let icon = document.querySelector(`.flaticon-padlock[data-id="${userId}"]`);
                        if (icon) {
                            if (icon.classList.contains("inactive")) {
                                icon.classList.remove("inactive");
                                icon.classList.add("active");
                            } else if (icon.classList.contains("active")) {
                                icon.classList.remove("active");
                                icon.classList.add("inactive");
                            }
                        }
                    } else {
                        throw new Error("Failed to unlock user");
                    }
                })
                .catch(error => showModal("Error", `Error: ${error.message}`));
        });
    } else {
        showModal("Lock User", message, true, function () {
            let reason = document.getElementById("lockReason").value;
            let duration = document.getElementById("lockDuration").value;
            toggleUserStatus(userId, userEmail, action, reason, duration);
        }, true);
    }
});


function toggleUserStatus(userId, userEmail, action, reason, duration) {
    let url = `api/user/${action === "unlock" ? "unban" : "ban"}/${userId}`;
    let method = action === "unlock" ? "PATCH" : "POST";
    let body = action === "unlock" ? null : JSON.stringify({duration, reason});

    fetch(url, {
        method,
        headers: {"Content-Type": "application/json"},
        body
    })
        .then(response => response.ok ? response.text() : response.text().then(text => {
            throw new Error(text);
        }))
        .then(() => {
            if (action === "lock") {
                fetch("api/email/send-mail", {
                    method: "POST",
                    headers: {"Content-Type": "application/json"},
                    body: JSON.stringify({
                        recipient: userEmail,  // Use extracted email
                        msgBody: reason,
                        subject: `Your account has been banned for ${duration} days`,
                        attachment: null
                    })
                });
            } else if (action === "unlock") {
                fetch(`api/user/unban/${userId}`, {
                    method: "DELETE",
                    headers: {"Content-Type": "application/json"},
                });
            }

            showModal("Success", `User has been ${action}ed!`);
            let icon = document.querySelector(`.flaticon-padlock[data-id="${userId}"]`);
            if (icon) {
                if (icon.classList.contains("inactive")) {
                    icon.classList.remove("inactive");
                    icon.classList.add("active");
                } else if (icon.classList.contains("active")) {
                    icon.classList.remove("active");
                    icon.classList.add("inactive");
                }
            }
        })
        .catch(error => showModal("Error", `Error: ${error.message}`));
}

function showModal(title, message, isConfirm, confirmCallback = null, showInputs = false) {
    let modalElement = document.getElementById("genericModal");
    let modalTitle = document.getElementById("modalTitle");
    let modalMessage = document.getElementById("modalMessage");
    let confirmBtn = document.getElementById("modalConfirmBtn");
    let cancelBtn = document.getElementById("modalCancelBtn");
    let okBtn = document.getElementById("modalOkBtn");
    let lockInputs = document.getElementById("lockInputs");

    modalTitle.innerText = title;
    modalMessage.innerText = message;
    lockInputs.classList.toggle("d-none", !showInputs);

    if (isConfirm) {
        confirmBtn.classList.remove("d-none");
        cancelBtn.classList.remove("d-none");
        okBtn.classList.add("d-none");

        confirmBtn.onclick = function () {
            if (confirmCallback) confirmCallback();
            let modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            modalInstance.hide();
        };

        cancelBtn.onclick = function () {
            let modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            modalInstance.hide();
        };
    } else {
        confirmBtn.classList.add("d-none");
        cancelBtn.classList.add("d-none");
        okBtn.classList.remove("d-none");

        okBtn.onclick = function () {
            let modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            modalInstance.hide();
        };
    }

    let modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
    modalInstance.show();
}

function openEditUserModal(element, event) {
    event.preventDefault();
    let userId = element.getAttribute("data-id");

    // Gửi request để lấy thông tin user
    fetch(`/api/user/${userId}`)
        .then(response => response.json())
        .then(user => {
            document.getElementById("editUserId").value = user.id;
            document.getElementById("editFullName").value = user.first_name + " " + user.last_name;
            document.getElementById("editEmail").value = user.email;
            document.getElementById("editPhoneNumber").value = user.phone_number;
            document.getElementById("editGender").value = user.gender;

            // Hiển thị modal
            $('#editUserModal').modal('show');
        })
        .catch(error => showToast("Error for Fetch user: " + error, "warning"));

}

function updateUser() {
    let fullNameInput = document.getElementById("editFullName");
    let emailInput = document.getElementById("editEmail");
    let phoneNumberInput = document.getElementById("editPhoneNumber");
    let genderInput = document.getElementById("editGender");

    let userId = document.getElementById("editUserId").value;
    let fullName = fullNameInput.value.trim();
    let email = emailInput.value.trim();
    let phoneNumber = phoneNumberInput.value.trim();

    let isValid = true;

    clearErrors();
    document.querySelectorAll(".error-message").forEach(e => e.remove());
    [fullNameInput, emailInput, phoneNumberInput].forEach(input => input.classList.remove("is-invalid"));


    if (fullName === "") {
        showError(fullNameInput, "This field is required");
        isValid = false;
    }
    if (email === "") {
        showError(emailInput, "This field is required");
        isValid = false;
    }
    if (phoneNumber === "") {
        showError(phoneNumberInput, "This field is required");
        isValid = false;
    }

    if (!isValid) return;

    let updatedUser = {
        first_name: fullName.split(" ")[0],
        last_name: fullName.split(" ").slice(1).join(" "),
        email: email,
        phone_number: phoneNumber,
        gender: genderInput.value,
    };

    fetch(`/api/user/update/`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(updatedUser)
    })
        .then(response => response.json())
        .then(user => {
            $('#editUserModal').modal('hide');
            location.reload();
        })
        .catch(error => showToast("Error updating user: " + error, "error"));
}



function showError(input, message) {
    let error = document.createElement("div");
    error.className = "error-message text-danger mt-1";
    error.innerText = message;
    input.parentNode.appendChild(error);
    input.classList.add("is-invalid");
}

function clearErrors() {
    let inputs = document.querySelectorAll(".is-invalid");
    inputs.forEach(input => input.classList.remove("is-invalid"));

    let errorMessages = document.querySelectorAll(".invalid-feedback");
    errorMessages.forEach(error => error.remove());
}

function filterTable() {
    let input = document.getElementById("searchInput");
    let filter = input.value.toLowerCase();
    let table = document.getElementById("table-body");
    let rows = table.getElementsByTagName("tr");

    for (let i = 0; i < rows.length; i++) {
        let cells = rows[i].getElementsByTagName("td");
        let match = false;

        for (let j = 0; j < cells.length - 1; j++) {
            if (cells[j].textContent.toLowerCase().includes(filter)) {
                match = true;
                break;
            }
        }
        rows[i].style.display = match ? "" : "none";
    }
}

function sortTable(columnIndex) {
    let table = document.getElementById("table-body");
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

function changePageSize(select) {
    pageSize = parseInt(select.value);
    loadPage(0, pageSize);
}

function loadPage(page, size) {
    fetch(`/api/user/role/INTERN?page=${page}&&size=${size}`)
        .then(response => response.json())
        .then(data => {
            console.log(data);
            totalPages = data.totalPages;
            currentPage = data.number;
            pageSize = data.size;
            renderTable(data.items);
            renderPagination();
        })
}

function renderTable(interns) {
    let tableContent = document.getElementById("table-body");
    tableContent.innerHTML = '';

    tableContent.innerHTML = interns.map(user => `
        <tr>
            <td>${user.first_name} ${user.last_name}</td>
            <td>${user.email}</td>
            <td>${user.class_id}</td>
            <td>${user.phone_number}</td>
            <td>${user.gender}</td>
            <td>${user.role}</td>
            <td class="${user.active ? 'active' : 'inactive'}">
                <a href="#" data-id="${user.id}" class="toggle-class-status">
                    <i data-id="${user.id}" data-user-email="${user.email}"
                       class="flaticon-padlock ${user.active ? 'active' : 'inactive'}">
                    </i>
                </a>
                <a href="#" class="edit-user-modal" data-id="${user.id}"
                   onclick="openEditUserModal(this, event)">
                    <i class="flaticon-edit edit-user-modal" style="cursor: pointer"></i>
                </a>
            </td>
        </tr>
    `).join('');
}

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
function showReadonlyToast() {
    showToast("Email can not be edited", "warning");
}
