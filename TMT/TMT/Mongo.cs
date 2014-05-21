namespace TMT
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GridFS;
    using MongoDB.Driver.Linq;

    /// <summary>
    /// Singleton class
    /// </summary>
    public class Mongo
    {
        private string _connectionString;
        private MongoClient _mongoClient;
        private MongoServer _mongoServer;
        private string _databaseName;
        private MongoDatabase _database;

        private static Mongo _instance;

        private Mongo() { } // Locking the constructor

        /// <summary>
        /// Returns the _instance of the Mongo
        /// </summary>
        public static Mongo Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Mongo();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets and Sets the _connectionString
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
                _mongoClient = new MongoClient(_connectionString);
                _mongoServer = _mongoClient.GetServer();
            }
        }

        /// <summary>
        /// Gets the _mongoClient
        /// </summary>
        public MongoClient MongoClient
        {
            get
            {
                return _mongoClient;
            }
        }

        /// <summary>
        /// Gets the _mongoServer
        /// </summary>
        public MongoServer MongoServer
        {
            get
            {
                return _mongoServer;
            }
        }

        /// <summary>
        /// Gets and Sets the _databaseName
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
                _database = _mongoServer.GetDatabase(_databaseName);
            }
        }

        /// <summary>
        /// Gets the _database
        /// </summary>
        public MongoDatabase Database
        {
            get
            {
                return _database;
            }
        }

    }
}
