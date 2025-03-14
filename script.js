document.addEventListener('DOMContentLoaded', () => {
    const loginWrapper = document.querySelector('.login-wrapper');
    const clockElement = document.getElementById('clock');
    const loginForm = document.getElementById('loginForm');
    const activationButton = document.getElementById('activationButton');
    const scrollContainer = document.querySelector('.scroll-container');
    const loginDetails = [];

    const classForm = document.getElementById('classForm');
    const tableBody = document.querySelector('#classTable tbody');

    function updateClock() {
        const now = new Date();
        if (clockElement) {
            clockElement.textContent = now.toLocaleTimeString();
        }
    }
    setInterval(updateClock, 1000);
    updateClock();

    if (loginForm) {
        loginForm.addEventListener('submit', (event) => {
            event.preventDefault();
            const username = document.getElementById('username').value.trim();
            const password = document.getElementById('password').value.trim();

            console.log("Attempting login with:", username, password);

            if (username === 'admin' && password === 'admin') {
                console.log("Login successful! Redirecting to table.html...");
                window.location.href = 'table.html';
            } else {
                alert('Invalid username or password. Please try again.');
            }
        });
    }

    if (activationButton) {
        activationButton.addEventListener('click', (event) => {
            event.preventDefault();
            if (scrollContainer) scrollContainer.style.display = 'none';
            if (loginWrapper) loginWrapper.style.display = 'flex';
            console.log('Activation button clicked. Login page shown.');
        });
    }

    document.addEventListener('keydown', (event) => {
        if (event.key.toLowerCase() === 'h') {
            if (loginWrapper) {
                loginWrapper.style.display = (loginWrapper.style.display === 'none') ? 'flex' : 'none';
            }
        }
    });

    if (classForm) {
        classForm.addEventListener('submit', (event) => {
            event.preventDefault();

            const className = document.getElementById('className').value.trim();
            const numPeople = document.getElementById('numPeople').value.trim();
            const description = document.getElementById('description').value.trim();

            console.log("Adding class:", className, numPeople, description);

            if (!className || !numPeople || !description) {
                alert('Please fill all fields!');
                return;
            }

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${className}</td>
                <td>${numPeople}</td>
                <td>${description}</td>
            `;

            // Append row to table and show it on the screen
            if (tableBody) {
                tableBody.appendChild(row);
                console.log("Class added to table: ", { className, numPeople, description });

                // Pop up the content (alert message)
                alert(`Class added: \nClass Name: ${className}\nNumber of People: ${numPeople}\nDescription: ${description}`);

            } else {
                console.error("Error: Table body not found.");
            }

            // Log to console
            console.log("Class added to table.");

            // Reset form fields after submission
            classForm.reset();
        });
    } else {
        console.error("Error: classForm not found.");
    }

    document.querySelectorAll('input').forEach(input => {
        // Input Focus Event (Highlight the field when active)
        input.addEventListener('focus', () => {
            input.style.border = '2px solid #4CAF50'; // Green border when active
            input.style.boxShadow = '0 0 8px rgba(76, 175, 80, 0.5)';
        });

        // Input Blur Event (Validate input & reset styling)
        input.addEventListener('blur', () => {
            if (input.value.trim() === '') {
                input.style.border = '2px solid red'; // Show red border if empty
                input.style.boxShadow = 'none';
            } else {
                input.style.border = '1px solid #ccc'; // Reset if filled
                input.style.boxShadow = 'none';
            }
        });
    });

    // Table Click Event: When the table is clicked (outside individual rows), print all class entries
    const classTable = document.getElementById('classTable');
    classTable.addEventListener('click', (event) => {
        // Check if the click is outside any individual row (tr)
        if (event.target.tagName !== 'TD') {
            const rows = classTable.querySelectorAll('tr');
            let classEntries = [];

            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                if (cells.length > 0) { // Skip the header row
                    classEntries.push({
                        className: cells[0].textContent,
                        numPeople: cells[1].textContent,
                        description: cells[2].textContent,
                    });
                }
            });

            console.log(classEntries); // Print all class entries to console
        }
    });
});
