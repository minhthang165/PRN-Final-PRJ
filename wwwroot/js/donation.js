// Global variables
let selectedAmount = 100000;
let isProcessing = false;

// DOM elements
const amountButtons = document.querySelectorAll('.amount-btn');
const customAmountInput = document.getElementById('customAmount');
const totalAmountDisplay = document.getElementById('totalAmount');
const donateBtn = document.getElementById('donateBtn');
const btnText = document.getElementById('btnText');
const btnLoader = document.getElementById('btnLoader');
const donationForm = document.getElementById('donationForm');
const successModal = document.getElementById('successModal');

// Initialize the page
document.addEventListener('DOMContentLoaded', function () {
    setupEventListeners();
    updateDisplay();
});

// Set up all event listeners
function setupEventListeners() {
    // Amount button clicks
    amountButtons.forEach(button => {
        button.addEventListener('click', function () {
            const amount = parseInt(this.dataset.amount);
            selectAmount(amount);
        });
    });

    // Custom amount input
    customAmountInput.addEventListener('input', function () {
        const customValue = parseFloat(this.value) || 0;
        if (customValue > 0) {
            selectedAmount = customValue;
            clearAmountButtons();
            updateDisplay();
        }
    });

    // Form submission
    donationForm.addEventListener('submit', function (e) {
        e.preventDefault();
        processDonation();
    });

    // Modal close on background click
    successModal.addEventListener('click', function (e) {
        if (e.target === successModal) {
            closeModal();
        }
    });
}

// Select a preset amount
function selectAmount(amount) {
    selectedAmount = amount;
    customAmountInput.value = '';

    // Update button states
    amountButtons.forEach(btn => {
        btn.classList.remove('active');
        if (parseInt(btn.dataset.amount) === amount) {
            btn.classList.add('active');
        }
    });

    updateDisplay();
}

// Clear all amount button selections
function clearAmountButtons() {
    amountButtons.forEach(btn => {
        btn.classList.remove('active');
    });
}

// Update the display with current amount
function updateDisplay() {
    const formattedAmount = selectedAmount.toFixed(0);
    totalAmountDisplay.textContent = `${formattedAmount}`;
    btnText.textContent = `Donate ${formattedAmount}`;

    // Enable/disable donate button
    donateBtn.disabled = selectedAmount <= 0 || isProcessing;
}

// Process the donation
async function processDonation() {
    if (isProcessing || selectedAmount <= 0) return;

    // Validate form
    const firstName = document.getElementById('firstName').value.trim();
    const lastName = document.getElementById('lastName').value.trim();
    const email = document.getElementById('email').value.trim();
    const message = document.getElementById('message')?.value.trim() || ''; // Get message if it exists

    if (!firstName || !lastName || !email) {
        alert('Please fill in all required fields.');
        return;
    }

    if (!isValidEmail(email)) {
        alert('Please enter a valid email address.');
        return;
    }

    // Start processing
    isProcessing = true;
    updateDisplay();
    showLoader();

    try {
        const donationDescription = `Donation from ${firstName} ${lastName} (${email})${message ? ': ' + message : ''}`;
        const response = await fetch(`/CreatePaymentUrl?moneyToPay=${selectedAmount}&description=${encodeURIComponent(donationDescription)}`);
        if (!response.ok) {
            throw new Error(`Payment request failed: ${response.statusText}`);
        }
        const data = await response.text();
        window.location.href = data;
        resetForm();

    } catch (error) {
        alert('There was an error processing your donation. Please try again.');
        console.error('Donation error:', error);
    } finally {
        isProcessing = false;
        hideLoader();
        updateDisplay();
    }
}

// Simulate payment processing delay
function simulatePaymentProcessing() {
    return new Promise((resolve) => {
        setTimeout(resolve, 2000); // 2 second delay
    });
}

// Show loading state
function showLoader() {
    btnText.style.display = 'none';
    btnLoader.style.display = 'flex';
}

// Hide loading state
function hideLoader() {
    btnText.style.display = 'block';
    btnLoader.style.display = 'none';
}

// Show success modal
function showSuccessModal(paymentResult = null, paymentAmmount = null) {
    const confirmedAmount = document.getElementById('confirmedAmount');
    const transactionId = document.getElementById('transactionId');
    const transactionDate = document.getElementById('transactionDate');
    if (paymentResult) {
        // Use the real transaction data from VNPay
        console.log(paymentResult);
        confirmedAmount.textContent = paymentAmmount;
        transactionId.textContent = paymentResult.PaymentId;
        transactionDate.textContent = new Date().toLocaleDateString();
    } else {
        // Fallback to default values if no payment result provided
        confirmedAmount.textContent = `${selectedAmount.toFixed(0)}`;
        transactionId.textContent = `TXN-${generateTransactionId()}`;
        transactionDate.textContent = new Date().toLocaleDateString();
    }

    successModal.classList.add('show');
    document.body.style.overflow = 'hidden';
}

// Close success modal
function closeModal() {
    successModal.classList.remove('show');
    document.body.style.overflow = 'auto';
}

// Reset the form
function resetForm() {
    donationForm.reset();
    selectedAmount = 100000;
    selectAmount(100000);
}

// Generate a random transaction ID
function generateTransactionId() {
    return Math.random().toString(36).substr(2, 9).toUpperCase();
}

// Validate email format
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Download receipt (placeholder function)
function downloadReceipt() {
    // In a real application, this would generate and download a PDF receipt
    alert('Receipt download would be implemented here. For now, a receipt will be sent to your email.');
    closeModal();
}

// Handle keyboard navigation
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && successModal.classList.contains('show')) {
        closeModal();
    }
});

// Add some interactive feedback
amountButtons.forEach(button => {
    button.addEventListener('mouseenter', function () {
        if (!this.classList.contains('active')) {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';
        }
    });

    button.addEventListener('mouseleave', function () {
        this.style.transform = 'translateY(0)';
        this.style.boxShadow = 'none';
    });
});

// Add smooth scrolling for any anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth'
            });
        }
    });
});