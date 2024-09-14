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

    IMongoCollection<TaskType> TaskTypeCollection { get; }

    string TaskTypeCollectionName { get; }

    IMongoCollection<User> UserCollection { get; }

    string UserCollectionName { get; }
}
