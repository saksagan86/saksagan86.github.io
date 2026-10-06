const projecten = [
    {
        id:1,
        titel:"All Fit, Fitness Applicatie",
        beschrijving:"Een fitness applicatie die ik heb gemaakt met HTML, CSS, JavaScript. Hier kun je je eigen trainingsschema's maken en bijhouden.",
        github_link:"https://github.com/saksagan86/AllFit-Groep2"

    },
    {
        id:2,
        titel:"Trouw Website",
        beschrijving:"Een trouw website voor mijn vrienden Meyra en Osman, gemaakt met HTML, CSS en JavaScript. Hier kun je aanmelden voor de bruiloft en informatie vinden.",
        link:"https://meyraosman.com",
        github_link:"https://github.com/saksagan86/meyraosman",

    },
    {
        id:3,
        titel:"Persoonlijke Website",
        beschrijving:"Mijn persoonlijke website, gemaakt met HTML, CSS en JavaScript. Hier kun je meer over mij te weten komen en mijn projecten bekijken.",
        link:"https://saksagan86.github.io",
        github_link:"https://github.com/saksagan86/saksagan86.github.io",
    },
    {
        id:4,
        titel:"Ubuntu Home Server",
        beschrijving:"Een Ubuntu home server die ik heb opgezet om mijn websites te hosten en linux te leren.",
        link:"",
        github_link:""
    }
]

const projectenLijst = document.getElementById("projecten-lijst");

function toonProjecten(lijst) {
    projectenLijst.textContent = "";

    lijst.forEach((project) => {
        const article = document.createElement("article");

        const titel = document.createElement("h3");
        titel.textContent = project.titel;
        article.appendChild(titel);

        const beschrijving = document.createElement("p");
        beschrijving.textContent = project.beschrijving;
        article.appendChild(beschrijving);

        const url = project.link || project.github_link;

        if (url) {
            const link = document.createElement("a");
            link.href = url;
            link.textContent = "Bekijk project";
            article.appendChild(link);
        }

        projectenLijst.appendChild(article);
    });
}

toonProjecten(projecten);