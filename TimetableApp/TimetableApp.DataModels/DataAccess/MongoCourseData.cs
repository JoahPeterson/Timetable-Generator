using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess
{
    /// <summary>
    /// Class the handles the Mongo DAL functions for Course
    /// </summary>
    public class MongoCourseData : ICourseData
    {
        private readonly IDbConnection _db;
        private readonly IMongoCollection<Course> _courses;
        private readonly IUserData _userData;

        /// <summary>
        /// Intantiates a new instance of the Mongo Course data class.
        /// </summary>
        /// <param name="db">Instance of a Mongo DB Connection</param>
        public MongoCourseData(IDbConnection db, IUserData userData)
        {
            _db = db;
            _courses = db.CourseCollection;
            _userData = userData;
        }

        /// <summary>
        /// Create a Course in the database and upate the user's Courses list.
        /// Created in a transaction so we can rollback if the user update fails.
        /// </summary>
        /// <param name="course">Course to be added.</param>
        /// <returns>Task</returns>
        public async Task<bool> CreateCourseAsync(Course course)
        {
            var client = _db.Client;

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionsInTransaction = db.GetCollection<Course>(_db.CourseCollectionName);
                await suggestionsInTransaction.InsertOneAsync(session, course);

                var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);
                var user = await _userData.GetByIdAsync(course.AuditInformation.CreatedById);
                user.Courses.Add(course);
                await usersInTransaction.ReplaceOneAsync(session, u => u.Id == user.Id, user);

                await session.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                return false;
                throw;
            }
        }

        /// <summary>
        /// Gets a list of Courses for a user.
        /// </summary>
        /// <param name="createdById">The id value from the user in MongoDB</param>
        /// <returns>List of Courses</returns>
        public async Task<List<Course>> GetUsersCoursesAsync(string createdById)
        {
            try
            {
                var results = await _courses.FindAsync(course => course.AuditInformation.CreatedById == createdById && course.AuditInformation.IsArchived == false);

                return results.ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a list of all Courses in the database, excluding archived Courses.
        /// </summary>
        /// <returns>List of NON archived Courses</returns>
        public async Task<List<Course>> GetCoursesAsync()
        {
            try
            {
                var results = await _courses.FindAsync(course => course.AuditInformation.IsArchived == false);

                return results.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
          
        }

        /// <summary>
        /// Gets a Course by its ID.
        /// </summary>
        /// <param name="id">Id value of the Course.</param>
        /// <returns>Course by Id or null.</returns>
        public async Task<Course?> GetCourseAsync(string id)
        {
            var result = await _courses.FindAsync(course => course.Id == id);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Updates a Course in the database and updates the user's Courses list.
        /// </summary>
        /// <param name="course">Course to be updated</param>
        /// <returns></returns>
        public async Task<bool> UpdateCourseAsync(Course course)
        {
            var client = _db.Client;
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var typesInTransaction = db.GetCollection<Course>(_db.CourseCollectionName);

                await typesInTransaction.ReplaceOneAsync(
                    session,
                    c => c.Id == course.Id,
                    course
                );

                var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);

                // Get the user from the Audit Info
                var userFilter = Builders<User>.Filter.Eq(u => u.Id, course.AuditInformation.CreatedById);
                var user = await usersInTransaction
                    .Find(session, userFilter)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    // Get the index of the Course in the list so we can replace it.
                    var index = user.Courses.FindIndex(utt => utt.Id == course.Id);

                    if (index != -1)
                    {
                        user.Courses[index] = course;
                    }
                    else
                    {
                        // Optional: Handle the case where the Course is not found in the user's list
                        // For example, you might want to add it or log a warning
                    }

                    await usersInTransaction.ReplaceOneAsync(
                        session,
                        u => u.Id == user.Id,
                        user
                    );
                }
                else
                {
                    throw new Exception($"User with ID {course.AuditInformation.CreatedById} not found.");
                }

                await session.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                return false;
                // Log the exception or rethrow it
                throw;
            }
        }
    }
}
