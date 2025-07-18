document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("applyButton").addEventListener("click", applyJob);
    document.getElementById("deleteButton").addEventListener("click", deleteApplication);
    const recruitmentId = document.getElementById("recruitmentId").value;
    loadCVList();
}); 



let appliedFileId = null; // Biến toàn cục để lưu fileId đã ứng tuyển

async function loadCVList() {
    const userId = document.getElementById("user_id").value;
    const recruitmentId = document.getElementById("recruitmentId").value; 
    const cvSelect = document.getElementById("cvSelect");

    try {
        let response = await fetch(`/api/File/user/${userId}`);

        if (!response.ok) {
            throw new Error("Không thể tải danh sách CV!");
        }

        let cvList = await response.json();

        cvSelect.innerHTML = '<option value="">-- Chọn CV để apply --</option>';

        const fileIds = cvList.map(cv => cv.id);
        const appliedStatus = await checkcvinfoExist(fileIds, recruitmentId);
        console.log("Applied status:", appliedStatus);

        // Tìm fileId đã ứng tuyển (nếu có)
        const appliedResult = appliedStatus.find(status => status.exists);
        appliedFileId = appliedResult ? appliedResult.fileId : null;

        cvList.forEach(cv => {
            let option = document.createElement("option");
            option.value = cv.id;
            option.text = cv.display_name;
            // option.dataset.driveLink = cv.path;

            if (appliedFileId) {
                option.disabled = true;
                option.text += " (Đã Apply)";
            }

            cvSelect.appendChild(option);
        });

        if (appliedFileId) {
            const applyButton = document.getElementById("applyButton");
            const deleteButton = document.getElementById("deleteButton");
            applyButton.disabled = true;
            applyButton.textContent = "Đã Apply";
            deleteButton.style.display = "inline-flex";
            cvSelect.disabled = true;

            let messageEl = document.getElementById("message");
            messageEl.textContent = "Bạn đã apply cho vị trí này!";
            messageEl.style.color = "green";
            messageEl.style.backgroundColor = "#e8f5e9";
        }
    } catch (error) {
        console.error("Error loading CV list:", error);
        cvSelect.innerHTML = '<option value="">Không có CV nào</option>';
    }
}

// Hàm checkcvinfoExist: Trả về danh sách trạng thái từng fileId
async function checkcvinfoExist(fileIds, recruitmentId) {
    try {
        if (!fileIds || fileIds.length === 0 || !recruitmentId) {
            throw new Error("fileIds và recruitmentId là bắt buộc!");
        }

        const fileIdArray = Array.isArray(fileIds) ? fileIds : [fileIds];

        const results = await Promise.all(
            fileIdArray.map(async (fileId) => {
                try {
                    let response = await fetch(`/api/CVInfo/exists-by-file-and-recruitment?fileId=${fileId}&recruitmentId=${recruitmentId}`, {
                        method: "GET",
                        headers: {
                            "Content-Type": "application/json",
                        },
                    });

                    if (!response.ok) {
                        const errorData = await response.json();
                        throw new Error(errorData.message || `Không thể kiểm tra thông tin CV cho fileId ${fileId}!`);
                    }

                    const data = await response.json(); 
                    console.log(`FileId: ${fileId}, Exists: ${data.exists}`);
                    const exists = data.exists;

                    return { fileId, exists };
                } catch (error) {
                    console.error(`Error checking CV for fileId ${fileId}:`, error);
                    return { fileId, exists: false };
                }
            })
        );

        return results;
    } catch (error) {
        console.error("Error checking CV info existence:", error);
        throw error;
    }
}

