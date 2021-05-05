using System;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Messages.Events;
using Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Guess.API.EventBusConsumer
{
    public class CaseNumbersPublishedConsumer : IConsumer<CaseNumbersPublishedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CaseNumbersPublishedConsumer> _logger;

        public CaseNumbersPublishedConsumer(IMapper mapper, IMediator mediator, ILogger<CaseNumbersPublishedConsumer> logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CaseNumbersPublishedEvent> context)
        {
            var command = _mapper.Map<CreateCaseNumbersCommand>(context.Message);
            await _mediator.Send(command);
            _logger.LogInformation($"CaseNumbersPublishedEvent consumed successfully for case numbers of {context.Message.Date.ToString("dd/MM/yyyy")}");
        }
    }
}
