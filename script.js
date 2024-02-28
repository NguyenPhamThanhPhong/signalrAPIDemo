document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Prevent default form submission

    // Get login code from input field
    var loginCode = document.getElementById("loginCode").value.trim();

    // Make API call to login endpoint
    var baseUrl = 'https://localhost:7161'
    var data = fetch(`${baseUrl}/user/login/${loginCode}`, {
        method: 'GET',
        credentials: 'include' // Send cookies along with the request
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        window.alert(response.json);
        return response.json(); // Parse response body as JSON
    })
    .then(data => {
        // If login successful, display user data
        console.log('Login successful:', data);
        // Redirect to dashboard or perform any other action
    })
    .catch(error => {
        console.error('There was a problem with the login:', error.message);
        // Display error message to the user
    });
    
});

