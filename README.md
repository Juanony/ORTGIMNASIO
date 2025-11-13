# Gym Membership Management System

A comprehensive ASP.NET MVC application for managing gym memberships, attendance tracking, and payment processing.

## Features

### Member Management
- Add, edit, and delete gym members
- Track member personal information and emergency contacts
- Manage membership plans and expiration dates
- Filter members by status (active, expired, payment due)
- View detailed member profiles with attendance and payment history

### Attendance Tracking
- Quick check-in system with member search
- Track check-in and check-out times
- View today's attendance and currently active members
- Historical attendance records with filtering
- Automatic duration calculation

### Payment Processing
- Record membership payments
- Multiple payment methods (Cash, Credit Card, Debit Card, Bank Transfer)
- Payment status tracking (Completed, Pending, Failed, Refunded)
- Automatic membership extension upon payment
- Payment due reminders
- Payment history and reporting

### Dashboard
- Real-time statistics (total members, active members, expired memberships)
- Today's attendance summary
- Monthly revenue tracking
- Recent activity feeds
- Payment due alerts

### Security
- User authentication and authorization
- Secure login/registration system
- Password requirements and validation

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Bootstrap Icons
- **JavaScript**: Vanilla JS with jQuery for validation

## Prerequisites

Before starting, ensure you have the following installed:

### 1. Install .NET SDK

#### macOS (using Homebrew - Recommended):
```bash
# Install Homebrew if you don't have it
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install .NET SDK
brew install --cask dotnet-sdk

# Verify installation
dotnet --version
```

#### macOS (Official Installer):
1. Visit https://dotnet.microsoft.com/download
2. Download the .NET 9.0 SDK installer for macOS
3. Run the downloaded .pkg file
4. Follow the installation wizard
5. Verify: `dotnet --version`

#### Windows:
1. Visit https://dotnet.microsoft.com/download
2. Download the .NET 9.0 SDK installer
3. Run the installer and follow the wizard
4. Verify: `dotnet --version`

#### Linux (Ubuntu/Debian):
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Verify installation
dotnet --version
```

### 2. Install Entity Framework Core Tools

After installing .NET SDK, install the EF Core CLI tools:

```bash
# Install globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

**If `dotnet ef` command is not found**, add the tools directory to your PATH:

#### macOS/Linux:
```bash
# Add to PATH
export PATH="$PATH:$HOME/.dotnet/tools"

# Make it permanent (for zsh)
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc

# For bash users
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bash_profile
source ~/.bash_profile

# Verify
dotnet ef --version
```

#### Windows (PowerShell):
```powershell
# The tools are usually automatically added to PATH
# If not, add manually:
$env:Path += ";$env:USERPROFILE\.dotnet\tools"

# Verify
dotnet ef --version
```

### 3. Optional Tools
- **Visual Studio 2022** or **Visual Studio Code** (recommended)
- **Git** (for version control)

## Installation & Setup

### Step 1: Clone or Download the Project

```bash
cd GymMembershipApp
```

### Step 2: Verify Database Configuration

The project is configured to use **SQLite** by default (no additional database server required).

The connection string in `appsettings.json` should be:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=GymMembership.db"
}
```

**Note**: If you prefer SQL Server, you'll need to:
1. Install SQL Server
2. Change the connection string in `appsettings.json`
3. Update `Program.cs` to use `UseSqlServer` instead of `UseSqlite`
4. Change the NuGet package from `Microsoft.EntityFrameworkCore.Sqlite` to `Microsoft.EntityFrameworkCore.SqlServer`

### Step 3: Install Dependencies

```bash
dotnet restore
```

This will download all required NuGet packages.

### Step 4: Create and Setup Database

Run the following commands to create the database and apply migrations:

```bash
# Create migration files
dotnet ef migrations add InitialCreate

# Apply migrations and create database
dotnet ef database update
```

**What this does:**
- Creates the SQLite database file `GymMembership.db` in the project folder
- Creates all necessary tables (Members, Attendances, Payments, MembershipPlans, AspNetUsers, etc.)
- Seeds initial membership plan data (Monthly, Quarterly, Annual)

**Troubleshooting EF Commands:**
If you get "command not found" errors:
```bash
# Check if dotnet-ef is installed
dotnet tool list -g

# If not listed, install it
dotnet tool install --global dotnet-ef

# If installed but not working, update it
dotnet tool update --global dotnet-ef

