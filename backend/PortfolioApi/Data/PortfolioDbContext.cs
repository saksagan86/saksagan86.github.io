using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data.Entities;

namespace PortfolioApi.Data;

// IdentityDbContext<User> voegt de Identity-tabellen toe (AspNetUsers, AspNetRoles, AspNetUserRoles, ...).
public class PortfolioDbContext : IdentityDbContext<User>
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}
