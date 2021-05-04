using System;
using Guess.Domain.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Guess.Application.Models;

namespace Guess.Infrastructure.Persistence
{
    public class GuessContext : IGuessContext
    {
        public DatabaseSettings _databaseSettings;

        public GuessContext(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            _databaseSettings = databaseSettings.Value;
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Guesses = database.GetCollection<UserGuess>("Guesses");
            CaseNumbers = database.GetCollection<CaseNumbers>("CaseNumbers");
            GuessContextSeed.SeedData(Guesses, CaseNumbers);
        }

        public IMongoCollection<UserGuess> Guesses { get; }
        public IMongoCollection<CaseNumbers> CaseNumbers { get; }
    }
}
