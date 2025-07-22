let newAvatarUrl = null; // Store uploaded image URL

async function updateAvatar(event) {
    const file = event.target.files[0];
    if (!file) return;

    let formData = new FormData();
    formData.append('file', file);

    try {
        const response = await fetch(`cloudinary/upload`, {
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
