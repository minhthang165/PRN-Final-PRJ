// Function to fetch Classrooms and populate the select
function populateClassroomSelect() {
    fetch('/api/Class') // Fetch từ endpoint /api/classrooms/all
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch classrooms');
            }
            return response.json();
        })
        .then(classrooms => {
            const classSelect = document.getElementById('classId');
            classSelect.innerHTML = '<option value="">Select a Classroom</option>'; // Reset options
            classrooms.forEach(classroom => {
                const option = document.createElement('option');
                option.value = classroom.id;
                option.textContent = classroom.className || `Class ${classroom.id}`; // Hiển thị name hoặc id
                classSelect.appendChild(option);
            });
        })
        .catch(error => {
            console.error('Error fetching classrooms:', error);
            const classSelect = document.getElementById('classId');
            classSelect.innerHTML = '<option value="">Error loading classrooms</option>';
        });
}

// Function to open the Add Requirement modal
function openAddRequirementModal() {
    const modal = document.getElementById("addRequirementModal");
    if (!modal) {
        console.error("Modal with ID 'addRequirementModal' not found in HTML");
        return;
    }
    modal.style.display = "block";
    document.body.classList.add('modal-open');

    // Set default values
    const endDateInput = document.getElementById("endTime");
    const thirtyDaysLater = new Date();
    thirtyDaysLater.setDate(thirtyDaysLater.getDate() + 30);
    endDateInput.value = thirtyDaysLater.toISOString().split('T')[0];

    document.getElementById("totalSlot").value = 1;
    document.getElementById("minGPA").value = 2.5;
    document.getElementById("experienceRequirement").value = "No experience required";
    document.getElementById("language").value = "English";

    // Fetch và điền danh sách classrooms khi mở modal
    populateClassroomSelect();
}

// Function to close the Add Requirement modal
function closeAddRequirementModal() {
    const modal = document.getElementById("addRequirementModal");
    if (modal) {
        modal.style.display = "none";
        document.body.classList.remove('modal-open');
        document.getElementById("requirementForm").reset();
    }
}

// Function to validate form data
function validateFormData(formDataObj) {
    const errors = [];

    // Validate minGPA
    const minGPA = parseFloat(formDataObj.minGPA);
    if (isNaN(minGPA) || minGPA < 0 || minGPA > 4) {
        errors.push("Min GPA must be a number between 0.0 and 4.0");
    }

    // Validate totalSlot
    const totalSlot = parseInt(formDataObj.totalSlot);
    if (isNaN(totalSlot) || totalSlot <= 0) {
        errors.push("Total Slot must be a positive integer greater than 0");
    }

    // Validate endTime
    const endTime = new Date(formDataObj.endTime);
    const now = new Date();
    if (isNaN(endTime.getTime()) || endTime <= now) {
        errors.push("End Time must be a valid date in the future");
    }

    return errors;
}

// Handle form submission
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById("requirementForm");
    if (!form) {
        console.error("Form with ID 'requirementForm' not found in HTML");
        return;
    }

    form.addEventListener('submit', function (event) {
        event.preventDefault();

        const saveButton = document.getElementById("saveButton");
        saveButton.disabled = true;
        saveButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';

        const formData = new FormData(form);
        const formDataObj = {};
        formData.forEach((value, key) => {
            formDataObj[key] = value;
        });

        // Đảm bảo các field mặc định
        formDataObj.experience = formDataObj.experience || "No experience required";
        formDataObj.description = formDataObj.description || "No description provided";

        // Validate dữ liệu
        const validationErrors = validateFormData(formDataObj);
        if (validationErrors.length > 0) {
            showToast("Validation errors: " + validationErrors.join(", "), "error");
            saveButton.disabled = false;
            saveButton.innerHTML = 'Save';
            return;
        }

        // Log dữ liệu gửi đi
        console.log('Sending data:', formDataObj);

        fetch('/api/Recruitment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formDataObj)
        })
            .then(response => {
                console.log('Response status:', response.status);
                return response.text().then(text => {
                    console.log('Response text:', text);
                    if (!response.ok) {
                        throw new Error(text || 'Failed to create requirement');
                    }
                    return text ? JSON.parse(text) : {success: true, message: "Requirement created successfully"};
                });
            })
            .then(data => {
                console.log('Success:', data);
                showToast("Requirement added successfully!", "success");
                closeAddRequirementModal();
                setTimeout(() => window.location.reload(), 1500);
            })
            .catch(error => {
                console.error('Error:', error);
                showToast("Error: " + error.message, "error");
                saveButton.disabled = false;
                saveButton.innerHTML = 'Save';
            });
    });
});

