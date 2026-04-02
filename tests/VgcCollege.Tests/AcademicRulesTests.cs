using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Controllers;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;
using Xunit;

namespace VgcCollege.Tests
{
    public class AcademicRulesTests
    {
        private ApplicationDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private async Task<(ApplicationDbContext context, int assignmentId, int examId, int student1Id, int student2Id)> CreateSeededContextAsync(string databaseName)
        {
            var context = CreateContext(databaseName);

            var branch = new Branch
            {
                Name = "Dublin Branch",
                Address = "10 O'Connell Street, Dublin"
            };

            context.Branches.Add(branch);
            await context.SaveChangesAsync();

            var course = new Course
            {
                Name = "BSc Computing",
                BranchId = branch.Id,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2027, 5, 31)
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var student1 = new StudentProfile
            {
                IdentityUserId = "student-1",
                Name = "John Carter",
                Email = "student1@vgc.ie",
                Phone = "111111111",
                Address = "Dublin",
                StudentNumber = "ST76266A"
            };

            var student2 = new StudentProfile
            {
                IdentityUserId = "student-2",
                Name = "Maria Silva",
                Email = "student2@vgc.ie",
                Phone = "222222222",
                Address = "Dublin",
                StudentNumber = "ST76266B"
            };

            context.StudentProfiles.AddRange(student1, student2);
            await context.SaveChangesAsync();

            var assignment = new Assignment
            {
                CourseId = course.Id,
                Title = "Programming Fundamentals Assignment",
                MaxScore = 100,
                DueDate = new DateTime(2026, 10, 15)
            };

            var exam = new Exam
            {
                CourseId = course.Id,
                Title = "Programming Fundamentals Exam",
                Date = new DateTime(2026, 12, 10),
                MaxScore = 100,
                ResultsReleased = true
            };

            context.Assignments.Add(assignment);
            context.Exams.Add(exam);
            await context.SaveChangesAsync();

            return (context, assignment.Id, exam.Id, student1.Id, student2.Id);
        }

        [Fact]
        public void ExamResult_Grade_Is_A_When_Score_Is_70_Or_More()
        {
            var result = new ExamResult
            {
                Score = 82
            };

            Assert.Equal("A", result.Grade);
        }

        [Fact]
        public void ExamResult_Grade_Is_B_When_Score_Is_Between_60_And_69()
        {
            var result = new ExamResult
            {
                Score = 65
            };

            Assert.Equal("B", result.Grade);
        }

        [Fact]
        public void ExamResult_Grade_Is_C_When_Score_Is_Between_50_And_59()
        {
            var result = new ExamResult
            {
                Score = 54
            };

            Assert.Equal("C", result.Grade);
        }

        [Fact]
        public void ExamResult_Grade_Is_Fail_When_Score_Is_Below_50()
        {
            var result = new ExamResult
            {
                Score = 41
            };

            Assert.Equal("Fail", result.Grade);
        }

        [Fact]
        public async Task AssignmentResultsController_Create_Redirects_When_Result_Is_New()
        {
            var (context, assignmentId, _, student1Id, _) = await CreateSeededContextAsync(Guid.NewGuid().ToString());
            var controller = new AssignmentResultsController(context);

            var result = new AssignmentResult
            {
                AssignmentId = assignmentId,
                StudentProfileId = student1Id,
                Score = 77,
                Feedback = "Good work"
            };

            var actionResult = await controller.Create(result);

            var redirect = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(context.AssignmentResults);
        }

        [Fact]
        public async Task AssignmentResultsController_Create_Returns_View_When_Duplicate_Result_Is_Added()
        {
            var (context, assignmentId, _, student1Id, _) = await CreateSeededContextAsync(Guid.NewGuid().ToString());

            context.AssignmentResults.Add(new AssignmentResult
            {
                AssignmentId = assignmentId,
                StudentProfileId = student1Id,
                Score = 80,
                Feedback = "Existing result"
            });

            await context.SaveChangesAsync();

            var controller = new AssignmentResultsController(context);

            var duplicate = new AssignmentResult
            {
                AssignmentId = assignmentId,
                StudentProfileId = student1Id,
                Score = 81,
                Feedback = "Duplicate result"
            };

            var actionResult = await controller.Create(duplicate);

            var view = Assert.IsType<ViewResult>(actionResult);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(1, context.AssignmentResults.Count());
            Assert.IsType<AssignmentResult>(view.Model);
        }

        [Fact]
        public async Task ExamResultsController_Create_Redirects_When_Result_Is_New()
        {
            var (context, _, examId, student1Id, _) = await CreateSeededContextAsync(Guid.NewGuid().ToString());
            var controller = new ExamResultsController(context);

            var result = new ExamResult
            {
                ExamId = examId,
                StudentProfileId = student1Id,
                Score = 68,
                Grade = "B"
            };

            var actionResult = await controller.Create(result);

            var redirect = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(context.ExamResults);
        }

        [Fact]
        public async Task ExamResultsController_Create_Returns_View_When_Duplicate_Result_Is_Added()
        {
            var (context, _, examId, student1Id, _) = await CreateSeededContextAsync(Guid.NewGuid().ToString());

            context.ExamResults.Add(new ExamResult
            {
                ExamId = examId,
                StudentProfileId = student1Id,
                Score = 72,
                Grade = "A"
            });

            await context.SaveChangesAsync();

            var controller = new ExamResultsController(context);

            var duplicate = new ExamResult
            {
                ExamId = examId,
                StudentProfileId = student1Id,
                Score = 75,
                Grade = "A"
            };

            var actionResult = await controller.Create(duplicate);

            var view = Assert.IsType<ViewResult>(actionResult);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(1, context.ExamResults.Count());
            Assert.IsType<ExamResult>(view.Model);
        }
    }
}