# Verify PATH includes ~/.dotnet/tools (see Prerequisites section)
```

### Step 5: Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio / Visual Studio Code.

The application will start and display:
```
Now listening on: http://localhost:5000
```

**The application is now running!** Open your browser to:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001 (if configured)

To stop the application, press **Ctrl+C** in the terminal.

### Step 6: Create First User

1. Open http://localhost:5000 in your browser
2. Click on **"Register"** in the navigation bar
3. Fill in the registration form:
   - First Name
   - Last Name
   - Email
   - Password (minimum 6 characters, must include uppercase, lowercase, and digit)
4. Click **"Register"**
5. You'll be automatically logged in
6. Start using the application!

## Default Membership Plans

The system comes with three pre-configured membership plans:

1. **Monthly** - $50.00 for 30 days
2. **Quarterly** - $135.00 for 90 days (10% discount)
3. **Annual** - $480.00 for 365 days (20% discount)

You can modify these or add new plans through the database.

## Project Structure

```
GymMembershipApp/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── AttendanceController.cs
│   ├── HomeController.cs
│   ├── MembersController.cs
│   └── PaymentsController.cs
├── Data/                # Database Context
│   └── ApplicationDbContext.cs
├── Models/              # Data Models
│   ├── ApplicationUser.cs
│   ├── Attendance.cs
│   ├── Member.cs
│   ├── MembershipPlan.cs
│   ├── Payment.cs
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
├── Views/               # Razor Views
│   ├── Account/
│   ├── Attendance/
│   ├── Home/
│   ├── Members/
│   ├── Payments/
│   └── Shared/
├── wwwroot/            # Static Files
│   ├── css/
│   └── js/
├── appsettings.json    # Configuration
└── Program.cs          # Application Entry Point
```

## Usage Guide

### Adding a New Member

1. Navigate to Members > Add New Member
2. Fill in personal information
3. Select a membership plan and start date
4. The system will automatically calculate the membership end date
5. Click "Create Member"

### Check-In Process

**Quick Check-In (Recommended):**
1. Navigate to Attendance > Quick Check-In
2. Start typing member's name, email, or phone
3. Select the member from search results
4. Click the check-in button

**Form-Based Check-In:**
1. Navigate to Attendance > Check-In Form
2. Select member from dropdown
3. Add optional notes
4. Submit the form

### Recording Payments

1. Navigate to Payments > New Payment
2. Select the member
3. Enter payment amount
4. Select membership plan (if renewing)
5. Choose payment method
6. Mark status as "Completed"
7. The system will automatically extend membership if a plan is selected

### Viewing Reports

- **Dashboard**: Overview of all statistics
- **Today's Attendance**: See who's currently in the gym
- **Payment Due**: Members whose membership expires within 7 days
- **Member Details**: Complete history for individual members

## Database Migrations

If you make changes to the models, create and apply a new migration:

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

To remove the last migration (if not applied):
```bash
dotnet ef migrations remove
```

## Troubleshooting

### .NET Version Mismatch

**Error**: `You must install or update .NET to run this application`

**Solution**: Check your installed .NET version:
```bash
dotnet --version
```

If you have .NET 9.x but the project targets a different version, the project is already configured for .NET 9.0. If you need to change it:
1. Open `GymMembershipApp.csproj`
2. Change `<TargetFramework>netX.0</TargetFramework>` to match your version
3. Update NuGet package versions accordingly
4. Run `dotnet restore`

### dotnet-ef Command Not Found

**Error**: `Não foi possível executar porque o comando ou arquivo especificado não foi encontrado`

**Solution**:
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Add tools to PATH (macOS/Linux)
export PATH="$PATH:$HOME/.dotnet/tools"
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc

# Verify
dotnet ef --version
```

### Database Connection Issues

**For SQLite** (default configuration):
- The database file `GymMembership.db` will be created automatically
- No additional database server needed
- Check that the connection string in `appsettings.json` is: `"Data Source=GymMembership.db"`

**For SQL Server**:
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database user has proper permissions
- On macOS, consider using Docker for SQL Server:
  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123" -p 1433:1433 --name sql-server -d mcr.microsoft.com/mssql/server:2022-latest
  ```

### Migration Issues

If you encounter migration errors:
```bash
# Remove the database
rm GymMembership.db

# Remove migrations folder
rm -rf Migrations/

# Create fresh migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

### Port Already in Use

**Error**: `Failed to bind to address http://127.0.0.1:5000`

**Solution**: Change the port in one of these ways:

1. **Command line**:
```bash
dotnet run --urls="http://localhost:5002;https://localhost:5003"
```

2. **Modify `Properties/launchSettings.json`**:
```json
"applicationUrl": "https://localhost:7001;http://localhost:5002"
```

3. **Find what's using the port**:
```bash
# macOS/Linux
lsof -i :5000

# Kill the process
kill -9 <PID>
```

### Build Errors After Package Changes

If you get build errors after changing packages:
```bash
# Clean the project
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

## Security Notes

- Change the default connection string password in production
- Enable HTTPS in production
- Use strong passwords for user accounts
- Regularly backup the database
- Keep .NET and packages updated

## Future Enhancements

Potential features to add:
- Email notifications for membership expiration
- SMS reminders
- Member photo uploads
- QR code check-in
- Attendance reports and analytics
- Multi-location support
- Staff role management
- Trainer assignment system
- Class scheduling
- Equipment tracking

## License

This project is for educational purposes. Modify and use as needed.

## Support

For issues or questions, please refer to the ASP.NET Core documentation:
- https://docs.microsoft.com/aspnet/core
- https://docs.microsoft.com/ef/core