// Update the existing "Add Requirement +" button
document.addEventListener('DOMContentLoaded', function () {
    const addButton = document.querySelector(".element__btn.yellow-bg");
    if (addButton) {
        addButton.removeAttribute("data-bs-toggle");
        addButton.removeAttribute("data-bs-target");
        addButton.onclick = openAddRequirementModal;
    }
});

// Make sure DOM is fully loaded before accessing elements
document.addEventListener('DOMContentLoaded', function () {
    loadPage(1, 5);
    // Fix table reference in filter function
    const searchInput = document.getElementById("searchInput");
    if (searchInput) {
        searchInput.addEventListener('keyup', filterTable);
    }
});

// Thêm xử lý tìm kiếm ứng viên
function setupCandidateSearch() {
    const searchButton = document.getElementById('searchCandidateButton');
    const searchInput = document.getElementById('searchCandidateInput');

    searchButton.addEventListener('click', function () {
        searchCandidates();
    });

    searchInput.addEventListener('keyup', function (event) {
        if (event.key === 'Enter') {
            searchCandidates();
        }
    });
}

function searchCandidates() {
    const searchTerm = document.getElementById('searchCandidateInput').value.toLowerCase().trim();

    if (!window.allCandidates) {
        return;
    }

    // Nếu không có từ khóa tìm kiếm, hiển thị tất cả
    if (!searchTerm) {
        displayCandidates(window.allCandidates, window.currentRecruitmentId);
        return;
    }

    // Lọc danh sách ứng viên theo từ khóa
    const filteredCandidates = window.allCandidates.filter(candidate => {
        return (
            (candidate.name && candidate.name.toLowerCase().includes(searchTerm)) ||
            (candidate.education && candidate.education.toLowerCase().includes(searchTerm)) ||
            (candidate.skill && candidate.skill.toLowerCase().includes(searchTerm))
        );
    });

    // Hiển thị kết quả tìm kiếm
    if (filteredCandidates.length === 0) {
        const tableBody = document.getElementById("candidateTableBody");
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center">No candidates found matching your search criteria</td></tr>';

        // Xóa phân trang nếu có
        const existingPagination = document.getElementById('candidatePagination');
        if (existingPagination) {
            existingPagination.remove();
        }
    } else {
        displayCandidates(filteredCandidates, window.currentRecruitmentId);
    }
}

function openCandidateModal(row) {
    // Get the data from the clicked row
    console.log(row);
    const name = row.cells[0].textContent;
    const position = row.cells[1].textContent;
    const recruitmentId = row.getAttribute('data-id');

    const modal = document.getElementById("candidateModal");
    const modalTitle = document.getElementById("modalTitle");
    modalTitle.textContent = name + " - " + position + " Candidates";

    // Load candidates
    loadCandidates(recruitmentId);

    // Set up search functionality
    setupCandidateSearch();

    // Add blur effect to sidebar and content
    document.body.classList.add('modal-open');

    // Display the modal
    modal.style.display = "block";

    // Prevent event bubbling
    event.stopPropagation();
}

function closeModal() {
    const modal = document.getElementById("candidateModal");
    modal.style.display = "none";

    // Remove blur effect
    document.body.classList.remove('modal-open');
}

