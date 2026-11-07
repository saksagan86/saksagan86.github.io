# Portfolio saksagan86

Dit is mijn portfoliosite voor het vak WPFW. In de hoofdmap staat de frontend (HTML, CSS en JavaScript). In de map `backend/PortfolioApi` staat de backend die ik voor Opdracht 3 heb gemaakt.

## Backend

De backend is een ASP.NET Core Web API in .NET 10. Ik heb gekozen voor controllers. Voor de database gebruik ik Entity Framework Core 10 met SQL Server Express. Met Swagger UI kun je de API testen.

### Opbouw van de code

De code is verdeeld in lagen:

- `Controllers`: ontvangen de HTTP-requests en geven een antwoord terug (ProjectsController en BlogpostsController).
- `Services`: hier zit de logica en de omzetting van entiteit naar DTO (ProjectService en BlogpostService).
- `Data`: de PortfolioDbContext, die met de database praat. In `Data/Entities` staan de entiteiten Project en Blogpost.
- `Dtos`: de DTO's die de API ontvangt en terugstuurt.
- `Migrations`: de EF Core-migratie waarmee de database wordt aangemaakt.
- `Program.cs`: hier worden de DbContext en de services geregistreerd.

Een request gaat van de controller naar de service, van de service naar de DbContext en dan naar SQL Server. Het resultaat komt als DTO in JSON terug.

### Wat je nodig hebt

- .NET 10 SDK
- SQL Server Express
- De tool dotnet-ef. Installeren: `dotnet tool install --global dotnet-ef --version 10.0.14`

### SQL Server instellen

De connectionstring staat in `appsettings.json`. Er wordt Windows-authenticatie gebruikt, dus er staat geen wachtwoord in de code. Mijn SQL Server heet `localhost\SQLEXPRESS`. Heeft jouw server een andere naam, pas dan het stuk na `Server=` aan.

### Database aanmaken

Ga naar de map `backend/PortfolioApi` en voer uit: `dotnet ef database update`

Dit maakt de database PortfolioDb aan met de tabellen Projects en Blogposts.

### Starten

Ga naar de map `backend/PortfolioApi` en voer uit: `dotnet run`

Open daarna http://localhost:5274/swagger in de browser.

### Endpoints

Voor projecten en blogposts zijn er elk vijf endpoints:

- `GET /api/projects` en `GET /api/blogposts`: alles opvragen
- `GET /api/projects/{id}` en `GET /api/blogposts/{id}`: één item opvragen (404 als het id niet bestaat)
- `POST /api/projects` en `POST /api/blogposts`: nieuw item aanmaken
- `PUT /api/projects/{id}` en `PUT /api/blogposts/{id}`: item wijzigen
- `DELETE /api/projects/{id}` en `DELETE /api/blogposts/{id}`: item verwijderen