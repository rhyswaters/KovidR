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
                var firstGuess = new DateTime(2021, 4, 25).Date.AddHours(12);

                seedDictionary.Add("Rhys",   new int[] { 444, 350, 420, 505, 425, 490, 466, 444, 457, 435, 481, 455,  -1,  -1, 350, 344, 375, 435, 476, 399, 378,  -1, 415, 419, 401, 333,  -1, 411, 332, 404, 351 });
                seedDictionary.Add("Marius", new int[] { 422, 333, 460, 444, 404, 581,  -1, 425, 404, 450, 380, 404,  -1,  -1, 419, 434, 419, 375, 414, 440, 429,  -1, 385, 370, 375, 420, 424, 465, 354, 425, 369 });
                seedDictionary.Add("Remco",  new int[] { 466, 404, 404, 404, 455, 505, 404, 459, 505, 465, 404, 350,  -1,  -1, 404, 404, 404, 390, 444,  -1, 404,  -1, 400, 385, 350, 450,  -1, 480, 400, 450, 384 });
                seedDictionary.Add("Covid",  new int[] { 461, 437, 426, 371, 475, 545, 569, 402, 453, 383, 418, 393, 434, 408, 514, 381, 379, 448, 456, 425, 447, 355, 360, 358, 503, 469, 524, 381, 438, 345 });

                guessesCollection.InsertManyAsync(GetExistingGuesses(seedDictionary, firstGuess));
                caseNumbersCollection.InsertManyAsync(GetExistingCaseNumbers(seedDictionary, firstGuess)); 
            }
        }

        private static IEnumerable<UserGuess> GetExistingGuesses(Dictionary<string, int[]> seedDictionary, DateTime firstGuessDate)
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
                            GuessDate = firstGuessDate.AddDays(i)
                        });
                    }

                    i++;
                }
            }

            return guesses;
        }

        private static IEnumerable<CaseNumbers> GetExistingCaseNumbers(Dictionary<string, int[]> seedDictionary, DateTime firstGuessDate)
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
                            Date = firstGuessDate.AddDays(i)
                        });
                    }

                    i++;
                }
            }

            return caseNumbers;
        }
    }
}
