const githubStatus = document.getElementById("github-status");
const githubResultaat = document.getElementById("github-resultaat");

function toonGithub(data) {
    githubResultaat.textContent = "";

    const aantal = document.createElement("p");
    aantal.textContent = "Openbare repositories: " + data.public_repos;
    githubResultaat.appendChild(aantal);
}

async function laadGithub() {
    githubStatus.textContent = "GitHub-gegevens laden...";
    githubResultaat.textContent = "";

    try {
        const response = await fetch(
            "https://api.github.com/users/saksagan86"
        );

        if (!response.ok) {
            throw new Error("GitHub ophalen mislukt.");
        }

        const data = await response.json();

        toonGithub(data);
        githubStatus.textContent = "";
    } catch (error) {
        githubStatus.textContent =
            "GitHub-gegevens konden niet worden geladen. Probeer het later opnieuw.";
    }
}

laadGithub();