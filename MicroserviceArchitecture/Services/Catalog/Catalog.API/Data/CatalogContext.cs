using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Microservices.Common;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>(Constants.GetMongoConnectionString));
            var database = client.GetDatabase(configuration.GetValue<string>(Constants.GetMongoDatabaseName));

            Products = database.GetCollection<Product>(configuration.GetValue<string>(Constants.GetMongoCollectionName));
            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}
