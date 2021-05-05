using System;
using System.Collections.Generic;
using System.Linq;
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
                var seedDictionary = new Dictionary<string, int[]>();
                var latestGuess = DateTime.Parse("05/05/2021").Date;

                seedDictionary.Add("Rhys", new int[] { 481, 435, 457, 444, 466, 490, 425, 505, 420, 350, 444 });
                seedDictionary.Add("Marius", new int[] { -1, 450, 404, 425, -1, 581, 404, 444, 460, 333, 422 });
                seedDictionary.Add("Remco", new int[] { 404, 465, 505, 459, 404, 505, 455, 404, 404, 404, 466 });
                seedDictionary.Add("Covid", new int[] { -1, 383, 453, 402, 569, 545, 475, 371, 426, 437, 461 });

                guessesCollection.InsertManyAsync(GetExistingGuesses(seedDictionary, latestGuess));
                caseNumbersCollection.InsertManyAsync(GetExistingCaseNumbers(seedDictionary, latestGuess));
            }
        }

        private static IEnumerable<UserGuess> GetExistingGuesses(Dictionary<string, int[]> seedDictionary, DateTime latestGuessDate)
        {
            var guesses = new List<UserGuess>();

            foreach (var entry in seedDictionary.Where(x => x.Key != "Covid"))
            {
                int i = 0;

                foreach (var caseNumbers in entry.Value)
                {
                    if (caseNumbers != -1)
                    {
                        guesses.Add(new UserGuess()
                        {
                            UserName = entry.Key,
                            TotalCases = caseNumbers,
                            GuessDate = latestGuessDate.AddDays(i)
                        });
                    }

                    i--;
                }
            }

            return guesses;
        }

        private static IEnumerable<CaseNumbers> GetExistingCaseNumbers(Dictionary<string, int[]> seedDictionary, DateTime latestGuessDate)
        {
            var caseNumbers = new List<CaseNumbers>();

            foreach (var entry in seedDictionary.Where(x => x.Key == "Covid"))
            {
                int i = 0;

                foreach (var num in entry.Value)
                {
                    if (num != -1)
                    {
                        caseNumbers.Add(new CaseNumbers()
                        {
                            TotalCases = num,
                            Date = latestGuessDate.AddDays(i)
                        });
                    }

                    i--;
                }
            }

            return caseNumbers;
        }
    }
}
