<img src="https://iili.io/Ffa7w5G.png" alt="Demo IMG" style="max-width: 100%; height: auto;" />

# 🚗 Hệ thống Nhận diện Biển số xe Việt Nam (Vietnamese License Plate Recognition System)
# WordWise - Language Learning Platform
[![MIT License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![GitHub stars](https://img.shields.io/github/stars/PhucDaizz/WordWise-BE.svg?style=social)](https://github.com/PhucDaizz/WordWise-BE/stargazers)

**WordWise** is a comprehensive AI-powered language learning platform that helps users master new languages through interactive features such as flashcards, writing exercises, and multiple-choice tests. The backend API is built with ASP.NET Core 8 and integrates AI services to offer a personalized learning experience.

---

## 🧭 Table of Contents

- [Features](#features)
- [System Architecture](#system-architecture)
- [Technologies Used](#technologies-used)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Authentication](#authentication)
- [AI Integration](#ai-integration)
- [License](#license)
- [Contributors](#contributors)

---

## 🚀 Features

### 🔹 Flashcard Sets
- Create and manage personalized flashcard sets
- Auto-generate flashcards by language, topic, and difficulty (via AI)
- Public/private sharing options
- Review and rating system

### 🔹 Writing Exercises
- Submit writings on AI-generated topics
- Receive detailed feedback on grammar, vocabulary, and structure
- Track writing progress over time

### 🔹 Multiple Choice Tests
- Take AI-generated reading comprehension tests
- Create custom tests by difficulty level
- View and track test performance

### 🔹 Additional Capabilities
- User learning statistics and streak tracking
- Reporting system for inappropriate content
- Admin content moderation tools
- Email verification for secure registration

---

## 🏗️ System Architecture
WordWise follows a layered architecture with a clear separation of concerns.
Below is a visual representation of the system architecture:

![WordWise System Architecture](https://iili.io/3Un4Jup.png)


- **Cross-Cutting Concerns**
- **Data Access Layer** (`Entity Framework Core`)
- **Business Logic Layer**
- **API Layer** (Controllers, Services, Repositories)
- **Client Applications**
- **Authentication & Authorization** (JWT + Identity)
- **Caching Layer** (In-memory for API key control)
- **AI Integration Layer** (Gemini AI)

> Follows the standard Controller → Service → Repository pattern.

---

## 💻 Technologies Used

| Layer/Function        | Technology                      |
|----------------------|----------------------------------|
| Backend Framework     | ASP.NET Core 8.0                |
| ORM / Data Access     | Entity Framework Core 8.0       |
| Database              | SQL Server                      |
| Authentication        | JWT + ASP.NET Core Identity     |
| AI Integration        | Google Gemini AI (v3.0.0)       |
| Object Mapping        | AutoMapper                      |
| Email Verification    | SMTP                            |
| Caching               | In-memory cache                 |
| API Documentation     | Swagger / OpenAPI               |

---

## 📡 API Endpoints

### 🛡️ Authentication
- Register (with email verification)
- Login (JWT Token)
- Forgot / Reset Password

### 📚 Flashcard Sets
- CRUD operations
- AI-generated flashcards
- Rating & Reviews

### ✍️ Writing Exercises
- Generate AI-powered writing topics
- Submit and get AI feedback
- View writing history and progress

### 📖 Multiple Choice Tests
- Generate AI-based tests
- Submit answers and receive scores
- Track performance over time

### 📊 Learning Statistics
- Track daily streaks and overall progress

### 🚨 Content Moderation
- Report inappropriate content
- Admin moderation tools

---

## 🛠️ Getting Started

### ✅ Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- Gemini AI API key (if using AI features)

### ⚙️ Setup Instructions

1.  **Clone the repository**
    ```bash
    git clone https://github.com/PhucDaizz/WordWise-BE
    cd WordWise.Api 
    ```
 

2.  **Configure connection strings and settings**
    *   Update `appsettings.json` with your SQL Server connection string.
    *   Add SMTP settings in `appsettings.json` for email verification.
    *   (Optional) Add your Gemini API key in `appsettings.json` for AI services.

3.  **Apply database migrations**
    ```bash
    dotnet ef database update
    ```

4.  **Run the application**
    ```bash
    dotnet run
    ```

5.  **Open the API in your browser**
    *   Navigate to: `https://localhost:5001/swagger` (or the respective port your project is running on) to access Swagger UI.

---

## 🔐 Authentication & Authorization

WordWise uses JWT-based authentication with ASP.NET Core Identity.

*   Role-based access: `User`, `Admin`, `SuperAdmin`
*   Email verification required for new accounts
*   Secure password policies enforced

---

## 🤖 AI Integration

WordWise integrates with **Google Gemini AI** for:

*   **Flashcard Generation:** Based on selected language, topic, and difficulty.
*   **Writing Topic Suggestions:** AI generates engaging prompts.
*   **Writing Feedback:** AI analyzes grammar, vocabulary, and structure.
*   **Test Question Creation:** Dynamically generates appropriate multiple-choice questions.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---

## 👥 Contributors

*   [Nguyễn Phúc Đại](https://github.com/PhucDaizz)

Contributions are welcome! Please fork the repository and submit a pull request.

---

### ✨ Show Your Support

Give a ⭐ if this project helped you or you find it interesting!

---

**Happy Learning with WordWise!** 🚀
