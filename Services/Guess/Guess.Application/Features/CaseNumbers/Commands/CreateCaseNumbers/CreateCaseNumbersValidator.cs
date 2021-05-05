using System;
using FluentValidation;

namespace Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers
{
    public class CreateCaseNumbersCommandValidator : AbstractValidator<CreateCaseNumbersCommand>
    {
        public CreateCaseNumbersCommandValidator()
        {
            RuleFor(p => p.Date)
                .NotEmpty().WithMessage("{Date} is required.")
                .NotNull();

            RuleFor(p => p.TotalCases)
                .NotEmpty().WithMessage("{TotalCases} is required.")
                .NotNull()
                .GreaterThanOrEqualTo(0).WithMessage("{TotalCases} cannot be negative.");
        }
    }
}
