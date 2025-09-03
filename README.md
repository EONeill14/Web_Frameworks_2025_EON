# Athlete Trade - Community Swapping Platform

This project is a web application built with ASP.NET Core MVC for a community organization that facilitates members swapping goods and services. The platform is designed for a not-for-profit organization and includes features for user registration, item listing, admin approvals, a real-time messaging system, a formal trade request system, and a donation facility.

This project was developed as a requirement for the Web Frameworks module.

## Features

* **Full CRUD Functionality:** Users can list, view, edit, and delete their own items and services.
* **Goods & Services:** The platform supports listings for both physical goods (with properties like Brand and Condition) and intangible services (like lessons).
* **Secure Authentication & Authorization:**
    * Three distinct user roles: Guest, Registered User, and Admin.
    * Three login methods: Local username/password, Google, and Microsoft external logins.
    * Users can only edit or delete their own listings.
* **Admin Panel:**
    * Admin users can approve new items before they appear publicly.
    * Admins can manage the list of available categories.
* **Real-Time Messaging:** A private, real-time messaging system built with SignalR allows users to communicate securely without sharing personal emails.
* **Trade Request System:** Users can formally request an item, and the owner can accept or deny the request. Accepting a request automatically decrements the item's available quantity.
* **Donation Facility:** A fully functional donation system integrated with the Stripe API.
* **Search & Filtering:** Users can filter listings by category and search by name.
* **Unit Tested & Well-Designed:** The application uses the Repository Pattern to separate concerns and includes a suite of xUnit tests to ensure code quality.

## Technologies Used

* **Backend:** ASP.NET Core MVC (.NET 9), Entity Framework Core, ASP.NET Core Identity, SignalR
* **Frontend:** HTML, CSS, Bootstrap 5, JavaScript, jQuery
* **Database:** SQL Server (LocalDB)
* **Testing:** xUnit, Moq, EF Core In-Memory Database
* **APIs:** Stripe API, Google OAuth, Microsoft OAuth

## Setup and Installation

To run this project locally, you will need the .NET 9 SDK installed.

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/your-username/your-repo-name.git](https://github.com/your-username/your-repo-name.git)
    ```
2.  **Configure API Keys (User Secrets):**
    This project uses the .NET User Secrets manager to store sensitive API keys. In the root directory of the project, run the following commands in your terminal, replacing the placeholders with your own keys:
    ```bash
    # Initialize User Secrets
    dotnet user-secrets init

    # Stripe Keys
    dotnet user-secrets set "Stripe:PublishableKey" "pk_test_YOUR_KEY_HERE"
    dotnet user-secrets set "Stripe:SecretKey" "sk_test_YOUR_KEY_HERE"

    # Google Auth Keys
    dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID_HERE"
    dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET_HERE"
    
    # Microsoft Auth Keys
    dotnet user-secrets set "Authentication:Microsoft:ClientId" "YOUR_CLIENT_ID_HERE"
    dotnet user-secrets set "Authentication:Microsoft:ClientSecret" "YOUR_CLIENT_SECRET_VALUE_HERE"
    ```
3.  **Create and Seed the Database:**
    * Open the solution in Visual Studio.
    * In the **Package Manager Console**, run the `Update-Database` command. This will create the database from the latest migration.
    * `Update-Database`
4.  **Run the Application:**
    * Run the project from Visual Studio (press F5 or the Start button).
    * The application will seed the database with an admin account, sample users, and sample items/services on its first run.

## Sample Accounts

The database is seeded with the following accounts for testing purposes.
* **Password for all accounts:** `Pass123!`

| Email             | Role             |
| ----------------- | ---------------- |
| `admin@admin.com` | Admin            |
| `user1@test.com`  | Registered User  |
| `user2@test.com`  | Registered User  |
| `user3@test.com`  | Registered User  |

## References
Pro ASP.NET CORE 6 Ninth Edition - Adam Freeman
