using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            Expression<Func<CaseNumbers, bool>> predicate = x => true;

            if (from.HasValue && to.HasValue)
                predicate = x => x.Date >= from && x.Date <= to;
            else if (from.HasValue)
                predicate = x => x.Date >= from;
            else if (to.HasValue)
                predicate = x => x.Date <= to;

            return await _context
                            .CaseNumbers
                            .Find(predicate)
                            .ToListAsync();
        }

        public async Task<IEnumerable<UserGuess>> GetGuesses(DateTime? from, DateTime? to)
        {
            Expression<Func<UserGuess, bool>> predicate = x => true;

            if (from.HasValue && to.HasValue)
                predicate = x => x.GuessDate >= from && x.GuessDate <= to;
            else if (from.HasValue)
                predicate = x => x.GuessDate >= from;
            else if (to.HasValue)
                predicate = x => x.GuessDate <= to;

            return await _context
                            .Guesses
                            .Find(predicate)
                            .ToListAsync();
        }
    }
}
