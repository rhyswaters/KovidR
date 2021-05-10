using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using MediatR;

namespace Guess.Application.Features.Results.GetResultsByDaysWon.Queries
{
    public class GetResultsByDaysWonQueryHandler : IRequestHandler<GetResultsByDaysWonQuery, ResultsByDaysWonVm>
    {
        private readonly IGuessRepository _guessRepository;
        private readonly IMapper _mapper;

        public GetResultsByDaysWonQueryHandler(IGuessRepository guessRepository, IMapper mapper)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
        }

        public async Task<ResultsByDaysWonVm> Handle(GetResultsByDaysWonQuery request, CancellationToken cancellationToken)
        {
            var guessesList = await _guessRepository.GetGuesses(request.From, request.To);
            var caseNumbersList = await _guessRepository.GetCaseNumbers(request.From, request.To);
            var resultsDictionary = new Dictionary<string, int>();
            var userNameList = guessesList.Select(x => x.UserName).Distinct();

            foreach(var userName in userNameList)
            {
                resultsDictionary.Add(userName, 0);
            }

            foreach(var entry in caseNumbersList)
            {
                var guessesForDay = guessesList.Where(g => g.GuessDate == entry.Date);

                if(!guessesForDay.Any())
                {
                    //we all forgot to guess
                    continue;
                }

                var caseNumbersOfGuesses = guessesForDay.Select(x => x.TotalCases);
                int closestGuess = caseNumbersOfGuesses.Aggregate((x, y) => Math.Abs(x - entry.TotalCases) < Math.Abs(y - entry.TotalCases) ? x : y);

                //possible to have more than one winner if they both guessed the same thing
                var winners = guessesForDay.Where(x => x.TotalCases == closestGuess);

                foreach(var winner in winners)
                {
                    resultsDictionary[winner.UserName]++;
                }
            } 

            var vm = new ResultsByDaysWonVm();
            vm.DaysWon = resultsDictionary;
            return vm;
        }
    }
}