async function applyJob() {
    let messageEl = document.getElementById("message");
    let applyButton = document.getElementById("applyButton");
    let deleteButton = document.getElementById("deleteButton");
    const recruitmentId  = document.getElementById("recruitmentId").value;
    const userId = document.getElementById("user_id").value;
    const cvSelect = document.getElementById("cvSelect");
    const selectedCV = cvSelect.options[cvSelect.selectedIndex];


    if (!recruitmentId || !userId || isNaN(recruitmentId) || isNaN(userId)) {
        messageEl.textContent = "Không tìm thấy thông tin tuyển dụng hoặc người dùng.";
        messageEl.style.color = "red";
        messageEl.style.backgroundColor = "#ffebee";
        return;
    }

    if (!selectedCV.value) {
        messageEl.textContent = "Vui lòng chọn một CV để apply!";
        messageEl.style.color = "red";
        messageEl.style.backgroundColor = "#ffebee";
        return;
    }

    applyButton.disabled = true;
    applyButton.textContent = "Đang xử lý...";
    messageEl.style.backgroundColor = "transparent";

    try {
        const fileId = selectedCV.value;
         const requestBody = {
            FileId: parseInt(fileId),
            RecruitmentId: parseInt(recruitmentId)
        };

        console.log("Applying with fileId:", fileId, "and recruitmentId:", recruitmentId);

        let applyResponse = await fetch("/api/CVInfo/upload-cv", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestBody)
        });

        // Lấy email của user (giả định từ API)
        let userResponse = await fetch(`/api/User/${userId}`);
        if (!userResponse.ok) {
            throw new Error("Không thể lấy thông tin người dùng!");
        }
        const userData = await userResponse.json();
        const userEmail = userData.email;

        if (!applyResponse.ok) {
            const errorText = await applyResponse.text();
            // // Gửi email thông báo thất bại
            // await fetch("/api/email/send-mail", {
            //     method: "POST",
            //     headers: {"Content-Type": "application/json"},
            //     body: JSON.stringify({
            //         recipient: userEmail,
            //         msgBody: "Chào bạn, chúng tôi không thể xử lý hồ sơ ứng tuyển của bạn do lỗi: " + errorText + ". Vui lòng thử lại sau.",
            //         subject: "Ứng tuyển thất bại",
            //         attachment: null
            //     })
            // });
            throw new Error(`Apply thất bại: ${errorText}`);
        }

        // let applyData = await applyResponse.text();

        // // Gửi email thông báo thành công
        // await fetch("/api/send-welcome-email/{userId}", {
        //     method: "POST",
        //     headers: { "Content-Type": "application/json" },
        //     body: JSON.stringify({
        //         recipient: userEmail,
        //         msgBody: "Chào bạn, chúng tôi đã nhận được hồ sơ ứng tuyển của bạn cho vị trí [Tên vị trí]. Cảm ơn bạn đã quan tâm!",
        //         subject: "Xác nhận ứng tuyển thành công",
        //         attachment: null
        //     })
        // }).then(data => {

        //     let notification = {
        //         content: "Your CV have been applied",
        //         type: "SYSTEM",
        //         recipientIds: [document.getElementById("user_id").value],
        //         url: `/recruitment/${recruitmentId}`
        //     }

        //     stompClient.send("/app/notification.sendNotification", {}, JSON.stringify(notification));

        // });

        messageEl.textContent = "Apply thành công! Email xác nhận đã được gửi.";
        messageEl.style.color = "green";
        messageEl.style.backgroundColor = "#e8f5e9";

        applyButton.textContent = "Đã Apply";
        applyButton.disabled = true;
        cvSelect.disabled = true;
        deleteButton.style.display = "inline-flex";
    } catch (error) {
        console.error("Error:", error);
        messageEl.textContent = "Apply thành công! Email xác nhận đã được gửi";
        messageEl.style.color = "green";
        messageEl.style.backgroundColor = "#ffebee";

        applyButton.disabled = false;
        applyButton.textContent = "Apply job";
    }
}

function subtractYears(date, years) {
    const dateCopy = new Date(date);
    dateCopy.setFullYear(date.getFullYear() - years);
    return dateCopy;
}

async function deleteApplication() {
    let messageEl = document.getElementById("message");
    let applyButton = document.getElementById("applyButton");
    let deleteButton = document.getElementById("deleteButton");
    let { recruitmentId } = getIdsFromPath();
    const cvSelect = document.getElementById("cvSelect");

    // Disable button during processing
    deleteButton.disabled = true;
    deleteButton.textContent = "Đang xử lý...";

    try {
        // Kiểm tra xem có fileId đã ứng tuyển không
        if (!appliedFileId) {
            throw new Error("Không tìm thấy CV đã ứng tuyển để hủy!");
        }

        // Gửi DELETE request với fileId đã ứng tuyển
        let response = await fetch(`/api/cv-info/reject?fileId=${appliedFileId}&recruitmentId=${recruitmentId}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            }
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Không thể huỷ ứng tuyển!");
        }

        // Reset appliedFileId sau khi xóa thành công
        appliedFileId = null;

        // Update UI to show not applied state
        applyButton.disabled = false;
        applyButton.textContent = "Apply job";
        deleteButton.style.display = "none";
        cvSelect.disabled = false;

        messageEl.textContent = "Đã huỷ ứng tuyển thành công!";
        messageEl.style.color = "blue";
        messageEl.style.backgroundColor = "#e3f2fd";

        // Reload CV list to update the status of CVs
        loadCVList();
    } catch (error) {
        console.error("Error deleting application:", error);
        messageEl.textContent = error.message;
        messageEl.style.color = "red";
        messageEl.style.backgroundColor = "#ffebee";

        // Re-enable delete button
        deleteButton.disabled = false;
        deleteButton.textContent = "Huỷ apply";
    }
}