🚗 Car Rental API – ASP.NET Core
A RESTful API for managing a Car Rental System, developed using ASP.NET Core, ADO.NET, Dapper, and LINQ.  
The system follows a 3-layer architecture (API/Presentation, Business Logic, Data Access) to ensure separation of concerns, maintainability, and scalability.
🛠️ Technologies & Skills
Category	Details
Framework	ASP.NET Core Web API
Languages	C#
Data Access	ADO.NET, Dapper, LINQ
Architecture	3-layer Architecture (API, BLL, DAL), Clean Coding Principles
Features	CRUD operations, Authentication, Reporting, File Uploads
📋 Main Endpoints
Cart
`GET /api/Cart/{userId}/Items` – Get cart items for a user
`POST /api/Cart` – Add item to cart
`PUT /api/Cart` – Update cart
`DELETE /api/Cart` – Clear cart
Categories
`GET /api/Categories/All` – Get all categories
`GET /api/Categories/{categoryId}/Vehicles` – Get vehicles by category
`GET /api/Categories/{id}` – Get category by id
`POST /api/Categories` – Create new category
`PUT /api/Categories/{id}` – Update category
`DELETE /api/Categories/{id}` – Delete category
`POST /api/Categories/Upload-Category-Image` – Upload category image
Fuel Type
`GET /api/FuelTypeAPI/All` – Get all fuel types
`GET /api/FuelTypeAPI/{id}` – Get fuel type by id
`POST /api/FuelTypeAPI` – Create new fuel type
`PUT /api/FuelTypeAPI/{id}` – Update fuel type
`DELETE /api/FuelTypeAPI/{id}` – Delete fuel type
User & Authentication
`POST /api/User/reg` – Register user
`POST /api/User/auth/login` – Login
`PUT /api/User/auth/Reset-Password` – Reset password
`GET /api/User/All` – Get all users
`GET /api/User/{id}` – Get user by id
`PUT /api/User/{id}` – Update user
`DELETE /api/User/{id}` – Delete user
`POST /api/User/Upload-User-Image` – Upload user image
Vehicle
`GET /api/Vehicle/All` – Get all vehicles
`GET /api/Vehicle/{id}` – Get vehicle by id
`GET /api/Vehicle/Count` – Count of vehicles
`POST /api/Vehicle` – Create vehicle
`PUT /api/Vehicle/{id}` – Update vehicle
`DELETE /api/Vehicle/{id}` – Delete vehicle
`POST /api/Vehicle/Upload-Vehicle-Image` – Upload vehicle image
Reporting
`GET /api/Reporting/Counts` – Get summary counts for dashboard
📸 Screenshots
![Screenshot 1](https://github.com/user-attachments/assets/81ef8abf-350c-4ad7-a3e5-167180f03c6c)
![Screenshot 2](https://github.com/user-attachments/assets/0a2da969-e89e-4e8a-9876-ec9a0c248196)
![Screenshot 3](https://github.com/user-attachments/assets/da3c2c8b-402a-4820-8e81-416b8a4a1afe)
📝 Notes
Uses DTOs for requests and responses: `CartDTO`, `UserInfoDTO`, `VehicleReadDTO`, etc.
Implements authentication and password reset workflows.
Supports file uploads for vehicle and category images.
Built with 3-layer architecture to separate API layer, business logic, and data access for maintainability and scalability.
