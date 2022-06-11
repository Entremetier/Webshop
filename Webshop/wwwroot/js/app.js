const registerBtn = document.getElementById("registerBtn");
const passwordOne = document.getElementById("passwordOne");
const passwordTwo = document.getElementById("passwordTwo");
const passwordMessageOne = document.getElementById("passwordMessageOne");
const passwordMessageTwo = document.getElementById("passwordMessageTwo");

if (registerBtn) {
    registerBtn.addEventListener("click", validatePassword);
}

function validatePassword() {
    let lowerCaseLetters = /[a-z]/g;
    let upperCaseLetters = /[A-Z]/g;
    let numbers = /[0-9]/g;
    let minPasswordLength = 8;

    if (passwordOne.value !== passwordTwo.value)
    {
        passwordMessageOne.textContent = "Passwörter stimmen nicht überein";
        passwordMessageTwo.textContent = "Passwörter stimmen nicht überein";
    }

    if (passwordOne.value.length < minPasswordLength ||
        !passwordOne.value.match(lowerCaseLetters) ||
        !passwordOne.value.match(upperCaseLetters) ||
        !passwordOne.value.match(numbers))
    {
        //passwordMessageOne.textContent = "Passwort muss mindestens 8 Zeichen, einen Groß- und einen Kleinbuchstaben und Sonderzeichen haben";
        alert("Failed " + passwordOne.value);
    }
}