const form = document.getElementById("contact-form");
const result = document.getElementById("form-result");

function showConfirmation(event) {
    event.preventDefault();
    result.textContent = "Formulier correct ingevuld.";
    form.reset();
}

function clearConfirmation() {
    result.textContent = "";
}

form.addEventListener("submit", showConfirmation);
form.addEventListener("input", clearConfirmation);