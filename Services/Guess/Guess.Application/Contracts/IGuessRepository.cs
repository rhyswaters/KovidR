using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guess.Domain.Entities;

namespace Guess.Application.Contracts
{
    public interface IGuessRepository
    {
        Task<IEnumerable<UserGuess>> GetGuesses(DateTime? from, DateTime? to);
        Task<IEnumerable<CaseNumbers>> GetCaseNumbers(DateTime? from, DateTime? to);
        Task CreateGuess(UserGuess guess);
        Task CreateCaseNumbers(CaseNumbers caseNumbers);
    }
}
