## 📂 HR Management System – ASP.NET Core Web API

A complete Human Resource Management System built using ASP.NET Core Web API. It provides full control over employees, departments, salaries, attendance, leave requests, documents, projects, performance reviews, and real-time notifications.

---

### 🔧 Technologies Used

* ASP.NET Core 8 Web API
* Entity Framework Core
* SQL Server
* SignalR (for real-time notifications)
* AutoMapper
* JWT Authentication
* Repository & Unit of Work Pattern
* Clean Architecture

---

### 🧩 Features Implemented

#### 👨‍💼 Employee Management

* Full CRUD operations.
* Only HR/Admin can edit employee info.
* Each employee is linked to an `ApplicationUser`.
* DTOs used for create/update/display.

---

#### 🏢 Department Management

* Full CRUD for departments.
* Soft Delete supported for Admins.
* Uses `DepartmentFormDto` for input/output.

---

#### 💰 Salary Management

* Generate monthly salary including bonuses and deductions.
* Track "Paid" vs. "Unpaid" status.
* View salary details per employee and per department.
* Retrieve full list of salaries.

---

#### ➕ Salary Adjustments

* Add bonus or deduction linked to employee.
* Filter by month and year.
* Admin/HR can delete any adjustment.

---

#### 🕒 Attendance Tracking

* Check-In and Check-Out.
* Prevent multiple attendance records on the same day.
* Automatically classify the attendance status (on time, late, etc.).

---

#### 📆 Leave Requests

* Employees can send leave requests.
* Requests are saved with `Pending` status.
* Use Enum to define status: Pending, Approved, Rejected.
* DTOs used for creating and listing requests.

---

#### 📁 Employee Documents

* Upload documents for each employee (ID, contract...).
* Save file name, path, and description.
* View all documents linked to an employee.
* Uses `IFormFile` for uploads.

---

#### 🔔 Notifications (SignalR)

* Create and send notifications to employees.
* Supports real-time delivery using **SignalR**.
* Fetch notifications for current user using JWT.
* Mark notifications as read.
* Optionally delete a notification.

---

#### 🧪 Performance Reviews

* HR/Admin can add performance reviews per employee.
* Each review includes a numeric score and a comment.
* View all reviews for a specific employee.

---

#### 🏗️ Projects & Employee Assignments

* Create new projects.
* Assign multiple employees to a project.
* View all projects assigned to an employee.
* View all employees assigned to a specific project.

---

### 🛡️ Role-Based Authorization

* Roles used: `Admin`, `HR`, `Employee`.
* Endpoints are secured with `[Authorize(Roles = "...")]`.

---

### ⚙️ Clean Code Practices

* DTOs for requests and responses.
* AutoMapper for model mapping.
* Repository + Unit of Work for data access abstraction.
* Domain entities separated from the presentation layer.

---

### 🚀 Getting Started

1. Clone the repository:
   git clone https://github.com/sohaibessa797/HRManagementSystem.git

2. Update your `appsettings.json` with your database connection string.
3. Apply EF Core migrations:
   dotnet ef database update

4. Run the project:
   dotnet run

5. Use Postman or Swagger to test the endpoints.

6. Connect to `/notificationHub` to test SignalR notifications.

---



### 📌 Final Notes

* No frontend included – backend API only.
* You can integrate this API with any client (Web, Mobile, etc.).
* Recommended to secure and host it in production using HTTPS and proper user role policies.

---
