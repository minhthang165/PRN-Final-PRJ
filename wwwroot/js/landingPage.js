// User dropdown functionality
document.addEventListener("DOMContentLoaded", function () {
    // Carousel functionality
    const carouselItems = document.querySelectorAll('.carousel-item');
    const indicators = document.querySelectorAll('.carousel-indicator');
    const prevBtn = document.querySelector('.carousel-control-prev');
    const nextBtn = document.querySelector('.carousel-control-next');
    let currentSlide = 0;
    const totalSlides = carouselItems.length;

    function showSlide(index) {
        // Hide all slides
        carouselItems.forEach(item => {
            item.classList.remove('active');
        });

        // Deactivate all indicators
        indicators.forEach(indicator => {
            indicator.classList.remove('active');
        });

        // Show the selected slide and activate indicator
        carouselItems[index].classList.add('active');
        indicators[index].classList.add('active');

        // Update current slide index
        currentSlide = index;
    }

    // Add click event to indicators
    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', () => {
            showSlide(index);
        });
    });

    // Previous button click
    prevBtn.addEventListener('click', () => {
        let newIndex = currentSlide - 1;
        if (newIndex < 0) newIndex = totalSlides - 1;
        showSlide(newIndex);
    });

    // Next button click
    nextBtn.addEventListener('click', () => {
        let newIndex = currentSlide + 1;
        if (newIndex >= totalSlides) newIndex = 0;
        showSlide(newIndex);
    });

    // Auto slide every 5 seconds
    setInterval(() => {
        let newIndex = currentSlide + 1;
        if (newIndex >= totalSlides) newIndex = 0;
        showSlide(newIndex);
    }, 5000);

    // Video modal functionality
    const videoBtn = document.getElementById('videoBtn');
    const videoModal = document.getElementById('videoModal');
    const closeModal = document.getElementById('closeModal');
    const videoIframe = document.querySelector('.video-wrapper iframe');

    if (videoBtn && videoModal && closeModal) {
        videoBtn.addEventListener('click', function(e) {
            e.preventDefault();
            videoIframe.src = "https://www.youtube.com/embed/MlLRo-GpHO4";
            videoModal.classList.add('show');
            document.body.style.overflow = 'hidden';
        });

        closeModal.addEventListener('click', function() {
            videoModal.classList.remove('show');
            videoIframe.src = "";
            document.body.style.overflow = '';
        });

        // Close modal when clicking outside the video container
        videoModal.addEventListener('click', function(e) {
            if (e.target === videoModal) {
                videoModal.classList.remove('show');
                videoIframe.src = "";
                document.body.style.overflow = '';
            }
        });
    }
});