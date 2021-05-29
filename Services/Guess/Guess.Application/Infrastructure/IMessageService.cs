using System;
using System.Threading.Tasks;
using Guess.Application.Models;

namespace Guess.Application.Infrastructure
{
    public interface IMessageService
    {
        Task<bool> SendMessage(Message message);
    }
}
