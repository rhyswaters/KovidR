using System;
using System.Collections.Generic;
using MediatR;

namespace Guess.Application.Features.Results.GetResultsByDaysWon.Queries
{
    public class GetResultsByDaysWonQuery : IRequest<ResultsByDaysWonVm>
    {
        public GetResultsByDaysWonQuery(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
