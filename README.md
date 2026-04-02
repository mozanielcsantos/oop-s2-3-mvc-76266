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
- `src/VgcCollege.Web` → main MVC application
- `tests/VgcCollege.Tests` → unit tests
- `.github/workflows/ci.yml` → CI pipeline

## How to Run Locally

### 1. Restore packages
```bash
dotnet restore

