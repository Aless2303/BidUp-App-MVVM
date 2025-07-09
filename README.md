# BidUp - Auction Management System

A comprehensive WPF application for managing online auction houses, built with C# using the MVVM pattern and Entity Framework for data persistence.

## 🎯 Project Overview
BidUp is a real-time auction management system that facilitates online bidding, manages bids in real-time, integrates payment systems, and handles auction certification. The system supports three distinct user roles with specialized functionalities.

## 🏗️ Architecture & Technologies
- **Framework**: .NET Framework 4.7.2
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture Pattern**: MVVM (Model-View-ViewModel)
- **Data Access**: LINQ to SQL with Entity Framework
- **Database**: SQL Server
- **Design Pattern**: Factory Pattern for user creation
- **Real-time Updates**: Timer-based refresh mechanisms

## 👥 User Roles

### 1. Bidder
- View available auctions with real-time updates
- Place bids on active auctions
- Manage wallet balance (add/deduct funds)
- View bidding history
- Receive notifications when outbid

### 2. Seller
- Create and manage auctions
- Set auction parameters (start/end time, starting price)
- Upload product images
- Monitor auction performance
- View earnings through wallet system

### 3. Admin
- Manage all users (view, delete)
- Oversee all auctions
- Close auctions manually
- View detailed user activity logs
- System-wide monitoring capabilities

## 🗄️ Database Schema
The system uses a well-structured relational database with the following key entities:
- **Users**: Core user information with role-based access
- **Auctions**: Auction details with timing and pricing
- **Products**: Product information and images
- **Wallets**: Financial transactions and balances
- **Cards**: Payment method information
- **Notifications**: Real-time user notifications
- **Logs**: Comprehensive activity tracking

## 🔧 Key Features

### Authentication & Security
- SHA256 password hashing
- Role-based access control
- Secure card information handling
- Password verification for sensitive operations

### Real-time Auction Management
- Live bid updates using DispatcherTimer
- Automatic auction status tracking
- Real-time remaining time calculations
- Bid validation and conflict resolution

### Financial System
- Integrated wallet management
- Automatic fund transfers during bidding
- Balance validation before bid placement
- Transaction logging for audit trails

### Notification System
- Real-time notifications for outbid scenarios
- Automatic notification cleanup
- User-friendly message delivery

### Comprehensive Logging
- Detailed activity tracking with JSON data storage
- User action history
- Bid tracking and audit trails
- Administrative oversight capabilities

## 📁 Project Structure
```
BidUp-App/
├── Models/
│   ├── Users/           # User entities and factory
│   ├── Interfaces/      # Contract definitions
│   └── Loguri/         # Logging models
├── ViewModels/         # MVVM ViewModels
├── Views/
│   ├── Admin/          # Administrative interfaces
│   ├── Bidder/         # Bidder-specific views
│   └── Seller/         # Seller management views
├── UserControls/       # Custom UI components
├── Converters/         # Data binding converters
└── Queries/           # Database scripts
```

## 🎨 UI/UX Design
- **Modern Dark Theme**: Professional appearance with #1e1e2f and #282846 color scheme
- **Responsive Design**: Adaptive layouts using Viewbox containers
- **Custom Controls**: Reusable CustomTextBox with placeholder functionality
- **Role-specific Dashboards**: Tailored interfaces for each user type
- **Real-time Visual Updates**: Live data refresh without page reloads

## 💻 Technical Implementation

### MVVM Pattern Implementation
- **Models**: Entity classes representing database structures
- **Views**: XAML-based user interfaces with data binding
- **ViewModels**: Business logic and data presentation layer
- **Commands**: RelayCommand implementation for UI interactions

### Data Access Layer
- LINQ to SQL for database operations
- Automatic relationship mapping
- Connection string management through app.config
- Entity refresh mechanisms for real-time data

### Custom Components
- **CustomTextBox**: Enhanced input control with placeholder support
- **BoolToVisibilityConverter**: UI state management
- **StartTimeConverter**: Auction timing logic
- **RelayCommand**: Generic command implementation

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2
- SQL Server (LocalDB or full instance)

### Installation
1. Clone the repository
2. Open BidUp-App.sln in Visual Studio
3. Restore NuGet packages
4. Update connection strings in app.config
5. Run database scripts from /Queries folder
6. Build and run the application

### Database Setup
Execute the following scripts in order:
1. Query Creare Users.sql - User and card tables
2. creare_tabel_auctions.sql - Auction and product tables
3. creareTabelWallets.sql - Wallet system
4. SQLQuery1.sql - Logging system

## 📊 Key Functionalities

### Auction Lifecycle
- **Creation**: Sellers create auctions with product details
- **Bidding**: Real-time bid placement and validation
- **Monitoring**: Live updates and notifications
- **Completion**: Automatic closure or manual admin intervention

### Financial Transactions
- Wallet Management: Add/remove funds
- Bid Escrow: Temporary fund holding during active bids
- Automatic Refunds: Return funds when outbid
- Transaction Logging: Complete audit trail

### Administrative Control
- User Management: View, edit, delete users
- Auction Oversight: Monitor and control all auctions
- System Logs: Comprehensive activity tracking
- Manual Interventions: Emergency auction closure

## 🔒 Security Features
- Password hashing using SHA256
- Input validation and sanitization
- Role-based access restrictions
- Secure financial transaction handling
- Audit trail maintenance

## 🚀 Future Enhancements
- Payment gateway integration
- Email notification system
- Mobile application support
- Advanced search and filtering
- Auction scheduling automation
- Multi-language support

Built with ❤️ using WPF, Entity Framework, and the MVVM pattern
