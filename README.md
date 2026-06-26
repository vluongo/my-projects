A repo where I put personal projects to help me get reacquainted with the latest .NET technologíes. Currently working on Choirs API.


# 🎶 Choirs API

A modern ASP.NET Core Web API for managing choirs, directors, chorists, and user roles — built with Entity Framework Core, JWT authentication, and ready for Azure deployment.

---

## 📖 Overview
The Choirs API provides endpoints to manage choir groups, directors, and chorists (upcoming) and handle user authentication/authorization.  
It’s designed as a practice project to refresh modern .NET backend development skills and demonstrate cloud‑ready architecture.

---

## ✨ Features
- Choir management (create, update, list choirs)
- Director assignment and retrieval
- User roles with JWT-based authentication
- Custom validation pipeline with filters
- ProblemDetails middleware for structured error responses
- Azure-ready deployment configuration
- ScalarUI

## 🚀 Getting Started

**Prerequisites**
- .NET 8 SDK
- SQL Server or Azure SQL Database

**Installation**
Clone the repository:
```bash
git clone https://github.com/vluongo/my-projects.git
```
Run migrations:
```bash
dotnet ef database update
```
Start the API:
```bash
dotnet run
```