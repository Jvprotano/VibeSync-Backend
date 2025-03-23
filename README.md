# VibeSync

## 📌 About the Project
**VibeSync** is a collaborative platform for suggesting and managing music at events and parties. An admin can create a *Space* and share a link, allowing guests to suggest songs without needing to create an account.

## 🚀 Features
### 🎵 Space Management
- Create a **Space** with a name and a 7-day validity.
- Generate an **Admin Token** (for management) and a **Public Token** (for participation).
- Link a Space to an authenticated user.

### 🔎 Music Search and Suggestions
- Search for songs via the **YouTube API**.
- Suggest songs in a Space without requiring login.
- Prevent duplicate suggestions to avoid spam.

### 🔐 Authentication and User Control
- Implemented **ASP.NET Identity**.
- *Freemium* model: Space creation requires only an email.
- Option to upgrade to a full account later.

### 📨 Integrations
- **YouTube API** for music search.
- **Email Service** for notifications and confirmations (planned).

## 🛠 Tech Stack
- **Backend:** ASP.NET Core (.NET 8)
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Infrastructure:** Docker (planned for cloud deployment)
- **Frontend:** Angular (separate project)
- **Integrations:** YouTube API

## 📦 Project Structure
```
VibeSync
│── VibeSync.Application    # Use cases and application logic
│── VibeSync.Domain         # Domain models
│── VibeSync.Infrastructure # Repositories, database, and configurations
│── VibeSync.API            # API Endpoints
```

## 🚀 How to Run
### 📌 Prerequisites
- .NET 8 SDK
- SQL Server
- Docker (if running in containers)

### 📦 Backend
```sh
cd VibeSync.API
 dotnet run
```
The API will be available at `http://localhost:5000`

## 📄 License
This project is licensed under the MIT License. Feel free to contribute!

---

🔥 **VibeSync - The right way to collaborate on music!**

