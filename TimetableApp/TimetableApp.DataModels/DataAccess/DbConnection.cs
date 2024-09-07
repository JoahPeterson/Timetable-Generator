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

        // Retrieve the connection string using the connectionId
        string connectionString = config[$"{_connectionId}"];

        this.Client = new MongoClient(connectionString);
        this.DbName = config["TimeTableDBName"];
        _db = Client.GetDatabase(DbName);

        this.TaskTypeCollection = _db.GetCollection<TaskType>(TaskTypeCollectionName);
    }
    public MongoClient Client { get; private set; }
    public string DbName { get; private set; }

    public string TaskTypeCollectionName { get; private set; } = "taskTypes";

    public IMongoCollection<TaskType> TaskTypeCollection { get; private set; }
}
