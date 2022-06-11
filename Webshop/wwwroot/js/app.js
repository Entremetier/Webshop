const registerBtn = document.getElementById("registerBtn");
const passwordOne = document.getElementById("passwordOne");
const passwordTwo = document.getElementById("passwordTwo");
const passwordMessageOne = document.getElementById("passwordMessageOne");
const passwordMessageTwo = document.getElementById("passwordMessageTwo");

registerBtn.addEventListener("click", validatePassword);

function validatePassword() {
    if (passwordOne.value !== passwordTwo.value)
    {
        passwordMessageOne.textContent = "Passwörter stimmen nicht überein";
        passwordMessageTwo.textContent = "Passwörter stimmen nicht überein";
    }
}