function loadCandidates(recruitmentId) {
    // Fetch CV_Info data from the server based on the recruitment ID
    const tableBody = document.getElementById("candidateTableBody");
    tableBody.innerHTML = '<tr><td colspan="6" class="text-center">Loading candidates...</td></tr>';

    // Call the API to get CV_Info data
    fetch('/api/CVInfo/recruitment/' + recruitmentId)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch candidates: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            displayCandidates(data, recruitmentId);
        })
        .catch(error => {
            console.error('Error fetching candidates:', error);
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Error loading candidates: ' + error.message + '</td></tr>';

            // For demo purposes, load sample data if API fails
            setTimeout(() => {
                loadSampleCandidates(recruitmentId);
            }, 1000);
        });
}


function displayCandidates(candidates, recruitmentId) {
    const tableBody = document.getElementById("candidateTableBody");
    tableBody.innerHTML = "";

    candidates = candidates.filter(candidate => candidate.isActive !== false);

    if (!candidates || candidates.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center">No candidates found for this requirement</td></tr>';
        return;
    }

    // Thêm biến phân trang - thay đổi từ 10 xuống 5
    const pageSize = 5;
    const totalCandidates = candidates.length;
    const totalPages = Math.ceil(totalCandidates / pageSize);
    let currentPage = 1;

    // Lưu toàn bộ dữ liệu ứng viên vào biến toàn cục để sử dụng khi chuyển trang
    window.allCandidates = candidates;
    window.currentRecruitmentId = recruitmentId;

    // Hiển thị chỉ 5 ứng viên của trang hiện tại
    displayCandidatesPage(currentPage, pageSize, recruitmentId);

    // Tạo phân trang nếu có nhiều hơn 5 ứng viên
    if (totalCandidates > pageSize) {
        createPagination(totalPages, currentPage);
    }

    // Thêm thông báo trạng thái cho người dùng
    addCandidateStatusMessage(recruitmentId);
}

