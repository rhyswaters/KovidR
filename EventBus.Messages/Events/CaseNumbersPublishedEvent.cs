using System;
namespace EventBus.Messages.Events
{
    public class CaseNumbersPublishedEvent : IntegrationBaseEvent
    {
        public CaseNumbersPublishedEvent(int caseNumbers)
        {
            totalCases = caseNumbers;
        }

        public int totalCases { get; set; }
    }
}
