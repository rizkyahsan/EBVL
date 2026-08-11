# 🚀 Solution Template 2 - Developer Guide

---

## 1. 📖 Introduction

Welcome to Solution Template 2! This template is designed for building robust, scalable, and maintainable web applications using .NET 9. It is built upon a custom **Pertamina framework** and follows a **Clean Architecture** approach, ensuring a clear separation of concerns. This design makes the application easier to test, maintain, and evolve over time.

### ✨ Core Principles

*   **Clean Architecture**: The dependencies in this solution flow inwards. The core business logic (Domain) is independent of external concerns like databases, UI frameworks, or third-party libraries.
*   **Mediator Pattern**: The template uses the MediatR library to enable developers to write Request and Request Handler separately. This allows for more optimized and maintainable code.
*   **Modular Design**: The application is organized into modules (e.g., `MasterData`, `Administration`), each representing a specific business capability.
*   **Pertamina Packages**: The solution is built on a set of custom `Pertamina.*` NuGet packages, which provide a foundation for dependency injection, configuration, and common services.

### 🛠️ Core Technologies

*   **Framework & Language**: .NET 9 and C# 13
*   **BackEnd**: ASP.NET Core Minimal API
*   **BackEnd Authentication**: JWT Bearer
*   **Database**: SQL Server
*   **ORM**: Entity Framework Core (with data encryption capability)
*   **API Documentation**: Scalar
*   **Background Jobs**: Hangfire
*   **Health Monitoring**: AspNetCore.HealthChecks
*   **FrontEnd**: ASP.NET Core Blazor Web App with Interactive Server render mode
*   **FrontEnd Authentication**: OpenID Connect (with IdAMan integration)
*   **User Interface Components**: MudBlazor and Blazor-ApexCharts
*   **Authorization**: Role-based and permission-based authorization
*   **Logging**: Serilog
*   **Application Performance Monitoring**: Azure Application Insights

---

## 2. 🏗️ Solution Structure

The solution is organized into three main parts: `Shared`, `BackEnd`, and `FrontEnd`.

### `01.Shared`

*   #### `01.Shared.Enums`
    *   **🎯 Purpose**: Contains enum types that are shared across the entire solution.
    *   **📦 Key Packages**: `Pertamina.Common.Attributes`.

*   #### `01.Shared.Statics`
    *   **🎯 Purpose**: Contains static classes and constants.

*   #### `01.Shared.Dto`
    *   **🎯 Purpose**: Contains Data Transfer Objects (DTOs) used for communication between the front-end and back-end.
    *   **📦 Key Packages**: `Pertamina.Common.Dto`.
    *   **🔗 Dependencies**: `01.Shared.Enums`, `01.Shared.Statics`.

### `02.BackEnd`

*   #### `01.BackEnd.Domain`
    *   **🎯 Purpose**: The heart of the back-end, containing the core domain entities.
    *   **📦 Key Packages**: `Pertamina.Common.Domain`.
    *   **🔗 Dependencies**: `01.Shared.Enums`.

*   #### `02.BackEnd.Services`
    *   **🎯 Purpose**: Defines the contracts (interfaces) for the back-end services.
    *   **📦 Key Packages**: `Microsoft.EntityFrameworkCore`.
    *   **🔗 Dependencies**: `01.BackEnd.Domain`.

*   #### `03.BackEnd.Infrastructure`
    *   **🎯 Purpose**: Implements the interfaces defined in `02.BackEnd.Services` and handles all external concerns.
    *   **📦 Key Packages**: `Pertamina.Extensions.*`, `Pertamina.Services.*`, `AspNetCore.HealthChecks.*`, `Hangfire.*`, `Serilog.*`.
    *   **🔗 Dependencies**: `01.Shared.Statics`, `02.BackEnd.Services`.

*   #### `04.BackEnd.Logics`
    *   **🎯 Purpose**: Contains the business logic of the back-end, including CQRS handlers.
    *   **📦 Key Packages**: `Pertamina.Extensions.MediatR`, `FluentValidation`.
    *   **🔗 Dependencies**: `01.Shared.Statics`, `01.Shared.Dto`, `02.BackEnd.Services`.

