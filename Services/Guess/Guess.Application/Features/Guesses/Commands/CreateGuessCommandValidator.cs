using System;
using FluentValidation;

namespace Guess.Application.Features.Guesses.Commands
{
    public class CreateGuessCommandValidator : AbstractValidator<CreateGuessCommand>
    {
        public CreateGuessCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage("{UserName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");

            RuleFor(p => p.GuessDate)
                .NotEmpty().WithMessage("{GuessDate} is required.")
                .NotNull();

            RuleFor(p => p.TotalCases)
                .NotEmpty().WithMessage("{TotalCases} is required.")
                .NotNull()
                .GreaterThanOrEqualTo(0).WithMessage("{TotalCases} cannot be negative.");

        }
    }
}
