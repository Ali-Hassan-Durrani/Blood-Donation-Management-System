# Blood-Donation-Management-System
The Blood Donation Management System is a web-based application designed to streamline and manage the process of blood donation, bridging the gap between donors, seekers, hospitals, and blood banks. Built using Blazor Server and SQL Server, the system ensures secure, efficient, and real-time handling of blood donation data and user interactions.

🔧 Features
👤 Role-Based Registration & Login: Supports multiple user types – Donors, Seekers, Hospitals, and Blood Banks – each with customized signup and dashboard functionality.
🗃️ Centralized User Table: Shared authentication system with linked user IDs across modules for easy management.
🩸 Blood Request Management: Seekers can request blood; hospitals and blood banks can view and respond.
📍 Search by City & Blood Group: Find donors or available blood quickly using smart filters.
📅 Donation History Tracking: Keeps a record of previous donations and requests for transparency.
🛡️ Secure & Scalable Architecture: Built with best practices in Blazor and ADO.NET, ensuring data safety and system scalability.

💻 Tech Stack
1) Frontend: Blazor Server (C#)
2) Backend: ASP.NET Core, ADO.NET
3) Database: SQL Server
4) Tools: Visual Studio, SQL Server Management Studio

📦 Setup Instructions
1) Clone the repository
2) git clone https://github.com/your-username/blood-donation-management-system.git
3) Open the project in Visual Studio.
4) Set up the SQL Server database using the provided SQL scripts in the /DatabaseScripts folder.
5) Update the connection string in appsettings.json.
6) Run the application.

🚀 Future Enhancements
1) Email/SMS notifications for donation requests and confirmations
2) Admin panel for managing users and data
3) Integration with Google Maps for location tracking
4) Advanced analytics dashboard
