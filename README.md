# KapeRest Api ☕

A modest backend solution built with **ASP.NET Core**, designed to help small to medium café operations manage their daily tasks. This project explores the implementation of Point of Sale (POS), inventory tracking, and basic analytics using a clean, accessible API.


## Key Features

* **User Support**: Simple role-based accounts to help manage different team permissions.
* **POS Essentials**: A straightforward interface focused on processing transactions efficiently.
* **Payment Options**: Integration with **PayMongo** to facilitate GCash payments.
* **Branch Management**: Tools to help track sales and staff performance across specific locations.
* **Inventory Tracking**: Basic automated stock updates and alerts to help prevent shortages.
* **Simple Analytics**: Daily and monthly reports to provide a clear view of business health.
* **Record Keeping**: Digital transaction history and reprint support for better bookkeeping.



## Tech Stack

| Component | Technology |
| --- | --- |
| **Framework** | ASP.NET Core 9.0 (Clean Architecture) |
| **Database** | MySQL 8.0+ |
| **Security** | ASP.NET Identity + JWT |
| **Payments** | PayMongo API |



## Getting Started

### Prerequisites

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [suspicious link removed]
* [Git](https://git-scm.com/)

### Installation

1. **Clone the Project**
```bash
git clone https://github.com/your-username/KapeRest.git
cd KapeRest

```


2. **Database Preparation**
Create a local database to get started:
```sql
CREATE DATABASE KapeRest;
-- Feel free to create a dedicated user for better security
CREATE USER 'kaperest_user'@'localhost' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON KapeRest.* TO 'kaperest_user'@'localhost';

```


3. **Local Configuration**
Create a `.env` file in the `KapeRest.Api/` folder. You can use these settings as a starting point:
```env
KapeRest_DB=Server=localhost;Port=3306;Database=KapeRest;Uid=your_username;Pwd=your_password;

JWT_ISSUER=https://localhost:5001
JWT_AUDIENCE=https://localhost:5001
JWT_KEY=PleaseUseAStrongSecretKeyWithAtLeast32Characters

Admin_Email=admin@kaperest.com
Admin_Password=Admin@123456

ApiKey=your_paymongo_key

```


4. **Running the API**
```bash
cd KapeRest.Api
dotnet ef database update
dotnet run

```


The API should now be available at `https://localhost:5001`.




## Usage & Documentation

Once the API is running, you can explore the available endpoints through the **Swagger UI**. It’s a great way to see how the system works and test out the logic.

* **Documentation**: `https://localhost:5001/swagger`
* **Default Admin**: `admin@kaperest.com` | `Admin@123456`




## Minor Troubleshooting

* **Connection**: If the app won't start, please double-check that your MySQL service is active.
* **Migrations**: Ensure the `dotnet-ef` tool is installed globally if the update command fails.
* **Ports**: If port 5001 is occupied, you can adjust the settings in `launchSettings.json`.




## License

This project is shared under the **MIT License**. Please see the `LICENSE` file for more details.

---

Built with ❤️ by Joshuaesc
