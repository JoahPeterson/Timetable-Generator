using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Timetable.XUnitTests
{
    public class CourseRepositoryTests
    {
        private readonly CourseRepository _courseRepo;

        public CourseRepositoryTests()
        {
            _courseRepo = new CourseRepository();
        }

        [Fact]
        public async Task GetCourseAsync_ShouldReturnCourse_WhenCourseExists()
        {
            // Arrange
            var course = new Course { Id = "123", Name = "Test Course" };
            _courseRepo.AddCourse(course);

            // Act
            var result = await _courseRepo.GetCourseAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123", result.Id);
            Assert.Equal("Test Course", result.Name);
        }

        [Fact]
        public async Task GetCourseAsync_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Act
            var result = await _courseRepo.GetCourseAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }
    }

    public class UserCoursesRepositoryTests
    {
        private readonly UserCoursesRepository _userCourseRepo;

        public UserCoursesRepositoryTests()
        {
            _userCourseRepo = new UserCoursesRepository();
        }

        [Fact]
        public async Task GetUsersCoursesAsync_ShouldReturnUsersCourses_WhenCoursesExist()
        {
            // Arrange
            var userId = "user123";
            var courses = new List<Course>
            {
                new Course
                {
                    Id = "course1",
                    Name = "Test Course 1",
                    AuditInformation = new Auditable
                    {
                        CreatedById = userId,
                        IsArchived = false
                    }
                },
                new Course
                {
                    Id = "course2",
                    Name = "Test Course 2",
                    AuditInformation = new Auditable
                    {
                        CreatedById = userId,
                        IsArchived = false
                    }
                },
                new Course
                {
                    Id = "course3",
                    Name = "Other User Course",
                    AuditInformation = new Auditable
                    {
                        CreatedById = "otherUser",
                        IsArchived = false
                    }
                }
            };

            foreach (var course in courses)
            {
                _userCourseRepo.AddCourse(course);
            }

            // Act
            var result = await _userCourseRepo.GetUsersCoursesAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, course => Assert.Equal(userId, course.AuditInformation.CreatedById));
            Assert.All(result, course => Assert.False(course.AuditInformation.IsArchived));
        }

        [Fact]
        public async Task GetUsersCoursesAsync_ShouldNotReturnArchivedCourses()
        {
            // Arrange
            var userId = "user123";
            var courses = new List<Course>
            {
                new Course
                {
                    Id = "course1",
                    Name = "Active Course",
                    AuditInformation = new Auditable
                    {
                        CreatedById = userId,
                        IsArchived = false
                    }
                },
                new Course
                {
                    Id = "course2",
                    Name = "Archived Course",
                    AuditInformation = new Auditable
                    {
                        CreatedById = userId,
                        IsArchived = true
                    }
                }
            };

            foreach (var course in courses)
            {
                _userCourseRepo.AddCourse(course);
            }

            // Act
            var result = await _userCourseRepo.GetUsersCoursesAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Course", result[0].Name);
        }

        [Fact]
        public async Task GetUsersCoursesAsync_ShouldReturnEmptyList_WhenNoCoursesExist()
        {
            // Act
            var result = await _userCourseRepo.GetUsersCoursesAsync("nonexistentUser");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
