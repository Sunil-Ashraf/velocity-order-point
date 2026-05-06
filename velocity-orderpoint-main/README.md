# OrderPoint

## Setup Instructions

Before running the application, you need to configure the credentials in the `appsettings.json` files.

### Database Configuration
In `OrderPoint.API/appsettings.json`, update the `ConnectionStrings.DefaultConnection` with your database details:
- Replace `YOUR_SQL_SERVER` with your SQL Server address.
- Replace `YOUR_DATABASE` with your database name.
- Replace `YOUR_USER` with your database username.
- Replace `YOUR_DB_PASSWORD` with your database password.

### JWT Configuration
Update the `Jwt.Key` with a secure secret key (at least 32 characters long).

### Email Configuration
Update the `EmailCredential` section:
- `Password`: Replace `YOUR_SMTP_PASSWORD` with your SMTP password (e.g., from Sendinblue/Brevo).

### Running the Application
1. Restore NuGet packages: `dotnet restore`
2. Build the solution: `dotnet build`
3. Run the API: `dotnet run --project OrderPoint.API`
4. For the web app, navigate to the respective project and run similarly.

Ensure you have .NET 10 SDK installed.