function displayCandidatesPage(page, pageSize, recruitmentId) {
    const tableBody = document.getElementById("candidateTableBody");
    tableBody.innerHTML = "";

    // Lấy danh sách ứng viên cho trang hiện tại
    const startIndex = (page - 1) * pageSize;
    const endIndex = Math.min(startIndex + pageSize, window.allCandidates.length);
    const candidatesForPage = window.allCandidates.slice(startIndex, endIndex);

    candidatesForPage.forEach(candidate => {
        const row = document.createElement("tr");

        // Name
        const nameCell = document.createElement("td");
        const fullName = `${candidate.first_name || ''} ${candidate.last_name || ''}`.trim();
        nameCell.textContent = fullName || "Unknown";
        row.appendChild(nameCell);

        // GPA
        const gpaCell = document.createElement("td");
        const gpaSpan = document.createElement("span");
        gpaSpan.textContent = candidate.gpa;

        if (candidate.gpa >= 3.7) {
            gpaSpan.className = "gpa gpa-high";
        } else if (candidate.gpa >= 3.0) {
            gpaSpan.className = "gpa gpa-medium";
        } else {
            gpaSpan.className = "gpa gpa-low";
        }

        gpaCell.appendChild(gpaSpan);
        row.appendChild(gpaCell);

        // Education
        const eduCell = document.createElement("td");
        eduCell.textContent = candidate.education || "Not specified";
        row.appendChild(eduCell);

        // Skills
        const skillsCell = document.createElement("td");
        const skillsContainer = document.createElement("div");
        skillsContainer.className = "skills-container";

        // Split the skills string into an array
        let skills = [];
        if (typeof candidate.skill === 'string') {
            skills = candidate.skill.split(',').map(s => s.trim()).filter(s => s);
        }

        // Nếu không có kỹ năng, thêm thông báo
        if (skills.length === 0) {
            const pill = document.createElement("span");
            pill.className = "skill-pill";
            pill.style.backgroundColor = "#f8f9fa";
            pill.style.color = "#6c757d";
            pill.textContent = "No skills specified";
            skillsContainer.appendChild(pill);
        } else {
            skills.forEach(skill => {
                const pill = document.createElement("span");
                pill.className = "skill-pill";
                pill.textContent = skill;
                skillsContainer.appendChild(pill);
            });
        }

        skillsCell.appendChild(skillsContainer);
        row.appendChild(skillsCell);

        // File Path Column
         const filePathCell = document.createElement("td");
        // Sửa lại thuộc tính thành candidate.path
        filePathCell.innerHTML = `<a href="${candidate.path}" target="_blank" class="text-blue-500 hover:text-blue-700">View</a>`;
        row.appendChild(filePathCell);

        // Actions
        const actionCell = document.createElement("td");

        // Create action icons container
        const actionIcons = document.createElement("div");
        actionIcons.className = "action-icons";

        // Create accept button
        const acceptButton = document.createElement("button");
        acceptButton.className = "action-icon check-icon";
        acceptButton.title = "Approve Candidate";
        acceptButton.innerHTML = '<i class="fas fa-solid fa-check"></i>';
        acceptButton.onclick = function () {
            approveCandidate(candidate.fileId, recruitmentId);
        };

        // Create reject button
        const rejectButton = document.createElement("button");
        rejectButton.className = "action-icon x-icon";
        rejectButton.title = "Reject Candidate";
        rejectButton.innerHTML = '<i class="fas fa-times"></i>';
        rejectButton.onclick = function () {
            console.log("Reject button clicked for fileId:", candidate.fileId);
            rejectCandidate(candidate.fileId, recruitmentId);
        };

        // Add buttons to container
        actionIcons.appendChild(acceptButton);
        actionIcons.appendChild(rejectButton);

        // Add container to cell
        actionCell.appendChild(actionIcons);

        // Add cell to row
        row.appendChild(actionCell);

        // Add row to table
        tableBody.appendChild(row);
    });
}

function rejectCandidate(cvId) {
    fetch(`/api/CVInfo/reject-cv?cvId=${cvId}`, {
        method: 'POST'
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => { throw new Error(err.message || 'Rejection failed') });
        }
        return response.json();
    })
    .then(data => {
        console.log(data.message);
        showToast('Candidate rejected successfully!', 'success');
    })
    .catch(error => {
        console.error('Error rejecting candidate:', error);
        showToast(`Error: ${error.message}`, 'error');
    });
}

function createPagination(totalPages, currentPage) {
    // Xóa phân trang cũ nếu có
    const existingPagination = document.getElementById('candidatePagination');
    if (existingPagination) {
        existingPagination.remove();
    }

    // Tạo container cho phân trang
    const paginationContainer = document.createElement('div');
    paginationContainer.id = 'candidatePagination';
    paginationContainer.className = 'pagination-container mt-3 d-flex justify-content-center';

    // Tạo ul element cho nút phân trang
    const paginationList = document.createElement('ul');
    paginationList.className = 'pagination';

    // Nút Previous
    const prevLi = document.createElement('li');
    prevLi.className = 'page-item ' + (currentPage === 1 ? 'disabled' : '');
    const prevLink = document.createElement('a');
    prevLink.className = 'page-link';
    prevLink.href = '#';
    prevLink.textContent = 'Previous';
    prevLink.onclick = function (e) {
        e.preventDefault();
        if (currentPage > 1) {
            goToPage(currentPage - 1);
        }
    };
    prevLi.appendChild(prevLink);
    paginationList.appendChild(prevLi);

    // Tạo các nút số trang
    const maxVisiblePages = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

    // Điều chỉnh lại startPage nếu cần
    if (endPage - startPage + 1 < maxVisiblePages) {
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = 'page-item ' + (i === currentPage ? 'active' : '');
        const pageLink = document.createElement('a');
        pageLink.className = 'page-link';
        pageLink.href = '#';
        pageLink.textContent = i;
        pageLink.onclick = function (e) {
            e.preventDefault();
            goToPage(i);
        };
        pageLi.appendChild(pageLink);
        paginationList.appendChild(pageLi);
    }

    // Nút Next
    const nextLi = document.createElement('li');
    nextLi.className = 'page-item ' + (currentPage === totalPages ? 'disabled' : '');
    const nextLink = document.createElement('a');
    nextLink.className = 'page-link';
    nextLink.href = '#';
    nextLink.textContent = 'Next';
    nextLink.onclick = function (e) {
        e.preventDefault();
        if (currentPage < totalPages) {
            goToPage(currentPage + 1);
        }
    };
    nextLi.appendChild(nextLink);
    paginationList.appendChild(nextLi);

    // Thêm phân trang vào container
    paginationContainer.appendChild(paginationList);

    // Thêm vào DOM sau bảng
    const cardElement = document.querySelector('.modal .card');
    cardElement.appendChild(paginationContainer);
}

