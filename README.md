# oop-s2-3-mvc-76266

## Overview

This project is an ASP.NET Core MVC college management system built for the S2-3 OOP assignment. It uses Entity Framework Core with SQLite, ASP.NET Core Identity for authentication, role-based access control, seeded demo data, xUnit tests, and GitHub Actions CI.

The system supports three main roles:

- Admin
- Faculty
- Student

## Main Features

### Admin

- View admin dashboard
- Manage branches
- Manage courses
- Manage enrolments

### Faculty

- View faculty dashboard
- View assignments
- View assignment results
- View attendance
- View exams
- View exam results

### Student

- View student dashboard
- View enrolled courses
- View assignment results
- View released exam results
- View attendance summary

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- ASP.NET Core Identity
- xUnit
- GitHub Actions

## Project Structure

- src/VgcCollege.Web -> main MVC application
- tests/VgcCollege.Tests -> unit tests
- .github/workflows/ci.yml -> CI pipeline

## How to Run Locally

### 1. Restore packages

dotnet restore

### 2. Run the application

dotnet run --project src/VgcCollege.Web --urls "http://localhost:5200"

## Demo Accounts

Admin  
Email: admin@vgc.com  
Password: Admin123!

Faculty  
Email: faculty@vgc.com  
Password: Faculty123!

Student 1  
Email: student1@vgc.com  
Password: Student123!

Student 2  
Email: student2@vgc.com  
Password: Student123!

## Design Decisions

The system was designed using ASP.NET Core MVC with Entity Framework Core and SQLite. Role-based access control was implemented using ASP.NET Core Identity to ensure secure data access for Admin, Faculty, and Student roles. Seed data was used to allow immediate testing without manual setup.

