# Dating Course API

This is a Dating Course API built with ASP.NET Core 8 Web API that integrates with Cloudinary for image management and includes real-time communication features using SignalR.

This project is based on the Udemy course [Build an app with ASPNET Core and Angular from scratch](https://www.udemy.com/course/build-an-app-with-aspnet-core-and-angular-from-scratch) by Neil Cummings.

## Technologies & Features

- **Framework**: ASP.NET Core 8.0
- **Database**: 
  - Microsoft SQL Server
  - Entity Framework Core 8.0
  - Identity Framework Core 8.0
- **Authentication & Authorization**:
  - JWT (JSON Web Tokens)
  - Role-based authorization
- **Real-time Communication**:
  - SignalR for chat and presence tracking
  - Connection management
- **Image Storage**: 
  - Cloudinary integration
  - Photo moderation capabilities
- **Other Tools & Libraries**:
  - AutoMapper for object mapping
  - Bogus for seeding test data

## Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/downloads)
- [Postman](https://www.postman.com/downloads/) (for testing)
- [Visual Studio Code](https://code.visualstudio.com/) or preferred IDE

## Getting Started

Follow these steps to get the project up and running on your local machine:

### 1. Clone the Repository

```bash
# Clone the project
git clone https://github.com/JorgeRiveraMancilla/dating-course-api.git

# Navigate to the project directory
cd dating-course-api
```

### 2. Configure Application Settings

Update the `appsettings.Development.json` file with your Cloudinary credentials. You'll need to:
1. Create a Cloudinary account at [cloudinary.com](https://cloudinary.com)
2. Get your credentials from your Cloudinary Dashboard
3. Replace the CloudinarySettings section with your actual credentials

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=dating-course-db;User Id=SA;Password=Password@1;TrustServerCertificate=True"
  },
  "JWTSettings": {
    "TokenKey": "rtIwMbKWHnZXLPkCZuSZGc6fBs7x9vt2weEjKkvyWzGbkWS5yMEtuqfPACtjK9s9"
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "AdminUser": {
    "UserName": "Jorge",
    "Email": "jiriveramancilla@gmail.com",
    "KnownAs": "Joignaci3",
    "Gender": "Masculino",
    "City": "Antofagasta",
    "Country": "Chile",
    "BirthDate": "2000-10-25",
    "Password": "Password12345"
  }
}
```

### 3. Start the Database

Launch the SQL Server container using Docker Compose:

```bash
docker compose up -d
```

### 4. Run the Application

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `http://localhost:5001`

## Testing the API

A Postman collection file (`Dating Course.postman_collection.json`) is provided in the repository for testing all available endpoints.

## Frontend Requirements

To use this API with the frontend application, you will need to set up the Dating Course Web Client. You can find the frontend repository here: [Dating Course Web Client](https://github.com/JorgeRiveraMancilla/dating-course-web-client)