# ☕ KapeRest Api

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-orange)](https://www.mysql.com/)

Backend built with ASP.NET Core, featuring Point of Sale, Inventory Management, and Analytics APIs.

## 📋 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Usage](#-usage)
- [API Documentation](#-api-documentation)
- [Troubleshooting](#-troubleshooting)
- [Contributing](#-contributing)
- [License](#-license)

## ⚙️ Features

- **User Management**: Role-based accounts with secure authentication
- **Point of Sale (POS)**: Responsive interface for quick transactions
- **Payment Integration**: Seamless GCash payments via PayMongo
- **Branch-Based Cashier System**: Branch-bound cashiers for precise reporting
- **Real-Time Sales Notification**: Live monitoring of transactions
- **Inventory Management**: Auto-updates stock levels and low item alerts
- **Supplier Management**: Centralized supplier and purchase records
- **Sales & Analytics**: Daily, weekly, and monthly reports
- **Transaction History**: Full audit log with reprint support
- **System Configuration**: Adjustable tax, discount, and operation parameters  

## 🧩 Tech Stack

| Component    | Technology                              |
|--------------|-----------------------------------------|
| **Backend**  | ASP.NET Core Web API (Clean Architecture) |
| **Database** | MySQL                                   |
| **Auth**     | ASP.NET Identity + JWT                  |
| **Payments** | PayMongo API                            |

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)
- [Git](https://git-scm.com/)

## 🚀 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/KapeRest.git
   cd KapeRest
   ```

2. **Set up the database**
   - Install and start MySQL Server (version 8.0 or later)
   - Open MySQL Command Line Client or MySQL Workbench
   - Create a new database:
     ```sql
     CREATE DATABASE KapeRest;
     ```
   - Create a MySQL user (optional, but recommended for security):
     ```sql
     CREATE USER 'kaperest_user'@'localhost' IDENTIFIED BY 'your_secure_password';
     GRANT ALL PRIVILEGES ON KapeRest.* TO 'kaperest_user'@'localhost';
     FLUSH PRIVILEGES;
     ```
   - Note down your database credentials (username and password)

3. **Configure environment variables**
   - Navigate to `KapeRest.Api/` directory
   - Create a `.env` file (copy from `.env.example` if it exists, or create new):
     ```env
     # Database Configuration
     # Replace with your actual MySQL credentials
     KapeRest_DB=Server=localhost;Port=3306;Database=KapeRest;Uid=your_mysql_username;Pwd=your_mysql_password;

     # JWT Configuration
     JWT_ISSUER=https://localhost:5001
     JWT_AUDIENCE=https://localhost:5001
     JWT_KEY=YourSuperSecretKeyHereThatShouldBeAtLeast32CharactersLong
     JWT_TOKEN_DURATION_MINUTES=60
     JWT_REFRESH_TOKEN_DURATION_MINUTES=10

     # Admin Seeded Account
     Admin_Email=admin@kaperest.com
     Admin_Password=Admin@123456
     Admin_FirstName=System
     Admin_MiddleName=Administrator
     Admin_LastName=Administrator

     # PayMongo API Configuration
     ApiKey=your_paymongo_api_key_here
     ```
   - **Important**: Update the `KapeRest_DB` connection string with your actual MySQL username and password
   - For production, use a strong, unique JWT key (at least 32 characters)

4. **Run database migrations**
   - Open a terminal/command prompt in the `KapeRest.Api` directory
   - Apply the database schema:
     ```bash
     dotnet ef database update
     ```
   - This will create all necessary tables, relationships, and seed initial data

5. **Run the backend**
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:5001`.

## 📖 Usage

1. Start the backend API as described in Installation.
2. Access the API endpoints or use the Swagger UI at `https://localhost:5001/swagger`.
3. The admin account is seeded with credentials from `.env` for initial access.
4. Use the API to manage café operations including POS, inventory, and analytics.

### Default Admin Credentials
- **Email**: admin@kaperest.com
- **Password**: Admin@123456

### API Endpoints
- **Swagger UI**: `https://localhost:5001/swagger`
- **Base API URL**: `https://localhost:5001/api`

## 📚 API Documentation

API documentation is available via Swagger at `https://localhost:5001/swagger` when the backend is running. This provides interactive documentation for all endpoints, including request/response examples and the ability to test API calls directly from the browser.

## 🔧 Troubleshooting

### Database Connection Issues
- Ensure MySQL Server is running
- Verify the connection string in `.env` matches your MySQL credentials
- Check that the database `KapeRest` exists
- Test connection using MySQL Workbench or command line

### Migration Errors
- Ensure you have the correct .NET SDK version (9.0)
- Run `dotnet restore` before running migrations
- Check that Entity Framework tools are installed: `dotnet tool install --global dotnet-ef`

### Port Conflicts
- If port 5001 is in use, check `launchSettings.json` for alternative ports
- The API may run on `http://localhost:5000` for HTTP instead of HTTPS

### JWT Configuration
- Ensure the JWT key is at least 32 characters long
- For production, use environment-specific issuer and audience URLs

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Built with ❤️ by John Joshua Manalo Escarez.