*   #### `05.BackEnd.WebApi`
    *   **🎯 Purpose**: The presentation layer of the back-end, exposing the application's functionality as a RESTful API.
    *   **📦 Key Packages**: `Microsoft.EntityFrameworkCore.Design`, `Scalar.AspNetCore`.
    *   **🔗 Dependencies**: `03.BackEnd.Infrastructure`, `04.BackEnd.Logics`.

### `03.FrontEnd`

*   #### `02.FrontEnd.Services`
    *   **🎯 Purpose**: Defines the contracts (interfaces) for the front-end services.
    *   **📦 Key Packages**: `RestSharp`.

*   #### `03.FrontEnd.Infrastructure`
    *   **🎯 Purpose**: Implements the front-end services and handles front-end specific infrastructure concerns.
    *   **📦 Key Packages**: `Pertamina.Services.*`, `Microsoft.AspNetCore.Authentication.OpenIdConnect`.
    *   **🔗 Dependencies**: `01.Shared.Dto`, `02.FrontEnd.Services`.

*   #### `04.FrontEnd.Logics`
    *   **🎯 Purpose**: Contains the front-end business logic.
    *   **📦 Key Packages**: `Pertamina.Extensions.MediatR`, `FluentValidation`.
    *   **🔗 Dependencies**: `01.Shared.Dto`, `02.FrontEnd.Services`.

*   #### `05.FrontEnd.WebUi`
    *   **🎯 Purpose**: The Blazor user interface project.
    *   **📦 Key Packages**: `Pertamina.Common.Components`, `Blazor-ApexCharts`.
    *   **🔗 Dependencies**: `03.FrontEnd.Infrastructure`, `04.FrontEnd.Logics`.

---

## 3. 🚀 Development Workflow

### 📦 Getting the Solution Template

To use this solution template, you first need to install the credential provider and the template itself.

1. **Create Personal Access Token (PAT)**
   - Generate a Personal Access Token (PAT) with the necessary permissions to access the Azure Artifacts feed.
   - Give checkmark to the scope Read in Packaging section. 
   - You can create a PAT in your TFS account under Your Avatar > Security > Personal Access Tokens.
2. **Add pertamina package feed as a nuget package source**
   ```cmd
   dotnet nuget add source "http://tfs.pertamina.com:8080/tfs/Enterprise Management/_packaging/pertamina/nuget/v3/index.json" --name pertamina-tfs --allow-insecure-connections 
   ```
3. **Update the authentication**
   ```cmd
   dotnet nuget update source pertamina-tfs --valid-authentication-types basic --username "pertamina" --password [Your PAT] 
   ```
4. **Install the Solution Template**:
    ```cmd
    dotnet new install Pertamina.Templates.EBVL --force
    ```
5. **Create a new folder**:
    ```cmd
    md YourAppName
    ```
6.  **Navigate to your folder**:
    ```cmd
    cd YourAppName
    ```
7.  **Create a new project**:
    Navigate to your desired folder and run the following command:
    ```cmd
    dotnet new ptmnsln2 --force
    ```

### ✅ Prerequisites

Before you begin, ensure you have the following installed:

