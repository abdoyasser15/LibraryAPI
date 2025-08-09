# 📚 LibraTrack API

**LibraTrack** is a **Library Management System** built with **ASP.NET Core Web API**.  
It provides features to manage books, users, borrowings, categories, fines, ratings, and notifications.

---

## 🚀 Features
- **User Account Management** (Register, Login, Logout, Update, Delete).
- **Book Management** (Add, Edit, Delete, View, View Top Rated).
- **Borrowing System** with book returns, overdue tracking, and active borrowings.
- **Category Management** (Add, Edit, Delete).
- **Fines Management** with payment support.
- **Notification System** for users.
- **Book Ratings**.
- **User Role Management**.

---

## 📂 API Endpoints

### 🔹 **Account**
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/api/Account/Login` | User login |
| POST   | `/api/Account/Register` | User registration |
| POST   | `/api/Account/Logout` | User logout |
| GET    | `/api/Account` | Get current account details |
| PUT    | `/api/Account` | Update account details |
| DELETE | `/api/Account` | Delete account |
| GET    | `/api/Account/emailExist` | Check if email exists |
| POST   | `/api/Account/RefreshToken` | Refresh access token |
| PUT    | `/api/Account/forgot-password` | Forgot password |
| POST   | `/api/Account/Reset-Password` | Reset password |
| POST   | `/api/Account/Change-Password` | Change password |

---

### 🔹 **Book**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Book` | Get all books |
| POST   | `/api/Book` | Add a new book |
| GET    | `/api/Book/{id}` | Get book by ID |
| PUT    | `/api/Book/{id}` | Update a book |
| DELETE | `/api/Book/{id}` | Delete a book |
| GET    | `/api/Book/TopRatedBooks` | Get top rated books |

---

### 🔹 **Borrowing**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Borrowing` | Get all borrowings |
| DELETE | `/api/Borrowing` | Delete a borrowing |
| POST   | `/api/Borrowing` | Create a new borrowing |
| GET    | `/api/Borrowing/{id}` | Get borrowing by ID |
| PUT    | `/api/Borrowing/{id}` | Update a borrowing |
| GET    | `/api/Borrowing/userBorrowings/{userId}` | Get borrowings by user |
| GET    | `/api/Borrowing/Overdue` | Get overdue borrowings |
| GET    | `/api/Borrowing/Active` | Get active borrowings |
| PUT    | `/api/Borrowing/{id}/Return` | Return a borrowed book |

---

### 🔹 **Category**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Category` | Get all categories |
| POST   | `/api/Category` | Add a new category |
| GET    | `/api/Category/{id}` | Get category by ID |
| PATCH  | `/api/Category/{id}` | Update a category |
| DELETE | `/api/Category/{id}` | Delete a category |

---

### 🔹 **Fine**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Fine` | Get all fines |
| GET    | `/api/Fine/{id}` | Get fine by ID |
| DELETE | `/api/Fine/{id}` | Delete a fine |
| PUT    | `/api/Fine/{id}` | Update a fine |
| GET    | `/api/Fine/borrowing/{id}` | Get fines for a borrowing |
| GET    | `/api/Fine/User/{id}` | Get fines for a user |
| POST   | `/api/Fine/{id}/pay` | Pay a fine |

---

### 🔹 **Notification**
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/api/Notification/sendNotification` | Send a notification |
| GET    | `/api/Notification/UserNotification` | Get user notifications |
| PUT    | `/api/Notification/{id}/MarkAsRead` | Mark notification as read |

---

### 🔹 **Ratings**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Ratings` | Get all ratings |
| GET    | `/api/Ratings/{id}` | Get rating by ID |
| PUT    | `/api/Ratings/{id}` | Update a rating |
| DELETE | `/api/Ratings/{id}` | Delete a rating |
| GET    | `/api/Ratings/book/{bookId}` | Get ratings for a book |

---

### 🔹 **Users**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Users` | Get all users |
| GET    | `/api/Users/{id}` | Get user by ID |
| DELETE | `/api/Users/{id}` | Delete user |
| PUT    | `/api/Users/update-role` | Update user role |

---

## 🛠️ Tech Stack
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Swagger**
- **JWT Authentication**
- **LINQ**
- **Design Patterns**

---

## 📦 How to Run
1. **Clone the repository:**
   ```bash
   git clone https://github.com/abdoyasser15/LibraTrack.API.git
