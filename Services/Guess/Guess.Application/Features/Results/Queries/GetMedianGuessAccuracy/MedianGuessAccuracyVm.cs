using System;
using System.Collections.Generic;

namespace Guess.Application.Features.Results.GetMedianGuessAccuracy.Queries
{
    public class MedianGuessAccuracyVm
    {
        public Dictionary<string, double> MedianAccuracy { get; set; }

        public MedianGuessAccuracyVm()
        {
            MedianAccuracy = new Dictionary<string, double>();
        }
    }
}
