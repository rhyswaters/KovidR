using System;
using Guess.Domain.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Guess.Infrastructure.Persistence
{
    public class GuessContext : IGuessContext
    {
        public GuessContext(IConfiguration configuration)
        {
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
