using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface IDbConnection
{
    MongoClient Client { get; }
    string DbName { get; }

    IMongoCollection<CourseType> CourseTypeCollection { get; }

    string CourseTypeCollectionName { get; }

    IMongoCollection<Course> CourseCollection { get; }

    string CourseCollectionName { get; }

    IMongoCollection<TaskType> TaskTypeCollection { get; }

    string TaskTypeCollectionName { get; }

    IMongoCollection<Term> TermCollection { get; }

    string TermCollectionName { get; }

    IMongoCollection<TermDuration> TermDurationCollection { get; }

    string TermDurationCollectionName { get; }

    IMongoCollection<User> UserCollection { get; }

    string UserCollectionName { get; }
}
