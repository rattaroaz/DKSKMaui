# DKSKMaui - Painting Contractor Management System

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![Platform](https://img.shields.io/badge/Platform-Windows%2010+-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![Build Status](https://img.shields.io/badge/Build-Passing-success)
![Tests](https://img.shields.io/badge/Tests-95%20Passing-success)

## 📋 Overview

DKSKMaui is a comprehensive business management application designed specifically for DKSK Official, a painting contractor company. Built with .NET MAUI and Blazor Hybrid technology, this cross-platform application streamlines various business operations including invoice management, contractor tracking, job scheduling, and financial reporting.

## ✨ Features

### Core Functionality
- **Invoice Management**: Create, track, and manage invoices with automatic numbering
- **Contractor Management**: Maintain contractor database with active/inactive status tracking
- **Company & Property Management**: Manage client companies and property locations
- **Job Tracking**: Track job descriptions, work orders, and completion status
- **Payment Processing**: Record multiple payments per invoice with check tracking
- **Accounts Receivable**: Monitor outstanding payments and aging reports
- **Reporting**: Generate PDF invoices and Excel reports

### Business Modules
- 📊 **Sales Management**: Track sales performance and opportunities
- 💼 **Payroll Processing**: Manage contractor payments
- 📅 **Job Scheduling**: Schedule and assign jobs to contractors
- 🏢 **Company Profile**: Manage your company information
- 📈 **Aging Reports**: Track payment aging and overdue accounts
- 🔧 **Active Jobs**: Monitor ongoing projects

## 🛠️ Technology Stack

- **Framework**: .NET MAUI with Blazor Hybrid
- **Target Platform**: Windows 10 (10.0.19041.0+)
- **Language**: C# 12 with .NET 8.0
- **Database**: SQLite with Entity Framework Core 8.0.8
- **UI Components**: Radzen Blazor Components 4.32.6
- **Authentication**: BCrypt.Net-Next 4.0.3
- **Document Generation**: 
  - PDF: PdfSharpCore 1.3.65
  - Excel: ClosedXML 0.102.3
- **Data Serialization**: Newtonsoft.Json 13.0.3

## 📋 Prerequisites

Before setting up the application, ensure you have the following installed:

### Required Software
1. **Visual Studio 2022** (version 17.8 or later)
   - Workloads needed:
     - .NET Multi-platform App UI development
     - ASP.NET and web development
     
2. **Windows SDK** (10.0.19041.0 or later)
   
3. **Windows App SDK Runtime**
   - Download from: [Microsoft Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads)
   - Install the latest WindowsAppRuntimeInstall.exe

4. **.NET 8.0 SDK**
   - Download from: [.NET 8.0 Downloads](https://dotnet.microsoft.com/download/dotnet/8.0)

### System Requirements
- **Operating System**: Windows 10 version 1903 or higher
- **RAM**: Minimum 8GB (16GB recommended)
- **Storage**: At least 2GB free space
- **Processor**: x64 architecture

## 🚀 Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/DKSKMaui.git
cd DKSKMaui
```

### 2. Install Windows App SDK Runtime
If you haven't already installed the Windows App SDK runtime:
```powershell
# Download and run the installer
Invoke-WebRequest -Uri "https://aka.ms/windowsappsdk/1.4/latest/windowsappruntimeinstall-x64.exe" -OutFile "WindowsAppRuntimeInstall.exe"
.\WindowsAppRuntimeInstall.exe
```

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Build the Application
```bash
dotnet build -c Release
```

### 5. Run the Application
```bash
dotnet run -c Release
```

Or from Visual Studio:
1. Open `DKSKMaui.sln` in Visual Studio 2022
2. Set build configuration to `Release`
3. Press `F5` or click the `Run` button

## 🏗️ Project Structure

```
DKSKMaui/
├── Backend/                 # Business logic and data layer
│   ├── Data/               # Database context and configurations
│   ├── Dto/                # Data transfer objects
│   ├── Models/             # Entity models
│   │   ├── Companny.cs     # Company entity (Note: typo to be fixed)
│   │   ├── Contractor.cs   # Contractor entity
│   │   ├── Invoice.cs      # Invoice entity
│   │   ├── JobDiscription.cs # Job description (Note: typo to be fixed)
│   │   ├── MyCompanyInfo.cs # Company profile
│   │   ├── Properties.cs   # Property entity
│   │   └── Supervisor.cs   # Supervisor entity
│   └── Services/           # Business services
│       ├── CompanyService.cs
│       ├── ContractorService.cs
│       ├── InvoiceService.cs
│       └── ...
├── Components/             # Blazor UI components
│   ├── Pages/             # Application pages
│   │   ├── AccountsReceivable/
│   │   ├── ActiveJobs/
│   │   ├── AddContactFolder/
│   │   ├── CreteInvoice/
│   │   ├── EditContacts/
│   │   └── ...
│   ├── Layout/            # Layout components
│   └── Common/            # Shared components
├── Platforms/             # Platform-specific code
│   └── Windows/          # Windows-specific implementations
├── Resources/             # Application resources
│   ├── AppIcon/          # Application icons
│   ├── Fonts/            # Custom fonts
│   └── Images/           # Image assets
├── wwwroot/              # Static web assets
├── MauiProgram.cs        # Application entry point
└── DKSKMaui.csproj      # Project file
```

## ⚙️ Configuration

### Database Configuration
The application uses SQLite database stored in the application data directory. The database is automatically created on first run.

Database location:
```
%LOCALAPPDATA%\DKSKMaui\app.db
```

### Application Settings
Edit `MauiProgram.cs` to configure:
- Database connection string
- Logging levels
- Service registrations

### Company Information
On first launch, navigate to the Home page to set up your company information:
- Company Name
- Phone Number
- Email Address
- Physical Address
- License Number
- Zip Code

## 🧪 Testing

### Running Tests
The DKSKMaui.Tests project provides comprehensive testing:
```bash
# Run all tests
dotnet test

# Run with coverage (Windows PowerShell)
.\DKSKMaui.Tests\run-tests.ps1 -OpenReport

# Or use the batch file
.\DKSKMaui.Tests\run-tests.bat

# Run specific test category
dotnet test --filter Category=Unit

# Run tests for a specific service
dotnet test --filter "FullyQualifiedName~InvoiceService"
```

### Test Categories
- **Unit Tests**: Test individual services and models
- **Integration Tests**: Test database operations and workflows
- **Component Tests**: Test Blazor UI components

## 🐛 Troubleshooting

### Common Issues and Solutions

#### 1. Application Won't Start
**Error**: "Class not registered (REGDB_E_CLASSNOTREG)"
**Solution**: Install Windows App SDK runtime (see Prerequisites)

#### 2. Database Errors
**Error**: "Cannot open database file"
**Solution**: 
```bash
# Clear the database
del %LOCALAPPDATA%\DKSKMaui\app.db
# Restart the application
```

#### 3. Build Errors
**Error**: "The target platform identifier windows was not recognized"
**Solution**: Ensure you have the correct Windows SDK installed

#### 4. Window Not Displaying
**Error**: Application runs but no window appears
**Solution**: Verify OutputType is set to "WinExe" in the .csproj file

#### 5. Missing Dependencies
**Solution**: 
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
# Restore packages
dotnet restore --force
```

## 📝 Development Notes

### Building for Production
```bash
# Create a release build
dotnet publish -c Release -r win10-x64 --self-contained

# Output will be in: bin\Release\net8.0-windows10.0.19041.0\win10-x64\publish\
```

### Database Migrations
When modifying entity models:
```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### Code Quality
The project is configured to suppress certain warnings. To see all warnings:
1. Remove the `<NoWarn>` element from DKSKMaui.csproj
2. Rebuild the project

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Coding Standards
- Follow C# naming conventions
- Use async/await for database operations
- Add XML documentation to public methods
- Write unit tests for new features
- Ensure no build warnings before submitting

### Areas for Contribution
- Fix typos in model names (Companny → Company, JobDiscription → JobDescription)
- Add data validation attributes to models
- Implement caching strategies
- Add dark mode support
- Improve responsive design
- Add more comprehensive error messages

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Contact & Support

**DKSK Official - Painting Contractor**
- Email: [contact@dkskofficial.com]
- Phone: [Your Phone Number]
- Website: [www.dkskofficial.com]

For technical support or questions about this application, please open an issue in the GitHub repository.

## 🙏 Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- UI components by [Radzen](https://www.radzen.com/)
- PDF generation by [PdfSharpCore](https://github.com/ststeiger/PdfSharpCore)
- Excel support by [ClosedXML](https://github.com/ClosedXML/ClosedXML)

## 📈 Version History

- **v1.0.0** (Current)
  - Initial release
  - Core invoice management functionality
  - Contractor management
  - Basic reporting features
  - Windows 10+ support

## 🚧 Roadmap

### Short Term (v1.1)
- [ ] Fix model name typos
- [ ] Add comprehensive data validation
- [ ] Implement audit logging
- [ ] Add backup/restore functionality

### Medium Term (v1.2)
- [ ] Multi-user support
- [ ] Role-based access control
- [ ] Email notifications
- [ ] Mobile responsive design

### Long Term (v2.0)
- [ ] Cloud synchronization
- [ ] Mobile app versions (iOS/Android)
- [ ] Advanced analytics dashboard
- [ ] Integration with accounting software
- [ ] Multi-language support

---

**Built with ❤️ by the DKSK Development Team**
