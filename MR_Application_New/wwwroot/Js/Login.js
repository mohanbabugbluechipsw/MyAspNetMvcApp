document.addEventListener("DOMContentLoaded", function () {
    // Clear input fields on page load
    document.getElementById('Email').value = '';
    document.getElementById('Password').value = '';

    // Show SweetAlert if login fails
    var errorMessage = document.getElementById("ErrorMessage")?.value || "";
    if (errorMessage.trim() !== "") {
        Swal.fire({
            icon: 'error',
            title: 'Login Failed',
            text: errorMessage,
            confirmButtonColor: '#007bff'
        });
    }

    // Client-Side Validation
    document.getElementById("loginForm").addEventListener("submit", function (event) {
        let isValid = true;

        // Clear previous error messages
        document.getElementById("emailError").innerText = "";
        document.getElementById("passwordError").innerText = "";

        // Validate Email
        const email = document.getElementById("Email").value;
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailPattern.test(email)) {
            document.getElementById("emailError").innerText = "Please enter a valid email address.";
            isValid = false;
        }

        // Validate Password
        const password = document.getElementById("Password").value;
        if (password.length < 6) {
            document.getElementById("passwordError").innerText = "Password must be at least 6 characters long.";
            isValid = false;
        }

        // Show alert if validation fails
        if (!isValid) {
            event.preventDefault();
            Swal.fire({
                icon: 'warning',
                title: 'Validation Error',
                text: 'Please fix the errors before submitting.',
                confirmButtonColor: '#007bff'
            });
        }
    });
});
