# 🩸 Shyrian - Blood Bank Management System

**Shyrian** is a comprehensive web-based platform designed to bridge the gap between blood donors and seekers. Built with a focus on reliability and security, this system ensures that life-saving resources reach those in need efficiently.

---

## 🚀 Technical Stack

*   **Framework:** ASP.NET MVC Core 8.0
*   **Database:** Microsoft SQL Server (SSMS)
*   **ORM:** Entity Framework Core (Code First)
*   **Security:** SHA256 Hashing for Passwords & Cookie-based Authentication
*   **Frontend:** Bootstrap 5, Razor Syntax, and Custom CSS
*   **Environment:** Developed using Windows Subsystem for Linux (WSL) for specific kernel-level tasks

---

## ✨ Key Features

### 👤 User Account & Profile
*   **Identity Verification:** Users must upload medical proof to verify their account.
*   **Smart Profile:** Displays user status (Not Submitted, Pending, Verified, or Rejected).
*   **Data Integrity:** Once a blood type is selected, it is locked and cannot be changed to ensure data reliability.
*   **Comprehensive Dashboards:** Separate tracking for personal blood requests and successful donation history.

### 🩸 Donation Management
*   **Find Blood:** A real-time listing of active blood requests filtered by location and blood type.
*   **Request Creation:** Seamlessly create blood requests with hospital details and quantities.
*   **Fulfillment Tracking:** Requests are marked as "Fulfilled" only after a successful donation from a selected donor.

### 🛡️ Admin Dashboard
*   **Verification Management:** Admins can review, approve, or reject user verification documents.
*   **System Oversight:** Monitor and manage all active blood requests and user activities.

---

## 🛠️ Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/YourUsername/Shyrian_project.git](https://github.com/YourUsername/Shyrian_project.git)

2. **Configure Database:**
Update the ConnectionStrings in appsettings.json with your SQL Server credentials.

3. **Run Migrations:**
Open the Package Manager Console in Visual Studio and execute:
```bash
    Update-Database
```

4. **Launch:**
   Run the project via Visual Studio (IIS Express).

---

## 👥 Team Members

* [Mohamed Mostafa](https://github.com/Mo7amd-hub)
* [Mahmoud Refaat](https://github.com/mhmod-hub)
* [Mohamed Abdel Fattah](https://github.com/MohammedAbdelfattah14)

---

## ⚖️ License
This project is for educational purposes and is available under the MIT License.

---

## ⭐ Support the Project
If you found this project helpful, please consider giving it a **Star**! It helps the project reach more developers and encourages further updates.
