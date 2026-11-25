namespace PortfolioApi.Auth;

// De namen van de rollen staan op één plek, zodat er geen typefouten in strings ontstaan.
public static class Rollen
{
    public const string Gast = "Gast";
    public const string Editor = "Editor";
    public const string Admin = "Admin";

    public static readonly string[] Alle = { Gast, Editor, Admin };
}

public static class Policies
{
    // Mag projecten en blogposts aanmaken, wijzigen en verwijderen.
    public const string KanBewerken = "KanBewerken";
}
