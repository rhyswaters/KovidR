using System;
using System.Collections.Generic;

namespace Guess.Application.Models
{
    public class Message
    {
        public List<string> Recipients { get; set; }
        public string MessageText { get; set; }
    }
}
