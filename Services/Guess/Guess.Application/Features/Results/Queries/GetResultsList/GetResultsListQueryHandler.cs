using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using Guess.Domain.Entities;
using MediatR;

namespace Guess.Application.Features.Results.Queries.GetResultsList
{
    public class GetResultsListQueryHandler : IRequestHandler<GetResultsListQuery, IEnumerable<ResultsListVm>>
    {
        private readonly IGuessRepository _guessRepository;

        public GetResultsListQueryHandler(IGuessRepository guessRepository)
        {
            _guessRepository = guessRepository;
        }

        public async Task<IEnumerable<ResultsListVm>> Handle(GetResultsListQuery request, CancellationToken cancellationToken)
        {
            var dateTo = DateTime.Now.Date.AddHours(13);
            var dateFrom = DateTime.Now.AddDays(-request.NumberOfDaysToFetch).Date.AddHours(12);

            var guessesList = await _guessRepository.GetGuesses(dateFrom, dateTo);
            var caseNumbersList = await _guessRepository.GetCaseNumbers(dateFrom, dateTo);
            var resultListVmList = new List<ResultsListVm>();

            //below scenario shoudl only happen in dev
            if (caseNumbersList.Count() == 0)
            {
                caseNumbersList = await _guessRepository.GetCaseNumbers(null, null);
                caseNumbersList = caseNumbersList.OrderByDescending(x => x.Date).Take(request.NumberOfDaysToFetch);
                guessesList = await _guessRepository.GetGuesses(null, null);
            }

            foreach (var result in caseNumbersList)
            {
                var guessesForDay = guessesList.Where(x => x.GuessDate.Date == result.Date.Date).ToList();
                var resultsForDay = CalculateResults(result.TotalCases, guessesForDay);

                resultListVmList.Add(new ResultsListVm()
                {
                    Date = result.Date,
                    CaseNumbers = result.TotalCases,
                    Winner = resultsForDay.Key,
                    WinningGuess = resultsForDay.Value,
                    Guesses = MapGuessesToDictionary(guessesForDay)
                }) ;
            }

            return resultListVmList.OrderByDescending(x=> x.Date).Take(request.NumberOfDaysToFetch);
        }

        private KeyValuePair<string, int?> CalculateResults(int caseNumbers, List<UserGuess> guesses)
        {
            if (!guesses.Any())
            {
                //we all forgot to guess
                return new KeyValuePair<string, int?>("No guesses", null);
            }

            var caseNumbersOfGuesses = guesses.Select(x => x.TotalCases);
            int closestGuess = caseNumbersOfGuesses.Aggregate((x, y) => Math.Abs(x - caseNumbers) < Math.Abs(y - caseNumbers) ? x : y);

            //possible to have more than one winner if they both guessed the same thing
            var winners = guesses.Where(x => x.TotalCases == closestGuess);
            string winnersString = string.Join(',', winners.Select(x => x.UserName));

            return new KeyValuePair<string, int?>(winnersString, closestGuess);
        }

        private Dictionary<string, int> MapGuessesToDictionary(List<UserGuess> guesses)
        {
            var dictionary = new Dictionary<string, int>();
            foreach(var guess in guesses)
            {
                dictionary.Add(guess.UserName, guess.TotalCases);
            }

            return dictionary;
        }
    }
}
