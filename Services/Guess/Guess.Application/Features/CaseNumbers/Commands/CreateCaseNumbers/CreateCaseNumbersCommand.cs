using System;
using MediatR;

namespace Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers
{
    public class CreateCaseNumbersCommand : IRequest
    {
        public int TotalCases { get; set; }
        public DateTime Date { get; set; }
    }
}
