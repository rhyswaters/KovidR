using System;
namespace Guess.Domain.Entities
{
    public class CaseNumbers : BaseEntity
    {
        public int TotalCases { get; set; }
        public DateTime Date { get; set; }
    }
}
