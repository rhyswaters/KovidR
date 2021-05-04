using System;
using Guess.Domain.Entities;
using MongoDB.Driver;

namespace Guess.Infrastructure.Persistence
{
    public interface IGuessContext
    {
        IMongoCollection<UserGuess> Guesses { get; }
        IMongoCollection<CaseNumbers> CaseNumbers { get; }
    }
}
