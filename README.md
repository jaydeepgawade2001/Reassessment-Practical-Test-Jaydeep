# Meeting Room Booking API

This is a **Reassessment Practical Test submission** by **Jaydeep Gawade**.
A RESTful API for managing meeting rooms and bookings, built with **.NET 8** using **Onion Architecture**.

---

## 🚀 Setup Instructions

### Prerequisites
1.  **Framework**: .NET 8 SDK
2.  **Database**: SQL Server (LocalDB or Standard)
3.  **IDE**: Visual Studio 2022 / VS Code

### Steps to Run
1.  **Clone the Repository**
    ```bash
    git clone https://github.com/jaydeepgawade2001/Reassessment-Practical-Test-Jaydeep.git
    cd Reassessment-Practical-Test-Jaydeep
    ```

2.  **Configure Database**
    *   The app uses `(localdb)\mssqllocaldb`.
    *   The database `ReassessmentApp_Jaydeeep_V2` will be **automatically created** on first run (EF Core Auto-migration).

3.  **Run the Application**
    ```bash
    cd Reassessment
    dotnet run --project ReassessmentApp.API/ReassessmentApp.API.csproj
    ```

4.  **Access Swagger UI**
    Open your browser to: `http://localhost:5200/swagger`

---

## 📌 Assumptions Made
1.  **Database**: Used `(localdb)\mssqllocaldb` to ensure ease of setup without requiring a full SQL Server instance.
2.  **Port**: The API runs on `http://localhost:5200` by default.
3.  **Conflict Logic**: Use strict non-overlapping time slots (Start Time < End Time).
4.  **Validation**: Room names must be unique. Bookings cannot be in the past.
5.  **Concurrency**: Booking conflicts are handled using Entity Framework Queries.

---

## 📡 API Endpoint List

### 🏠 Rooms
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Rooms` | Get all meeting rooms |
| `GET` | `/api/Rooms/{id}` | Get details of a specific room |
| `POST` | `/api/Rooms` | Create a new meeting room |
| `DELETE` | `/api/Rooms/{id}` | Delete a room (if no bookings exist) |

### 📅 Bookings
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Bookings` | Get all bookings (System-wide) |
| `GET` | `/api/Bookings/{id}` | Get details of a specific booking |
| `GET` | `/api/Bookings/room/{roomId}` | Get all bookings for a specific room |
| `POST` | `/api/Bookings` | Create a new booking (with conflict check) |
| `DELETE` | `/api/Bookings/{id}` | Cancel/Delete a booking |

---

## ✅ Deliverables Checklist
- [x] **Working Web API Project** (Onion Architecture, SOLID Principles)
- [x] **Database Schema** (EF Core Code-First with Automatic Migrations)
- [x] **Endpoints Tested** (Verified via Swagger & Unit Tests)
- [x] **README** (Setup Docs, Assumptions, Endpoint List)
