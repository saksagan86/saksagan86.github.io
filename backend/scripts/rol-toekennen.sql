
USE PortfolioDb;

DECLARE @Email nvarchar(256) = N'editor@voorbeeld.nl';
DECLARE @Rol   nvarchar(256) = N'Editor';   -- of N'Admin'

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE u.NormalizedEmail = UPPER(@Email)
  AND r.NormalizedName = UPPER(@Rol)
  AND NOT EXISTS (
      SELECT 1 FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id AND ur.RoleId = r.Id
  );

-- Controle: welke rollen heeft deze gebruiker nu?
SELECT u.Email, r.Name AS Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON ur.UserId = u.Id
JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE u.NormalizedEmail = UPPER(@Email);
