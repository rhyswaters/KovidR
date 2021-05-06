using System;
namespace EventBus.Messages.Events
{
    public class CaseNumbersPublishedEvent : IntegrationBaseEvent
    {
        public CaseNumbersPublishedEvent(int caseNumbers)
        {
            TotalCases = caseNumbers;
            Date = DateTime.Now.Date;
        }

        public int TotalCases { get; set; }
        public DateTime Date { get; set; }
    }
}
