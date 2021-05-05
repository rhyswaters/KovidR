using System;
using System.Collections.Generic;
using MediatR;

namespace Guess.Application.Features.Results.GetResultsByCumulativeTotals.Queries
{
    public class GetResultsByCumulativeTotalsQuery : IRequest<GetResultsByCumulativeTotalsVm>
    {
        public GetResultsByCumulativeTotalsQuery(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
