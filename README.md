# 🎉 Celebration Manager - Wedding & Event Invitation Platform

A modern, full-stack platform for creating beautiful digital invitations for weddings, baptisms, quinceañeras, and other special celebrations.

## ✨ Features

- **Multi-Event Support**: Wedding, Baptism, Quinceañera, Birthday Party, and more
- **Beautiful Event Creation**: 3-step form with theme customization
- **Guest Management**: Invite and manage guests with RSVP tracking
- **Responsive Design**: Works perfectly on desktop and mobile
- **Secure Authentication**: JWT-based user authentication
- **Real-time Updates**: Live event statistics and RSVP tracking

## 🛠️ Technology Stack

### Frontend
- **React 19** with TypeScript
- **Vite** for fast development
- **Tailwind CSS** for styling
- **React Router** for navigation
- **Axios** for API calls
- **Lucide React** for icons

### Backend
- **ASP.NET Core 9.0** Web API
- **Entity Framework Core** ORM
- **Microsoft SQL Server** database
- **JWT Authentication** with ASP.NET Identity
- **Swagger/OpenAPI** documentation

### Development Tools
- **Visual Studio Code**
- **Git** version control
- **npm** package manager
- **.NET CLI** for backend

## 🚀 Getting Started

### Prerequisites
- **Node.js** (v18 or higher)
- **.NET 9.0 SDK**
- **SQL Server** (LocalDB or full instance)
- **Git**

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/celebration-manager.git
   cd celebration-manager
   ```

2. **Setup Backend**
   ```bash
   cd server
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

3. **Setup Frontend**
   ```bash
   cd client-ts
   npm install
   npm run dev
   ```

4. **Access the application**
   - Frontend: http://localhost:5173
   - Backend API: https://localhost:7193
   - Swagger UI: https://localhost:7193/swagger

## 📁 Project Structure

```
celebration-manager/
├── client-ts/                 # React TypeScript Frontend
│   ├── src/
│   │   ├── components/       # Reusable UI components
│   │   ├── pages/           # Page components
│   │   ├── contexts/        # React contexts
│   │   └── types/           # TypeScript definitions
│   ├── package.json
│   └── vite.config.ts
├── server/                   # ASP.NET Core Backend
│   ├── Controllers/         # API controllers
│   ├── Models/             # Entity models
│   ├── Data/               # Database context
│   ├── DTOs/               # Data transfer objects
│   └── Services/           # Business logic
└── README.md
```

## 🎯 Current Features

### ✅ Completed
- User authentication (register/login)
- Event creation with multiple celebration types
- Protected routing
- Database schema with relationships
- Event management API
- Responsive dashboard
- Theme customization

### 🚧 In Development
- Guest management system
- RSVP tracking
- Email notifications
- Table assignments
- Media upload
- QR code generation

## 🔧 Development

### Database Migrations
```bash
cd server
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Running Tests
```bash
# Frontend tests
cd client-ts
npm test

# Backend tests
cd server
dotnet test
```

## 🌟 Event Types Supported

- **Weddings** - Complete wedding management
- **Engagements** - Engagement announcements
- **Baptisms** - Religious celebrations
- **First Communions** - Religious milestones
- **Quinceañeras** - 15th birthday celebrations
- **Bar/Bat Mitzvahs** - Coming of age ceremonies
- **Birthday Parties** - Birthday celebrations
- **Graduations** - Academic achievements
- **Baby Showers** - Baby welcoming parties
- **Bridal Showers** - Pre-wedding celebrations

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📞 Support

If you have any questions or need help, please open an issue on GitHub.

---

**Built with ❤️ for celebrating life's special moments**
