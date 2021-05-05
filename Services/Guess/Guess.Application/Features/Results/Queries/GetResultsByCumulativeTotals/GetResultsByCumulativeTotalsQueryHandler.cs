using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using MediatR;

namespace Guess.Application.Features.Results.GetResultsByCumulativeTotals.Queries
{
    public class GetResultsByDaysWonQueryHandler : IRequestHandler<GetResultsByCumulativeTotalsQuery, GetResultsByCumulativeTotalsVm>
    {
        private readonly IGuessRepository _guessRepository;
        private readonly IMapper _mapper;

        public GetResultsByDaysWonQueryHandler(IGuessRepository guessRepository, IMapper mapper)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
        }

        public async Task<GetResultsByCumulativeTotalsVm> Handle(GetResultsByCumulativeTotalsQuery request, CancellationToken cancellationToken)
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

                foreach(var guess in guessesForDay)
                {
                    resultsDictionary[guess.UserName] += Math.Abs(entry.TotalCases - guess.TotalCases);
                }
            }

            var vm = new GetResultsByCumulativeTotalsVm();
            vm.OutBy = resultsDictionary;
            return vm;
        }
    }
}
