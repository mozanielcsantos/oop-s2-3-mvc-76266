using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            string[] roles = { "Administrator", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = await EnsureUserAsync(
                userManager,
                "admin@vgc.ie",
                "Password123!");

            var facultyUser = await EnsureUserAsync(
                userManager,
                "faculty@vgc.ie",
                "Password123!");

            var studentUser1 = await EnsureUserAsync(
                userManager,
                "student1@vgc.ie",
                "Password123!");

            var studentUser2 = await EnsureUserAsync(
                userManager,
                "student2@vgc.ie",
                "Password123!");

            await EnsureRoleAsync(userManager, adminUser, "Administrator");
            await EnsureRoleAsync(userManager, facultyUser, "Faculty");
            await EnsureRoleAsync(userManager, studentUser1, "Student");
            await EnsureRoleAsync(userManager, studentUser2, "Student");

            if (!await context.Branches.AnyAsync())
            {
                var branches = new List<Branch>
                {
                    new Branch
                    {
                        Name = "Dublin Branch",
                        Address = "10 O'Connell Street, Dublin"
                    },
                    new Branch
                    {
                        Name = "Cork Branch",
                        Address = "25 Patrick Street, Cork"
                    },
                    new Branch
                    {
                        Name = "Galway Branch",
                        Address = "8 Eyre Square, Galway"
                    }
                };

                context.Branches.AddRange(branches);
                await context.SaveChangesAsync();
            }

            if (!await context.Courses.AnyAsync())
            {
                var dublinBranch = await context.Branches.FirstAsync(b => b.Name == "Dublin Branch");
                var corkBranch = await context.Branches.FirstAsync(b => b.Name == "Cork Branch");
                var galwayBranch = await context.Branches.FirstAsync(b => b.Name == "Galway Branch");

                var courses = new List<Course>
                {
                    new Course
                    {
                        Name = "BSc Computing",
                        BranchId = dublinBranch.Id,
                        StartDate = new DateTime(2026, 9, 1),
                        EndDate = new DateTime(2027, 5, 31)
                    },
                    new Course
                    {
                        Name = "BA Business Management",
                        BranchId = corkBranch.Id,
                        StartDate = new DateTime(2026, 9, 1),
                        EndDate = new DateTime(2027, 5, 31)
                    },
                    new Course
                    {
                        Name = "Diploma in Data Analytics",
                        BranchId = galwayBranch.Id,
                        StartDate = new DateTime(2026, 9, 1),
                        EndDate = new DateTime(2027, 5, 31)
                    },
                    new Course
                    {
                        Name = "Higher Certificate in Software Development",
                        BranchId = dublinBranch.Id,
                        StartDate = new DateTime(2026, 9, 1),
                        EndDate = new DateTime(2027, 5, 31)
                    }
                };

                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            if (!await context.FacultyProfiles.AnyAsync())
            {
                var facultyProfile = new FacultyProfile
                {
                    IdentityUserId = facultyUser.Id,
                    Name = "Dr Sarah Murphy",
                    Email = "faculty@vgc.ie",
                    Phone = "0851111111"
                };

                context.FacultyProfiles.Add(facultyProfile);
                await context.SaveChangesAsync();
            }

            if (!await context.StudentProfiles.AnyAsync())
            {
                var studentProfiles = new List<StudentProfile>
                {
                    new StudentProfile
                    {
                        IdentityUserId = studentUser1.Id,
                        Name = "John Carter",
                        Email = "student1@vgc.ie",
                        Phone = "0852222222",
                        Address = "1 Main Road, Dublin",
                        StudentNumber = "ST76266A"
                    },
                    new StudentProfile
                    {
                        IdentityUserId = studentUser2.Id,
                        Name = "Maria Silva",
                        Email = "student2@vgc.ie",
                        Phone = "0853333333",
                        Address = "2 River Lane, Cork",
                        StudentNumber = "ST76266B"
                    }
                };

                context.StudentProfiles.AddRange(studentProfiles);
                await context.SaveChangesAsync();
            }

            if (!await context.FacultyCourseAssignments.AnyAsync())
            {
                var facultyProfile = await context.FacultyProfiles.FirstAsync();
                var computingCourse = await context.Courses.FirstAsync(c => c.Name == "BSc Computing");
                var softwareCourse = await context.Courses.FirstAsync(c => c.Name == "Higher Certificate in Software Development");

                var assignments = new List<FacultyCourseAssignment>
                {
                    new FacultyCourseAssignment
                    {
                        FacultyProfileId = facultyProfile.Id,
                        CourseId = computingCourse.Id
                    },
                    new FacultyCourseAssignment
                    {
                        FacultyProfileId = facultyProfile.Id,
                        CourseId = softwareCourse.Id
                    }
                };

                context.FacultyCourseAssignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            if (!await context.CourseEnrolments.AnyAsync())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266A");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266B");
                var computingCourse = await context.Courses.FirstAsync(c => c.Name == "BSc Computing");
                var softwareCourse = await context.Courses.FirstAsync(c => c.Name == "Higher Certificate in Software Development");

                var enrolments = new List<CourseEnrolment>
                {
                    new CourseEnrolment
                    {
                        StudentProfileId = student1.Id,
                        CourseId = computingCourse.Id,
                        EnrolDate = new DateTime(2026, 8, 20),
                        Status = "Active"
                    },
                    new CourseEnrolment
                    {
                        StudentProfileId = student2.Id,
                        CourseId = softwareCourse.Id,
                        EnrolDate = new DateTime(2026, 8, 22),
                        Status = "Active"
                    }
                };

                context.CourseEnrolments.AddRange(enrolments);
                await context.SaveChangesAsync();
            }

            if (!await context.AttendanceRecords.AnyAsync())
            {
                var enrolments = await context.CourseEnrolments.ToListAsync();

                var attendance = new List<AttendanceRecord>
                {
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[0].Id,
                        SessionDate = new DateTime(2026, 9, 7),
                        Present = true
                    },
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[0].Id,
                        SessionDate = new DateTime(2026, 9, 14),
                        Present = true
                    },
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[0].Id,
                        SessionDate = new DateTime(2026, 9, 21),
                        Present = false
                    },
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[1].Id,
                        SessionDate = new DateTime(2026, 9, 7),
                        Present = true
                    },
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[1].Id,
                        SessionDate = new DateTime(2026, 9, 14),
                        Present = false
                    },
                    new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolments[1].Id,
                        SessionDate = new DateTime(2026, 9, 21),
                        Present = true
                    }
                };

                context.AttendanceRecords.AddRange(attendance);
                await context.SaveChangesAsync();
            }

            if (!await context.Assignments.AnyAsync())
            {
                var computingCourse = await context.Courses.FirstAsync(c => c.Name == "BSc Computing");
                var softwareCourse = await context.Courses.FirstAsync(c => c.Name == "Higher Certificate in Software Development");

                var assignments = new List<Assignment>
                {
                    new Assignment
                    {
                        CourseId = computingCourse.Id,
                        Title = "Programming Fundamentals Assignment",
                        MaxScore = 100,
                        DueDate = new DateTime(2026, 10, 15)
                    },
                    new Assignment
                    {
                        CourseId = softwareCourse.Id,
                        Title = "Web Development Coursework",
                        MaxScore = 100,
                        DueDate = new DateTime(2026, 10, 20)
                    }
                };

                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            if (!await context.AssignmentResults.AnyAsync())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266A");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266B");
                var assignment1 = await context.Assignments.FirstAsync(a => a.Title == "Programming Fundamentals Assignment");
                var assignment2 = await context.Assignments.FirstAsync(a => a.Title == "Web Development Coursework");

                var results = new List<AssignmentResult>
                {
                    new AssignmentResult
                    {
                        AssignmentId = assignment1.Id,
                        StudentProfileId = student1.Id,
                        Score = 82,
                        Feedback = "Strong work with clear logic"
                    },
                    new AssignmentResult
                    {
                        AssignmentId = assignment2.Id,
                        StudentProfileId = student2.Id,
                        Score = 76,
                        Feedback = "Good implementation, improve layout consistency"
                    }
                };

                context.AssignmentResults.AddRange(results);
                await context.SaveChangesAsync();
            }

            if (!await context.Exams.AnyAsync())
            {
                var computingCourse = await context.Courses.FirstAsync(c => c.Name == "BSc Computing");
                var softwareCourse = await context.Courses.FirstAsync(c => c.Name == "Higher Certificate in Software Development");

                var exams = new List<Exam>
                {
                    new Exam
                    {
                        CourseId = computingCourse.Id,
                        Title = "Computing End of Semester Exam",
                        Date = new DateTime(2026, 12, 10),
                        MaxScore = 100,
                        ResultsReleased = false
                    },
                    new Exam
                    {
                        CourseId = softwareCourse.Id,
                        Title = "Software Development Final Exam",
                        Date = new DateTime(2026, 12, 12),
                        MaxScore = 100,
                        ResultsReleased = true
                    }
                };

                context.Exams.AddRange(exams);
                await context.SaveChangesAsync();
            }

            if (!await context.ExamResults.AnyAsync())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266A");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.StudentNumber == "ST76266B");
                var exam1 = await context.Exams.FirstAsync(e => e.Title == "Computing End of Semester Exam");
                var exam2 = await context.Exams.FirstAsync(e => e.Title == "Software Development Final Exam");

                var examResults = new List<ExamResult>
                {
                    new ExamResult
                    {
                        ExamId = exam1.Id,
                        StudentProfileId = student1.Id,
                        Score = 68,
                        Grade = "B"
                    },
                    new ExamResult
                    {
                        ExamId = exam2.Id,
                        StudentProfileId = student2.Id,
                        Score = 74,
                        Grade = "B+"
                    }
                };

                context.ExamResults.AddRange(examResults);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<IdentityUser> EnsureUserAsync(
            UserManager<IdentityUser> userManager,
            string email,
            string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return user;
            }

            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {email}: {errors}");
            }

            return user;
        }

        private static async Task EnsureRoleAsync(
            UserManager<IdentityUser> userManager,
            IdentityUser user,
            string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to assign role {role} to {user.Email}: {errors}");
                }
            }
        }
    }
}
