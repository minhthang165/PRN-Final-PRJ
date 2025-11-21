let newAvatarUrl = null; // Store uploaded image URL

async function updateAvatar(event) {
    const file = event.target.files[0];
    if (!file) return;

    let formData = new FormData();
    formData.append('file', file);

    try {
        const response = await fetch(`https://localhost:7063/Cloudinary/upload/uploadFile`, {
            method: 'POST',
            cache: 'no-cache',
            body: formData,
        });

        if (!response.ok) throw new Error("File upload failed");

        const data = await response.json();
        newAvatarUrl = data.url; // Store new avatar URL

        await updateUser();

        // Update the displayed profile picture
        document.getElementById("picture").src = newAvatarUrl;

    } catch (error) {
        console.error("Error uploading file:", error);
    }
}

async function updateUser() {
    const finalAvatarUrl = newAvatarUrl ? newAvatarUrl : document.getElementById("picture").src; // Use new or existing avatar
    const userData = {
        first_name: document.getElementById('first_name').value,
        last_name: document.getElementById('last_name').value,
        gender: document.getElementById('gender').value.toUpperCase(),
        email: document.getElementById('email').value,
        phone_number: document.getElementById('phone_number').value,
        avatar_path: finalAvatarUrl, // Set avatar
    };

    console.log(userData);

    try {
        const response = await fetch('https://localhost:7063/api/user/update/', {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });

        if (!response.ok) throw new Error('Network response was not ok');

        const result = await response.json();
        console.log('User updated successfully:', result);  

        // Redirect to user profile after update
        window.location.href = "/profile"
    } catch (error) {
        console.error('Error updating user:', error);
    }
}

document.querySelector('form').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent the default form submission
    updateUser();
});
