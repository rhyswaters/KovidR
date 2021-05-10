using System;
using System.Collections.Generic;
using MediatR;

namespace Guess.Application.Features.Results.Queries.GetResultsList
{
    public class GetResultsListQuery : IRequest<IEnumerable<ResultsListVm>>
    {
        public GetResultsListQuery(int numDays = 10)
        {
            NumberOfDaysToFetch = numDays;
        }

        public int NumberOfDaysToFetch { get; set; }
    }
}
