JobBoard - ASP.NET Core MVC Minimal Job Board (MVP)
Seeded users:
- admin@jobboard.local / Admin123!
- employer@company.com / Employer123!
- applicant@example.com / Applicant123!

To run:
1. Install .NET 7 SDK (or change TargetFramework to net6.0).
2. From project root:
   dotnet tool install --global dotnet-ef
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
