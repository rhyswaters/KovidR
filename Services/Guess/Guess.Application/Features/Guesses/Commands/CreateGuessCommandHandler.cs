using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Guess.Application.Contracts;
using Guess.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Guess.Application.Features.Guesses.Commands
{
    public class CreateGuessCommandHandler : IRequestHandler<CreateGuessCommand>
    {
        private readonly IGuessRepository _guessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateGuessCommandHandler> _logger;

        public CreateGuessCommandHandler(IGuessRepository guessRepository, IMapper mapper, ILogger<CreateGuessCommandHandler> logger)
        {
            _guessRepository = guessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateGuessCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<UserGuess>(request);
            await _guessRepository.CreateGuess(entity);

            _logger.LogInformation($"Guess for {request.UserName} on {request.GuessDate} successfully created.");

            return Unit.Value;
        }
    }
}
