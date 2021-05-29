using System;
using System.Collections.Generic;
using Guess.Application.Models;

namespace Guess.Infrastructure.Messaging
{
    public class SendSignalMessageRequest
    {
        public string message { get; set; }
        public string number { get; set; }
        public List<string> recipients { get; set; }

        public SendSignalMessageRequest(Message m, string sendNumber)
        {
            number = sendNumber;
            message = m.MessageText;
            recipients = m.Recipients;
        }
    }
}