function goToPage(page) {
    // Cập nhật trang hiện tại và hiển thị lại dữ liệu
    window.currentPage = page;
    displayCandidatesPage(page, 5, window.currentRecruitmentId);

    // Cập nhật UI phân trang
    const totalPages = Math.ceil(window.allCandidates.length / 5);
    createPagination(totalPages, page);
}

function approveCandidate(cvId) {
    fetch(`/api/CVInfo/approve-cv?cvId=${cvId}`, {
        method: 'POST',
        headers: {
            // Không cần 'Content-Type' vì không có body
        }
    })
    .then(response => {
        if (!response.ok) {
            // Ném lỗi nếu server trả về status không thành công
            return response.json().then(err => { throw new Error(err.message || 'Approval failed') });
        }
        return response.json();
    })
    .then(data => {
        console.log(data.message); // Sẽ log ra: "Chấp thuận cv thành công"
        showToast('Candidate approved successfully!', 'success');
        // Thêm logic để cập nhật giao diện, ví dụ: xóa ứng viên khỏi danh sách
    })
    .catch(error => {
        console.error('Error approving candidate:', error);
        showToast(`Error: ${error.message}`, 'error');
    });
}

/**
 * Shows a toast notification
 * @param {string} message - The message to display
 * @param {string} type - The type of toast ('success' or 'error')
 */
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

/**
 * Xóa ứng viên đã được approve khỏi danh sách hiển thị
 */
