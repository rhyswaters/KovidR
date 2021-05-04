using System;
using System.Collections.Generic;
using Guess.Domain.Entities;
using MongoDB.Driver;

namespace Guess.Infrastructure.Persistence
{
    public class GuessContextSeed
    {
        public static void SeedData(IMongoCollection<UserGuess> guessesCollection, IMongoCollection<CaseNumbers> caseNumbersCollection)
        {
            bool existGuess = guessesCollection.Find(p => true).Any();

            if (!existGuess)
            {
                guessesCollection.InsertManyAsync(GetExistingGuesses());
                caseNumbersCollection.InsertManyAsync(GetExistingCaseNumbers());
            }
        }

        private static IEnumerable<UserGuess> GetExistingGuesses()
        {
            return new List<UserGuess>()
            {
                new UserGuess()
                {
                    UserName = "Rhys",
                    TotalCases = 435,
                    GuessDate = DateTime.Parse("04/05/2021")
                },
                new UserGuess()
                {
                    UserName = "Remco",
                    TotalCases = 465,
                    GuessDate = DateTime.Parse("04/05/2021")
                },
                new UserGuess()
                {
                    UserName = "Marius",
                    TotalCases = 450,
                    GuessDate = DateTime.Parse("04/05/2021")
                },
            };
        }

        private static IEnumerable<CaseNumbers> GetExistingCaseNumbers()
        {
            return new List<CaseNumbers>()
            {
                new CaseNumbers()
                {
                    TotalCases = 383,
                    Date = DateTime.Parse("04/05/2021")
                }
            };
        }
    }
}
