using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Messages.Events;
using Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers;
using Guess.Application.Features.Results.Queries.GetResultsList;
using Guess.Application.Infrastructure;
using Guess.Application.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Guess.API.EventBusConsumer
{
    public class CaseNumbersPublishedConsumer : IConsumer<CaseNumbersPublishedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CaseNumbersPublishedConsumer> _logger;
        private readonly IMessageService _messageService;
        private readonly IConfiguration _configuration;

        public CaseNumbersPublishedConsumer(IMapper mapper, IMediator mediator, ILogger<CaseNumbersPublishedConsumer> logger, IMessageService messageService, IConfiguration configuration)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _messageService = messageService;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<CaseNumbersPublishedEvent> context)
        {
            var command = _mapper.Map<CreateCaseNumbersCommand>(context.Message);
            await _mediator.Send(command);
            _logger.LogInformation($"CaseNumbersPublishedEvent consumed successfully for case numbers of {context.Message.Date.ToString("dd/MM/yyyy")}");

            //get latest results for messaging service
            var query = new GetResultsListQuery(1);
            var results = await _mediator.Send(query);
            var latestResult = results.FirstOrDefault();

            if (latestResult == null)
                return;

            string guessesForDayText = "";
            foreach (var guess in latestResult.Guesses)
            {
                guessesForDayText += $"{guess.Key}: {guess.Value}\n";
            }

            string messageText = $"KovidR results for {latestResult.Date.ToString("dd/MM/yyyy")}:\n\n{guessesForDayText}\nTotal Cases: {latestResult.CaseNumbers}\nWinner: {latestResult.Winner}";
            var recipients = new List<string>() { "group.ZHdyS2drWmMxMENxeGlaTGZVckg0bXpNL1hFZ3hQVjZEc2ZXNjFMSHkwdz0=" };
            var message = new Message() { Recipients = recipients, MessageText = messageText };
            await _messageService.SendMessage(message);
        }
    }
}
