using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GridFS;
    using MongoDB.Driver.Linq;

    public static class MongoDulguun
    {
        public static string connectionString = "mongodb://localhost";
        public static MongoClient mongoClient = new MongoClient(connectionString);
        public static MongoServer mongoServer = mongoClient.GetServer();
        public static string databaseName = "iDictionaryDB";
    }
}
