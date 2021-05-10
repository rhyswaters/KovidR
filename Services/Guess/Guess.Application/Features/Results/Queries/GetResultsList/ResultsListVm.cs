using System;
using System.Collections.Generic;

namespace Guess.Application.Features.Results.Queries.GetResultsList
{
    public class ResultsListVm
    {
        public DateTime Date { get; set; }
        public int CaseNumbers { get; set; }
        public string Winner { get; set; }
        public int? WinningGuess { get; set; }
        public Dictionary<string, int> Guesses { get; set; }
    }
}
