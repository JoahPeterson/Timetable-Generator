using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionId = "TimeTableDBConectionString";

    public DbConnection(IConfiguration config)
    {
        _config = config;

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        string? connectionString = config[$"{_connectionId}"];

        Client = new MongoClient(connectionString);
        DbName = config["TimeTableDBName"];
        _db = Client.GetDatabase(DbName);

        TaskTypeCollection = _db.GetCollection<TaskType>(TaskTypeCollectionName);
        UserCollection = _db.GetCollection<User>(UserCollectionName);
        CourseTypeCollection = _db.GetCollection<CourseType>(CourseTypeCollectionName);
        TermCollection = _db.GetCollection<Term>(TermCollectionName);
        TermDurationCollection = _db.GetCollection<TermDuration>(TermDurationCollectionName);
        CourseCollection = _db.GetCollection<Course>(CourseCollectionName);
    }
    public MongoClient Client { get; private set; }
    public string? DbName { get; private set; }

    public string CourseTypeCollectionName { get; private set; } = "courseTypes";

    public IMongoCollection<CourseType> CourseTypeCollection { get; private set; }

    public string CourseCollectionName { get; private set; } = "courses";

    public IMongoCollection<Course> CourseCollection { get; private set; }

    public string TaskTypeCollectionName { get; private set; } = "taskTypes";

    public IMongoCollection<TaskType> TaskTypeCollection { get; private set; }

    public string TermCollectionName { get; private set; } = "terms";

    public IMongoCollection<Term> TermCollection { get; private set; }

    public string TermDurationCollectionName { get; private set; } = "termDurations";

    public IMongoCollection<TermDuration> TermDurationCollection { get; private set; }

    public string UserCollectionName { get; private set; } = "users";

    public IMongoCollection<User> UserCollection { get; private set; }


}