function removeApprovedCandidate(fileId) {
    // Xóa ứng viên khỏi danh sách toàn cục
    if (window.allCandidates) {
        window.allCandidates = window.allCandidates.filter(candidate => candidate.fileId != fileId);

        // Cập nhật lại danh sách hiển thị
        const pageSize = 5;
        const totalCandidates = window.allCandidates.length;
        const totalPages = Math.ceil(totalCandidates / pageSize);

        // Kiểm tra nếu trang hiện tại lớn hơn tổng số trang sau khi xóa
        if (window.currentPage > totalPages && totalPages > 0) {
            window.currentPage = totalPages;
        } else if (totalPages === 0) {
            window.currentPage = 1;
        }

        // Cập nhật hiển thị
        displayCandidatesPage(window.currentPage || 1, pageSize, window.currentRecruitmentId);

        // Cập nhật phân trang
        if (totalCandidates > pageSize) {
            createPagination(totalPages, window.currentPage || 1);
        } else {
            // Xóa phân trang nếu không đủ ứng viên
            const existingPagination = document.getElementById('candidatePagination');
            if (existingPagination) {
                existingPagination.remove();
            }
        }

        // Hiển thị thông báo nếu không còn ứng viên
        if (totalCandidates === 0) {
            const tableBody = document.getElementById("candidateTableBody");
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center">No candidates found for this requirement</td></tr>';
        }
    }
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

function loadPage(page, size) {
    fetch(`/api/Recruitment/paging?page=${page}&&size=${size}`)
        .then(response => response.json())
        .then(data => {
            totalPages = data.totalPages;
            currentPage = data.pageNumber;
            pageSize = data.pageSize;
            renderTable(data.items);
            renderPagination();
        })


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

function renderTable(requirements) {
    let tableContent = document.getElementById("table-content");
    tableContent.innerHTML = '';
    if (!Array.isArray(requirements)) {
        console.error("Lỗi: renderTable mong đợi một mảng, nhưng nhận được:", requirements);
        tableContent.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Lỗi tải dữ liệu.</td></tr>';
        return; 
    }

    // Nếu là mảng rỗng, hiển thị thông báo
    if (requirements.length === 0) {
        tableContent.innerHTML = '<tr><td colspan="7" class="text-center">Không tìm thấy dữ liệu.</td></tr>';
        return;
    }
    tableContent.innerHTML = requirements.map(r => `
        <tr class="requirement-row" onclick="openCandidateModal(this)" data-id="${r.id}">
            <td>${r.name}</td>
            <td>${r.position}</td>
            <td>${r.experience_requirement}</td>
            <td>${r.language}</td>
            <td>${r.min_GPA}</td>
            <td>${new Date(r.end_time).toLocaleDateString('en-GB')}</td>
            <td>
                <a href="#" data-id="${r.id}" class="edit-recruitment-modal"
                   onclick="openEditRecruitmentModal(this, event)">
                    <i class="flaticon-edit" style="cursor: pointer"></i>
                </a>
                <a href="#" data-id="${r.id}" class="delete-recruitment-modal"
                   onclick="confirmDeleteRecruitment(this, event)">
                    <i class="flaticon-padlock" style="cursor: pointer; color: red"></i>
                </a>
            </td>
        </tr>
    `).join('');
}

function changePageSize(select) {
    pageSize = parseInt(select.value);
    loadPage(0, pageSize);
}

// Close modal when clicking outside the modal content
window.onclick = function (event) {
    const modal = document.getElementById("candidateModal");
    if (event.target === modal) {
        closeModal();
    }
}

// Thêm thông báo về trạng thái hiển thị CV
function addCandidateStatusMessage(recruitmentId) {
    // Xóa thông báo cũ nếu có
    const existingMessage = document.getElementById('candidateStatusMessage');
    if (existingMessage) {
        existingMessage.remove();
    }

    // Tạo phần tử thông báo
    const messageDiv = document.createElement('div');
    messageDiv.id = 'candidateStatusMessage';
    messageDiv.className = 'alert alert-info mt-2';
    messageDiv.style.fontSize = '0.9rem';
    messageDiv.innerHTML = '<i class="fas fa-info-circle"></i> Only showing CV candidates that have not been approved yet.';

    // Thêm vào DOM
    const modalCard = document.querySelector('.modal .card');
    modalCard.parentNode.insertBefore(messageDiv, modalCard);
}

let recruitmentToDelete = null

// Function to open the edit recruitment modal
function openEditRecruitmentModal(element, event) {
    event.stopPropagation() // Prevent row click event

    const recruitmentId = element.getAttribute("data-id")

    // Fetch recruitment details
    fetch(`/api/recruitment/${recruitmentId}`)
        .then((response) => {
            if (!response.ok) {
                throw new Error("Failed to fetch recruitment details")
            }
            return response.json()
        })
        .then((recruitment) => {
            // Populate the form fields
            document.getElementById("editRecruitmentId").value = recruitment.id
            document.getElementById("editName").value = recruitment.name
            document.getElementById("editPosition").value = recruitment.position
            document.getElementById("editExperienceRequirement").value = recruitment.experienceRequirement || ""
            document.getElementById("editLanguage").value = recruitment.language || ""
            document.getElementById("editMinGPA").value = recruitment.minGPA
            document.getElementById("editTotalSlot").value = recruitment.totalSlot

            // Format date for the date input (YYYY-MM-DD)
            const endDate = new Date(recruitment.endTime)
            const formattedDate = endDate.toISOString().split("T")[0]
            document.getElementById("editEndTime").value = formattedDate

            // Show the modal
            const editModal = new bootstrap.Modal(document.getElementById("editRecruitmentModal"))
            editModal.show()
        })
        .catch((error) => {
            console.error("Error fetching recruitment details:", error)
            alert("Error loading recruitment details: " + error.message)
        })
}

// Function to save recruitment changes
function saveRecruitmentChanges() {
    const recruitmentId = document.getElementById("editRecruitmentId").value

    // Create recruitment object from form data
    const recruitment = {
        id: recruitmentId,
        name: document.getElementById("editName").value,
        position: document.getElementById("editPosition").value,
        experienceRequirement: document.getElementById("editExperienceRequirement").value,
        language: document.getElementById("editLanguage").value,
        minGPA: Number.parseFloat(document.getElementById("editMinGPA").value),
        totalSlot: Number.parseInt(document.getElementById("editTotalSlot").value),
        endTime: document.getElementById("editEndTime").value,
    }

    // Send update request
    fetch(`/api/recruitment/update/${recruitmentId}`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(recruitment),
    })
        .then((response) => {
            if (!response.ok) {
                return response.text().then((text) => {
                    throw new Error(text || "Failed to update recruitment")
                })
            }
            return response.json()
        })
        .then((data) => {
            // Close the modal
            const editModalElement = document.getElementById("editRecruitmentModal")
            const editModal = bootstrap.Modal.getInstance(editModalElement)
            editModal.hide()

            // Show success message
            showToast("Recruitment updated successfully!", "success")

            // Reload the page to show updated data
            window.location.reload()
        })
        .catch((error) => {
            console.error("Error updating recruitment:", error)
            showToast("Error updating recruitment: " + error.message, "warning")
        })
}

// Function to open delete confirmation modal
function confirmDeleteRecruitment(element, event) {
    event.stopPropagation() // Prevent row click event

    recruitmentToDelete = element.getAttribute("data-id")

    // Get the recruitment name from the table row
    const row = element.closest("tr")
    const recruitmentName = row.cells[0].textContent

    // Set the recruitment name in the confirmation modal
    document.getElementById("deleteRecruitmentName").textContent = recruitmentName

    // Show the modal
    const deleteModal = new bootstrap.Modal(document.getElementById("deleteRecruitmentModal"))
    deleteModal.show()
}

// Function to delete recruitment
function deleteRecruitment() {
    if (!recruitmentToDelete) {
        alert("No recruitment selected for deletion")
        return
    }

    // Send delete request
    fetch(`/api/recruitment/delete/${recruitmentToDelete}`, {
        method: "DELETE",
    })
        .then((response) => {
            if (!response.ok) {
                return response.text().then((text) => {
                    throw new Error(text || "Failed to delete recruitment")
                })
            }
            return response.text()
        })
        .then((data) => {
            // Close the modal
            const deleteModalElement = document.getElementById("deleteRecruitmentModal")
            const deleteModal = bootstrap.Modal.getInstance(deleteModalElement)
            deleteModal.hide()

            // Show success message
            alert("Recruitment deleted successfully!")

            // Reload the page to update the table
            window.location.reload()
        })
        .catch((error) => {
            console.error("Error deleting recruitment:", error)
            alert("Error deleting recruitment: " + error.message)
        })
}

// Add event listeners when the DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    // Save changes button in edit modal
    const saveChangesBtn = document.getElementById("saveRecruitmentChanges")
    if (saveChangesBtn) {
        saveChangesBtn.addEventListener("click", saveRecruitmentChanges)
    }

    // Confirm delete button in delete modal
    const confirmDeleteBtn = document.getElementById("confirmDeleteRecruitment")
    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener("click", deleteRecruitment)
    }
})
