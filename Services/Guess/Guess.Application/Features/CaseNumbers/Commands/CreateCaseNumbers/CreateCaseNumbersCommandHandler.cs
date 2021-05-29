using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Guess.Domain.Entities;
using Guess.Application.Infrastructure;
using Guess.Application.Models;

namespace Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers
{
    public class CreateCaseNumbersCommandHandler : IRequestHandler<CreateCaseNumbersCommand>
    {
        private readonly IGuessRepository _guessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCaseNumbersCommandHandler> _logger;
        private readonly IMessageService _messageService;

        public CreateCaseNumbersCommandHandler(IGuessRepository guessRepository, IMapper mapper, ILogger<CreateCaseNumbersCommandHandler> logger, IMessageService messageService)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
            _logger = logger;
            _messageService = messageService;
        }

        public async Task<Unit> Handle(CreateCaseNumbersCommand request, CancellationToken cancellationToken)
        {
            var newCaseNumbers = _mapper.Map<Guess.Domain.Entities.CaseNumbers>(request);

            var caseNumbers = await _guessRepository.GetCaseNumbers(null, null);

            if(caseNumbers.Where(x => x.Date.Date == request.Date.Date).Any())
            {
                _logger.LogInformation($"Case numbers for {request.Date.ToString("dd/MM/yyyy")} is already stored.");
                return Unit.Value;
            }

            await _guessRepository.CreateCaseNumbers(newCaseNumbers);
            _logger.LogInformation($"Case numbers for {request.Date.ToString("dd/MM/yyyy")} is successfully stored.");

            //send results message
            var dateTo = DateTime.Now.Date.AddHours(13);
            var dateFrom = DateTime.Now.AddDays(-2).Date.AddHours(12);

            var guessesList = await _guessRepository.GetGuesses(dateFrom, dateTo);
            var caseNumbersList = await _guessRepository.GetCaseNumbers(dateFrom, dateTo);

            var latestCaseNumbers = caseNumbersList.OrderByDescending(x => x.Date).FirstOrDefault();
            var guessesForDay = guessesList.Where(x => x.GuessDate.Date == latestCaseNumbers.Date.Date).ToList();

            var resultForDay = CalculateResults(latestCaseNumbers.TotalCases, guessesForDay);
            string guessesForDayText = "";
            foreach(var guess in guessesForDay)
            {
                guessesForDayText += $"{guess.UserName}: {guess.TotalCases}\n";
            }

            string messageText = $"Kovidr results for {latestCaseNumbers.Date.ToString("dd/MM/yyyy")}:\n\n{guessesForDayText}\nTotal Cases: {latestCaseNumbers.TotalCases}\nWinner: {resultForDay.Key}";
            List<string> recipients = new List<string> { "+353861743488", "+353868729411", "+353834574638" };
            var message = new Message() { Recipients = recipients, MessageText = messageText };
            await _messageService.SendMessage(message);

            return Unit.Value;
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
    }
}
