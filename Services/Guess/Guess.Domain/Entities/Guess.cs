using System;
namespace Guess.Domain.Entities
{
    public class UserGuess : BaseEntity
    {
        public string UserName { get; set; }
        public int TotalCases { get; set; }
        public DateTime GuessDate { get; set; }
    }
}
