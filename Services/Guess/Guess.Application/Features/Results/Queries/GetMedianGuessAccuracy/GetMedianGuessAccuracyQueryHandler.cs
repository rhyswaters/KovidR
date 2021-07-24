using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using MediatR;
using MathNet.Numerics.Statistics;

namespace Guess.Application.Features.Results.GetMedianGuessAccuracy.Queries
{
    public class GetMedianGuessAccuracyQueryHandler : IRequestHandler<GetMedianGuessAccuracyQuery, MedianGuessAccuracyVm>
    {
        private readonly IGuessRepository _guessRepository;
        private readonly IMapper _mapper;

        public GetMedianGuessAccuracyQueryHandler(IGuessRepository guessRepository, IMapper mapper)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
        }

        public async Task<MedianGuessAccuracyVm> Handle(GetMedianGuessAccuracyQuery request, CancellationToken cancellationToken)
        {
            var guessesList = await _guessRepository.GetGuesses(request.From, request.To);
            var caseNumbersList = await _guessRepository.GetCaseNumbers(request.From, request.To);
            var accuracyDictionary = new Dictionary<string, List<double>>();
            var userNameList = guessesList.Select(x => x.UserName).Distinct();

            foreach(var userName in userNameList)
            {
                accuracyDictionary.Add(userName, new List<double>());
            }

            foreach(var entry in caseNumbersList)
            {
                var guessesForDay = guessesList.Where(g => g.GuessDate == entry.Date);

                foreach(var guess in guessesForDay)
                {
                    accuracyDictionary[guess.UserName].Add(GetGuessAccuracy(guess.TotalCases, entry.TotalCases));
                }
            }

            var vm = new MedianGuessAccuracyVm();

            foreach (var entry in accuracyDictionary)
            {
                var median = Math.Round(entry.Value.Median(), 1);
                vm.MedianAccuracy.Add(entry.Key, median);
            }

            return vm;
        }

        private double GetGuessAccuracy(int guess, int caseNumbers)
        {
            double accuracy = (double)guess / (double)caseNumbers;

            if (accuracy > 1)
                accuracy = 2 - accuracy;

            accuracy = Math.Round(accuracy * 100, 2);

            return accuracy;
        }
    }
}
