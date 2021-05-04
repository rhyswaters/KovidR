using System;
namespace Guess.Domain.Entities
{
    public class UserGuess
    {
        public string UserName { get; set; }
        public int TotalCases { get; set; }
        public DateTime GuessDate { get; set; }
    }
}
