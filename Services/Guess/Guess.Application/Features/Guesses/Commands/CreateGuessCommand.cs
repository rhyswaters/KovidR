using System;
using MediatR;

namespace Guess.Application.Features.Guesses.Commands
{
    public class CreateGuessCommand : IRequest
    {
        public string UserName { get; set; }
        public int TotalCases { get; set; }
        public DateTime GuessDate { get; set; }
    }
}
