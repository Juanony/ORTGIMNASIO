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

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Bootstrap Icons
- **JavaScript**: Vanilla JS with jQuery for validation

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or Visual Studio Code
- Git (optional)

## Installation & Setup

### 1. Clone or Download the Project

```bash
cd GymMembershipApp
```

### 2. Configure Database Connection

Edit the `appsettings.json` file and update the connection string according to your SQL Server setup:

For LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GymMembershipDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

For SQL Server with authentication:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=GymMembershipDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;MultipleActiveResultSets=true"
}
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Create and Setup Database

Run the following commands to create the database and apply migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will:
- Create the database
- Create all necessary tables (Members, Attendances, Payments, MembershipPlans, AspNetUsers, etc.)
- Seed initial membership plan data (Monthly, Quarterly, Annual)

### 5. Run the Application

```bash
dotnet run
```

Or press F5 in Visual Studio.

The application will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### 6. Create First User

1. Navigate to the registration page
2. Create an admin account
3. Use this account to access all features

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

### Database Connection Issues

- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database user has proper permissions

### Migration Issues

If you encounter migration errors:
```bash
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Already in Use

If ports 5000/5001 are in use, modify `launchSettings.json` or use:
```bash
dotnet run --urls="http://localhost:5002;https://localhost:5003"
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
