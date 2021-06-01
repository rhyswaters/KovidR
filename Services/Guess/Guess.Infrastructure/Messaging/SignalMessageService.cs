using System;
using System.Threading.Tasks;
using Guess.Application.Infrastructure;
using Guess.Application.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Guess.Infrastructure.Messaging
{
    public class SignalMessageService : IMessageService
    {
        private readonly IConfiguration _configuration;
        public ILogger<SignalMessageService> _logger { get; }

        public SignalMessageService(IConfiguration configuration, ILogger<SignalMessageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendMessage(Message message)
        {
            var requestContent = new SendSignalMessageRequest(message, _configuration["MessagingSettings:SendNumber"]);
            var baseUrl = _configuration["MessagingSettings:ConnectionString"];

            using (var client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(requestContent);
                var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/v2/send");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;

                _logger.LogError("Signal message sending failed.");

                return false;
            }
        }
    }
}
