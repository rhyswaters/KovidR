using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Messages.Events;
using Guess.Application.Features.Guesses.Queries.HasUserSubmittedNextGuess;
using Guess.Application.Infrastructure;
using Guess.Application.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Guess.API.EventBusConsumer
{
    public class WorkerServiceStartedConsumer : IConsumer<WorkerServiceStartedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CaseNumbersPublishedConsumer> _logger;
        private readonly IMessageService _messageService;
        private readonly IConfiguration _configuration;

        public WorkerServiceStartedConsumer(IMediator mediator, ILogger<CaseNumbersPublishedConsumer> logger, IMessageService messageService, IConfiguration configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _messageService = messageService;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<WorkerServiceStartedEvent> context)
        {
            var userDictionary = _configuration.GetSection("MessagingSettings:Recipients").Get<Dictionary<string, string>>();
            var reminderTextRecipients = new List<string>();

            foreach(var user in userDictionary.Keys)
            {
                var query = new HasUserSubmittedNextGuessQuery(user);
                var results = await _mediator.Send(query);
                if (!results.NextGuessSubmitted)
                    reminderTextRecipients.Add(userDictionary[user]);
            }

            string messageText = $"This is a friendly reminder from KovidR to submit your guess for today.\n\n\n8=====D";
            var message = new Message() { Recipients = reminderTextRecipients, MessageText = messageText };
            await _messageService.SendMessage(message);
        }
    }
}
