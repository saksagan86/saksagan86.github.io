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

Dit maakt de database PortfolioDb aan met de tabellen Projects en Blogposts, en (sinds Opdracht 4) de Identity-tabellen voor gebruikers en rollen (AspNetUsers, AspNetRoles, AspNetUserRoles en andere).

De migraties worden ook automatisch toegepast als de applicatie start. Bij het starten worden ook de rollen Gast, Editor en Admin aangemaakt als ze nog niet bestaan (`Data/RolSeeder.cs`). Er worden daarbij geen gebruikers of wachtwoorden aangemaakt.

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

Iedereen mag de GET-routes gebruiken. POST, PUT en DELETE mogen alleen door een ingelogde gebruiker met de rol Editor of Admin. Zonder login krijg je 401, met een andere rol (bijvoorbeeld Gast) krijg je 403.

## Opdracht 4: inloggen en rollen

Voor gebruikers gebruik ik ASP.NET Core Identity met EF Core. De gebruikers en rollen staan in dezelfde SQL Server-database.

- `POST /api/auth/register`: nieuwe gebruiker maken. Iedere nieuwe gebruiker krijgt de rol Gast. Via deze route kun je nooit Editor of Admin worden.
- `POST /api/auth/login`: inloggen. Als het lukt, stuurt de API een authenticatiecookie terug. Swagger en de browser sturen die cookie daarna automatisch mee.
- `POST /api/auth/logout`: uitloggen.
- `GET /api/auth/me`: laat zien wie er is ingelogd en welke rollen die gebruiker heeft.

### Een gewone gebruiker, Editor en Admin maken (lokaal)

1. Start de API en registreer drie gebruikers via `POST /api/auth/register` in Swagger. Ze krijgen allemaal de rol Gast. Kies zelf een wachtwoord; dat komt nergens in de repository.
2. Open `backend/scripts/rol-toekennen.sql` in SSMS. Vul bij `@Email` het e-mailadres in en bij `@Rol` de rol (`Editor` of `Admin`) en voer het script uit. Doe dit een keer voor de Editor en een keer voor de Admin.
3. Log daarna opnieuw in met die gebruiker. De rollen staan in de cookie, dus na een rolwijziging moet je altijd opnieuw inloggen.

## Tests

In de map `backend/PortfolioApi.Tests` staat een xUnit-testproject. Het zit samen met de API in de solution `backend/PortfolioApi.slnx`.

De tests starten de echte API in het geheugen en sturen HTTP-requests (met `WebApplicationFactory`). Ze gebruiken een aparte database `PortfolioDb_Test` (zie `appsettings.Testing.json`), zodat de gewone database niet wordt aangeraakt. Die testdatabase wordt bij elke testrun opnieuw aangemaakt.

Tests uitvoeren: ga naar de map `backend` en voer uit: `dotnet test`