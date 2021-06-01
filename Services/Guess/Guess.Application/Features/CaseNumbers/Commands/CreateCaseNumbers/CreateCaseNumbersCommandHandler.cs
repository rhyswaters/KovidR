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

        public CreateCaseNumbersCommandHandler(IGuessRepository guessRepository, IMapper mapper, ILogger<CreateCaseNumbersCommandHandler> logger)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
            _logger = logger;
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

            return Unit.Value;
        }
    }
}
