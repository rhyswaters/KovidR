using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guess.Application.Contracts;
using Guess.Domain.Entities;
using Guess.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Guess.Infrastructure.Repositories
{
    public class GuessRepository : IGuessRepository
    {
        private readonly IGuessContext _context;

        public GuessRepository(IGuessContext context)
        {
            _context = context;
        }

        public async Task CreateCaseNumbers(CaseNumbers caseNumbers)
        {
            await _context.CaseNumbers.InsertOneAsync(caseNumbers);
        }

        public async Task CreateGuess(UserGuess guess)
        {
            await _context.Guesses.InsertOneAsync(guess);
        }

        public async Task<IEnumerable<CaseNumbers>> GetCaseNumbers(DateTime? from, DateTime? to)
        {
            return await _context
                            .CaseNumbers
                            .Find(p => true)
                            //.Find(p => p.Date > from && p.Date < to)
                            .ToListAsync();
        }

        public async Task<IEnumerable<UserGuess>> GetGuesses(DateTime? from, DateTime? to)
        {
            return await _context
                            .Guesses
                            .Find(p => true)
                            //.Find(p => p.GuessDate > from && p.GuessDate < to)
                            .ToListAsync();
        }
    }
}
