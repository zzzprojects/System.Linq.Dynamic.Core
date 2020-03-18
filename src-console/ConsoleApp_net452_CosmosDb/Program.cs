using ConsoleAppCosmosDb.Entities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ConsoleAppCosmosDb
{
    class Program
    {
        // Designed for use with the Cosmos DB Emulator
        // https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator
        private static readonly string _DatabaseName = "samples-db";
        private static readonly string _CollectionName = "samples-col";
        private static readonly string _EndpointUrl = "https://localhost:8081";
        private static readonly string _AuthorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static DocumentClient _Client;

        static void Main(string[] args)
        {
            using (_Client = new DocumentClient(new Uri(_EndpointUrl), _AuthorizationKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https }))
            {
                RunDemoAsync(_DatabaseName, _CollectionName).Wait();
            }
        }

        private static async Task RunDemoAsync(string databaseId, string collectionId)
        {
            Database database = await GetOrCreateDatabaseAsync(databaseId);
            DocumentCollection collection = await GetOrCreateCollectionAsync(databaseId, collectionId);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            await PopulateCollectionAsync(collectionUri);

            Console.WriteLine("TESTING ANY()");
            Console.WriteLine();

            string anyReadingsInvalid = @"Data.Readings.Any( x => x.Status == @0 )";
            object[] anyReadingsInvalidParams = new[] { @"invalid" };

            QueryCollection(collectionUri, ParsingConfig.Default, nameof(ParsingConfig.Default), anyReadingsInvalid, anyReadingsInvalidParams, @"FAIL");

            QueryCollection(collectionUri, ParsingConfig.DefaultCosmosDb, nameof(ParsingConfig.DefaultCosmosDb), anyReadingsInvalid, anyReadingsInvalidParams, @"SUCCEED");

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("TESTING SUM()");
            Console.WriteLine();

            string sumReadingsInvalid = @"Data.Readings.Sum( x => x.Weight ) <= @0";
            object[] sumReadingsInvalidParams = new[] { @"5" };

            QueryCollection(collectionUri, ParsingConfig.Default, nameof(ParsingConfig.Default), sumReadingsInvalid, sumReadingsInvalidParams, @"FAIL");

            QueryCollection(collectionUri, ParsingConfig.DefaultCosmosDb, nameof(ParsingConfig.DefaultCosmosDb), sumReadingsInvalid, sumReadingsInvalidParams, @"SUCCEED");

            await DeleteDatabaseAsync();
        }

        private static async Task<Database> GetOrCreateDatabaseAsync(string databaseId)
        {
            return await _Client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
        }

        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(string databaseId, string collectionId)
        {
            DocumentCollection collectionDefinition = new DocumentCollection();
            collectionDefinition.Id = collectionId;
            collectionDefinition.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
            collectionDefinition.PartitionKey.Paths.Add("/Id");

            return await _Client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                collectionDefinition,
                new RequestOptions { OfferThroughput = 400 });
        }

        private static async Task PopulateCollectionAsync(Uri collectionUri)
        {
            var entities = new List<Entity>()
            {
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "valid",
                                Weight = 10,
                            },
                            new EntityReading
                            {
                                Status = "invalid",
                                Weight = 2,
                            },
                        },
                    },
                },
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "valid",
                                Weight = 0,
                                Value = new EntityReadingValue
                                {
                                    Weight = 0,
                                },
                            },
                        },
                    },
                },
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "Valid",
                                Weight = 12,
                            },
                        },
                    },
                },
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "VALID",
                                Weight = 9,
                                Value = new EntityReadingValue
                                {
                                    Weight = 0,
                                },
                            },
                        },
                    },
                },
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "invalid",
                                Weight = 5,
                            },
                        },
                    },
                },
                new Entity
                {
                    Id = Guid.NewGuid(),
                    Data = new EntityData
                    {
                        Readings = new List<EntityReading>
                        {
                            new EntityReading
                            {
                                Status = "invalid",
                                Weight = 0,
                                Value = new EntityReadingValue
                                {
                                    Weight = 3,
                                },
                            },
                            new EntityReading
                            {
                                Status = "invalid",
                                Weight = 8,
                                Value = new EntityReadingValue
                                {
                                    Weight = 8,
                                },
                            },
                        },
                    },
                },
            };

            foreach (var entity in entities)
            {
                await _Client.UpsertDocumentAsync(collectionUri, entity);
            }
        }

        private static void QueryCollection(Uri collectionUri, ParsingConfig config, string configName, string predicate, object[] parameters, string successOrFailure)
        {
            try
            {
                IQueryable<Entity> entities = _Client.CreateDocumentQuery<Entity>(collectionUri);

                IQueryable<Entity> results = entities.Where(config, predicate, parameters);

                string querySqlJson = results.AsDocumentQuery().ToString();
                JObject querySqlJsonObject = JObject.Parse(querySqlJson);
                string querySql = (string)querySqlJsonObject["query"];

                Console.WriteLine($"Config {configName} produces the following SQL:");
                Console.WriteLine();
                Console.WriteLine($"**** {querySql} ****");
                Console.WriteLine();
                Console.WriteLine($"This request will {successOrFailure}");
                Console.WriteLine();

                var querySqlSpec = new SqlQuerySpec(querySql);

                results = _Client.CreateDocumentQuery<Entity>(collectionUri, querySqlSpec, new FeedOptions() { EnableCrossPartitionQuery = true });
                IList<Entity> output = results.ToList();

                Console.WriteLine($"Successfully returned {output.Count} entities");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown: {ex.Message}");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static async Task DeleteDatabaseAsync()
        {
            await _Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_DatabaseName));
        }
    }
}