*   **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**
*   **A code editor**:
    *   [Visual Studio 2026 version 18.3.0](https://visualstudio.microsoft.com/)
    *   [VS Code](https://code.visualstudio.com/)

### ⚙️ Configuration

Effective configuration is key to getting your application up and running.

#### 🤫 Secret Management

This solution uses a flexible secret management system that can be configured for different environments.

*   **Local Development (`secrets.json`)**: For local development, the application is configured to use the **.NET Secret Manager**. The secrets are stored in a `secrets.json` file, which is located outside of the project directory and is not checked into source control. To use this, set the `Secret:Provider` in `appsettings.Development.json` to `JsonFile`.

    ```json
    "Secret": {
      "Provider": "JsonFile",
      "JsonFile": {
        "FilePath": "secrets.json"
      }
    }
    ```

*   **Staging/Production (Hashicorp Key Vault)**: For published environments, the application is configured to use **Hashicorp Key Vault**. To use this, set the `Secret:Provider` in your environment-specific `appsettings.json` to `PertaminaVault` and provide the necessary configuration.

#### 🔐 IdAMan Authentication Configuration

This project utilizes **IdAMan** for authentication and authorization.

**For `05.BackEnd.WebApi` (API):**

*   **`appsettings.Development.json` Configuration:**

    ```json
    "IdAMan": {
      "Authentication": {
        "AuthorityUrl": "https://login-v3.qa.idaman.pertamina.com",
        "HealthCheckEndpoint": "/healthcheck",
        "TokenEndpoint": "/connect/token",
        "ApiAudienceScope": "SolTem2.API"
      }
    }
    ```

**For `05.FrontEnd.WebUi` (Blazor UI):**

*   **`appsettings.Development.json` Configuration:**

    ```json
    "IdAMan": {
      "Authentication": {
        "AuthorityUrl": "https://login-v3.qa.idaman.pertamina.com",
        "HealthCheckEndpoint": "/healthcheck",
        "TokenEndpoint": "/connect/token",
        "ApiAudienceScope": "SolTem2.API"
      }
    }
    ```

#### 🔑 Hashicorp Key Vault Implementation

This solution is configured to integrate with **Hashicorp Key Vault** for securely retrieving sensitive configuration values.

*   **Configuration**: The configuration for Hashicorp Key Vault is defined within the `appsettings.json` files under the `Secret:PertaminaVault` section.

    ```json
    "Secret": {
      "Provider": "PertaminaVault",
      "PertaminaVault": {
        "ServerUrl": "https://dev-dc-vault.pertamina.com:8200",
        "RoleId": "xxx",
        "SecretId": "xxx",
        "Namespace": "xxx",
        "SecretPath": "xxx",
        "MountPoint": "xxx"
      }
    }
    ```

*   **What to change**: To use this for another application, you will need to update the `RoleId`, `SecretId`, `Namespace`, `SecretPath`, and `MountPoint` with the values for your application's Vault instance.

#### 📧 Email Blast Implementation

This template includes an **`EmailBlast`** configuration for sending bulk emails.

*   **Configuration**: The configuration for `EmailBlast` is defined within the `appsettings.json` file in the `WebApi` project under the `Email:EmailBlast` section.

    ```json
    "EmailBlast": {
      "RestBaseUrl": "https://apps.pertamina.com/mblast20-api",
      "HealthCheckEndpoint": "index.html",
      "Scope": "mb.client.send",
      "ApiPathBase": "/api",
      "ResourceEndpoint": "/mails/send/noTemplate"
    }
    ```

*   **What to change**: This configuration is ready for use. You don't need to change anything unless you have a different Email Blast service setup.

### 🏗️ Building the Application

To build the solution, run the following command from the root directory:

```cmd
dotnet build
```

### ▶️ Running the Application

1.  **Run the Web API (`05.BackEnd.WebApi`)**:
    ```cmd
    cd src/02.BackEnd/05.BackEnd.WebApi
    dotnet run
    ```
2.  **Run the Blazor UI (`05.FrontEnd.WebUi`)**:
    ```cmd
    cd src/03.FrontEnd/05.FrontEnd.WebUi
    dotnet run
    ```

### 💾 Database Migrations

This template uses **Entity Framework Core** for data persistence. We use the Entity Framework Core tools to manage database migrations.
To install the EF Core tools, run the following command:
```cmd
dotnet tool install --global dotnet-ef
```

To update the EF Core tools to the latest version, run:
```cmd
dotnet tool update --global dotnet-ef
```

To add a new migration or apply migrations to the database, use the following commands:

1.  **Add a new migration**:

    Make sure you are in the `src\02.BackEnd` directory, then run:
    ```cmd
    dotnet ef migrations add <MigrationName> -p 03.BackEnd.Infrastructure -s 05.BackEnd.WebApi -o Database/Migrations
    ```
2.  **Apply migrations to the database**:

    Make sure you are in the `src\02.BackEnd` directory, then run:
    ```cmd
    dotnet ef database update -p 03.BackEnd.Infrastructure -s 05.BackEnd.WebApi
    ```

---

## 4. 🔗 FrontEnd-BackEnd Communication and Testing

### 🤝 FrontEnd-BackEnd Connection

The Blazor front-end communicates with the back-end API via HTTP requests. The base URL of the back-end API is configured in the `appsettings.Development.json` file of the `05.FrontEnd.WebUi` project.

### 🔐 Security Flow

This solution uses OpenID Connect (OIDC) for authentication, with IdAMan as the identity provider.

1.  A user attempts to access a protected page in the Blazor application.
2.  The application redirects the user to the IdAMan login page.
3.  After a successful login, IdAMan redirects the user back to the Blazor application with an authorization code.
4.  The Blazor application exchanges the authorization code for an access token and an ID token.
5.  The access token is then used to make authenticated HTTP requests to the back-end API.

### 🧪 Testing the FrontEnd

1.  **Run the `05.FrontEnd.WebUi` project**.
2.  **Access the web app in your browser** (the URL will be displayed in the console).
3.  **Log in** using your IdAMan credentials.
4.  **Test the application's functionality** by navigating through the different pages and interacting with the UI.

### 🧪 Testing the BackEnd

1.  **Run the `05.BackEnd.WebApi` project**.
2.  **Access the Scalar UI** in your browser (typically at `/scalar`). This will display the API documentation.
3.  **Test the API endpoints** directly from the Scalar UI. You can provide parameters, send requests, and view the responses.

---

## 5. ✅ Testing

*   **Unit Tests**: Should be created in a separate `tests` folder. They should focus on testing the `Domain` and `Logics` layers in isolation, using a mocking framework like **Moq** or **NSubstitute**.
*   **Integration Tests**: Should also be in a separate `tests` folder. They should test the integration between the `WebApi` and the database, as well as other external services.

---

## 6. 🚀 CI/CD

A CI/CD pipeline can be set up using **Azure DevOps** or **GitHub Actions**. The pipeline should be configured to:

1.  **Build the solution**
2.  **Run unit and integration tests**
3.  **Publish the `WebApi` and `WebUi` artifacts**
4.  **Deploy the applications**

---

## 7. ❓ Troubleshooting

*   **API not starting**: Check if the `05.BackEnd.WebApi` project is running and that the port numbers are configured correctly.
*   **Database Connection Issues**: Verify the connection string in your secret management system and ensure the database server is accessible.
*   **Authentication Errors**: Double-check your IdAMan configuration in `appsettings.Development.json` and in your secret management system.

---

## 8. 👥 Role-Based Access Control (RBAC)

Here is the RBAC (Role-Based Access Control) configuration for the Solution Template 2 application.

### Roles
1. Executive
   - SolTem2.Administration
   - SolTem2.Administration.Audits.Read
   - SolTem2.Administration.Configurations.Read
   - SolTem2.Main.Claims.Read
   - SolTem2.MasterData
   - SolTem2.MasterData.Countries.Read
2. Administrator
   - SolTem2.Administration
   - SolTem2.Administration.Audits.Read
   - SolTem2.Administration.Configurations.Read
   - SolTem2.Administration.Configurations.Write
   - SolTem2.Main.Claims.Read
3. Master Data Manager
   - SolTem2.MasterData
   - SolTem2.MasterData.Countries.Read
   - SolTem2.MasterData.Countries.Write

### Positions
1. Chief Executive Officer
   - Executive
2. Chief Entertainment Officer
   - Administrator
3. Cinema Manager
   - Master Data Manager

### Users
1. arturo.angelini@soltem.com
   - Chief Executive Officer (Permanent Position): Executive
2. bernardo.balzano@soltem.com
   - Chief Entertainment Officer (Permanent Position): Administrator
3. cecillia.castellani@soltem.com
   - Cinema Manager (Permanent Position): Master Data Manager
   - Personal: Administrator
