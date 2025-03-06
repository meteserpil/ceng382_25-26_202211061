    document.addEventListener('DOMContentLoaded', () => {
    const loginWrapper = document.querySelector('.login-wrapper');
    const clockElement = document.getElementById('clock');
    const loginForm = document.getElementById('loginForm');
    const activationButton = document.getElementById('activationButton');
    const scrollContainer = document.querySelector('.scroll-container');
    const loginDetails = [];

    // Function to update the clock
    function updateClock() {
        const now = new Date();
        clockElement.textContent = now.toLocaleTimeString();
    }

    // Update the clock every second
    setInterval(updateClock, 1000);
    updateClock();

    // Handle form submission
    loginForm.addEventListener('submit', (event) => {
        event.preventDefault();
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        loginDetails.push({ username, password });
        console.log('Login Details:', loginDetails);
    });

    // Hide activation button and show login wrapper when activation button is clicked
    activationButton.addEventListener('click', (event) => {
        event.preventDefault(); // Prevent default link behavior
        scrollContainer.style.display = 'none'; // Hide the activation button
        loginWrapper.style.display = 'flex'; // Show the login wrapper
        console.log('Activation button clicked. Login page shown.');
    });

    // 'h'
    document.addEventListener('keydown', (event) => {
        if (event.key.toLowerCase() === 'h') {
            if (loginWrapper.style.display === 'none') {
                loginWrapper.style.display = 'flex'; //show login page
            } else {
                loginWrapper.style.display = 'none'; //hide login page
            }
        }
    });
});