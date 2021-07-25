using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservices.Common
{
    public class Constants
    {
        public const string GetMongoConnectionString = "DatabaseSettings:ConnectionString";
        public const string GetMongoDatabaseName = "DatabaseSettings:DatabaseName";
        public const string GetMongoCollectionName = "DatabaseSettings:CollectionName";


        public const string GetRedisConnectionString = "CacheSettings:ConnectionString";
    }
}
