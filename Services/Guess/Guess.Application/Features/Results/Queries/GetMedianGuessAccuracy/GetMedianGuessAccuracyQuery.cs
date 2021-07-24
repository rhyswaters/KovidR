using System;
using System.Collections.Generic;
using MediatR;

namespace Guess.Application.Features.Results.GetMedianGuessAccuracy.Queries
{
    public class GetMedianGuessAccuracyQuery : IRequest<MedianGuessAccuracyVm>
    {
        public GetMedianGuessAccuracyQuery(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
