# Enforca Assesment Authentication API

A robust .NET 8 Web API built with Clean Architecture principles, featuring user authentication, email verification, and comprehensive API documentation.

## 🚀 Features

- **Clean Architecture** - Organized in Domain, Application, Infrastructure, and Web layers
- **User Authentication** - Registration, login, and email verification
- **Email Verification** - Secure verification code system
- **JWT Authentication** - Token-based security
- **Entity Framework Core** - Code-first database approach
- **Swagger Documentation** - Interactive API documentation
- **SQL Server** - Robust database backend
- **Repository Pattern** - Clean data access layer

## 🏗️ Architecture

```
Enforca/
├── Domain/           # Business entities and core logic
├── Application/      # Use cases and DTOs
├── Infrastructure/   # Data access and external services
└── Web/             # API controllers and configuration
```

## 🛠️ Technologies

- **.NET 8** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation
- **JWT Bearer** - Authentication tokens

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## 🚦 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/enforca-api.git
cd enforca-api
```

### 2. Configure Database Connection
Update the connection string in `Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EnforcaDb;Trusted_Connection=true;"
  }
}
```

### 3. Configure Email Settings (Optional)
For email verification functionality, add to `appsettings.json`:
```json
{
  "EmailSettings": {
    "FromEmail": "your-email@gmail.com",
    "FromName": "Enforca App",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### 4. Install Dependencies
```bash
dotnet restore
```

### 5. Create Database
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create and apply migrations
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Web
dotnet ef database update --project Infrastructure --startup-project Web
```

### 6. Run the Application
```bash
cd Web
dotnet run
```

The API will be available at:
- **HTTPS**: https://localhost:7001
- **HTTP**: http://localhost:5000
- **Swagger UI**: https://localhost:7001/swagger

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/verify-email` - Email verification
- `POST /api/auth/resend-verification` - Resend verification code

### User Management
- `GET /api/users/profile` - Get user profile (authenticated)
- `PUT /api/users/profile` - Update user profile (authenticated)

## 📝 API Usage Examples

### Register a New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

### Verify Email
```http
POST /api/auth/verify-email
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "code": "123456"
}
```

## 🗄️ Database Schema

### Users Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| FullName | nvarchar(100) | User's full name |
| Email | nvarchar(255) | User's email (unique) |
| PasswordHash | nvarchar(500) | Hashed password |
| IsEmailVerified | bit | Email verification status |
| CreatedAt | datetime2 | Creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

### VerificationCodes Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| UserId | uniqueidentifier | Foreign key to Users |
| Code | nvarchar(6) | Verification code |
| CreatedAt | datetime2 | Creation timestamp |
| ExpiresAt | datetime2 | Expiration timestamp |
| IsUsed | bit | Usage status |

## 🔧 Development

### Adding New Migrations
```bash
dotnet ef migrations add YourMigrationName --project Infrastructure --startup-project Web
dotnet ef database update --project Infrastructure --startup-project Web
```

### Running Tests
```bash
dotnet test
```

### Project Structure Details
```
Domain/
├── Entities/         # Domain entities (User, VerificationCode)
├── Interfaces/       # Repository interfaces
└── Enums/           # Domain enums

Application/
├── DTOs/            # Data Transfer Objects
├── Services/        # Application services
├── Interfaces/      # Service interfaces
└── Mappings/        # AutoMapper profiles

Infrastructure/
├── Data/            # DbContext and configurations
├── Repositories/    # Repository implementations
├── Services/        # Infrastructure services (Email, etc.)
└── Migrations/      # EF Core migrations

Web/
├── Controllers/     # API controllers
├── Middleware/      # Custom middleware
└── Extensions/      # Service extensions
```

## 🔐 Security Features

- **Password Hashing** - Secure password storage with bcrypt
- **JWT Tokens** - Stateless authentication
- **Email Verification** - Account security through email confirmation
- **Input Validation** - Comprehensive request validation
- **CORS Configuration** - Cross-origin request handling

## 🌐 Environment Configuration

### Development
- Uses SQL Server LocalDB
- Detailed error messages
- Swagger UI enabled

### Production
- Configure production database connection
- Set up proper email service (SendGrid, etc.)
- Enable HTTPS enforcement
- Configure logging

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Francis Gbohunmi**
- GitHub: [@francisgbohunmi](https://github.com/gbohunmifrancis)
- Email: francisgbohunmi@gmail.com

## 🙏 Acknowledgments

- Built with .NET 8 and Entity Framework Core
- Clean Architecture principles
- RESTful API best practices

---

⭐ If you found this project helpful, please give it a star!
