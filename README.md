# GymMarket

GymMarket is a comprehensive digital marketplace and management platform for fitness enthusiasts, trainers, and gym owners. The application enables course discovery and registration, real-time messaging, meal/nutritional tracking, and AI-powered body fat percentage estimations.

---

## 📚 Reference Documentation

For a complete and detailed list of all system endpoints, backend API controllers, and frontend single-page application (SPA) routes, see the following reference:

* 👉 **[API and Frontend Routes Reference Guide](file:///workspaces/gym_market/API_AND_ROUTES.md)**

---

## 🛠️ Technology Stack

* **Frontend:** Angular 17, NgRx Signals, Tailwind CSS, SignalR Client
* **Backend API:** C# .NET 8 (ASP.NET Core), EF Core, MS SQL Server, Identity (JWT)
* **ML/AI Service:** FastAPI (Python), TensorFlow, MobileNetV4, OpenCV
* **Storage:** MinIO Object Storage (Dockerized)
* **Payments:** Momo API Integration

---

## 🚀 Quick Start Guide

To run GymMarket locally, follow the instructions in the **[Project Startup Guide](file:///workspaces/gym_market/RUN_GUIDE.md)**:

1. **Infrastructure (MinIO & MSSQL):**
   ```bash
   cd MinIO && docker-compose up -d
   ```
2. **Python ML Server:**
   ```bash
   cd Python_server
   pip install -r requirements.txt
   uvicorn app:app --reload
   ```
3. **Backend API (.NET):**
   ```bash
   cd GymMaket/GymMarket.API
   dotnet ef database update
   dotnet run
   ```
4. **Frontend Client (Angular):**
   ```bash
   cd gym_market_client
   npm install
   npm start
   ```