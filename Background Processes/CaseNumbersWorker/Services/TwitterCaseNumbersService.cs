using System;
using System.Net.Http;
using CaseNumbersWorker.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Messages.Events;
using MassTransit;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CaseNumbersWorker.Services
{
    public class TwitterCaseNumbersService : ICaseNumbersService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<CaseNumbersService> _logger;
        private readonly IPublishEndpoint _publishEndPoint;
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _hostApplicationLifeTime;

        public TwitterCaseNumbersService(IHttpClientFactory clientFactory,
                                         ILogger<CaseNumbersService> logger,
                                         IPublishEndpoint publishEndPoint,
                                         IConfiguration configuration,
                                         IHostApplicationLifetime hostApplicationLifeTime)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _publishEndPoint = publishEndPoint;
            _configuration = configuration;
            _hostApplicationLifeTime = hostApplicationLifeTime;
        }

        public async Task PublishCaseNumbers()
        {
            var eventMessage = new WorkerServiceStartedEvent();
            await _publishEndPoint.Publish(eventMessage);

            int count = 1;
            var caseNumbersPublished = false;
            var baseUrl = "https://api.twitter.com/2/users/2195808223/tweets";

            var client = _clientFactory.CreateClient();

            while (!caseNumbersPublished)
            {
                try
                {
                    caseNumbersPublished = await CheckCaseNumbers(baseUrl, client);
                    _logger.LogInformation($"PublishCaseNumbers executed {count++} times");

                    if (!caseNumbersPublished)
                        Thread.Sleep(GetWaitTime(DateTime.Now));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"PublishCaseNumbers exception caught: {ex.Message} {ex.StackTrace}");
                }
            }
        }

        private async Task<bool> CheckCaseNumbers(string url, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["TwitterApiSettings:BearerToken"]);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<TwitterResponse>(content);
                var dateToSearchFor = DateTime.Now.AddDays(-1);
                var daySuffix = GetDaySuffix(dateToSearchFor.Day);

                var dateText1 = dateToSearchFor.ToString("dd MMMM");
                var dateText2 = dateToSearchFor.ToString("dd MMM");
                var dateText3 = dateToSearchFor.ToString("d MMMM");
                var dateText4 = dateToSearchFor.ToString("d MMM");
                var dateText5 = string.Format("{0}{1} {2:MMMM}", dateToSearchFor.Day, daySuffix, dateToSearchFor);
                var dateText6 = string.Format("{0}{1} {2:MMM}", dateToSearchFor.Day, daySuffix, dateToSearchFor);
                var dateText7 = string.Format("{0:dd}{1} {0:MMMM}", dateToSearchFor, daySuffix);
                var dateText8 = string.Format("{0:dd}{1} {0:MMM}", dateToSearchFor, daySuffix);

                var latestCaseTweet = model.data.Where(x => (x.text.Contains(dateText1) ||
                                                             x.text.Contains(dateText2) ||
                                                             x.text.Contains(dateText3) ||
                                                             x.text.Contains(dateText4))||
                                                             x.text.Contains(dateText5) ||
                                                             x.text.Contains(dateText6) ||
                                                             x.text.Contains(dateText7) ||
                                                             x.text.Contains(dateText8)
                                                             && x.text.Contains("confirmed cases")).OrderByDescending(y => y.id).FirstOrDefault();

                if(latestCaseTweet != null)
                {
                    var caseNumbers = ParseCaseNumbers(latestCaseTweet);

                    if(caseNumbers != -1)
                    {
                        var eventMessage = new CaseNumbersPublishedEvent(caseNumbers);
                        await _publishEndPoint.Publish(eventMessage);
                        _logger.LogInformation($"CaseNumbersPublihsedEvent for {eventMessage.Date.ToString("dd/MM/yyyy")} successfully queued");
                        _hostApplicationLifeTime.StopApplication();
                        return true;
                    }
                    _logger.LogInformation($"Latest tweet identified but case numbers could not be parsed from it ");
                    //return true here to stop the worker service, no point in keeping it running if it's found a tweet for today but
                    //can't parse the case numbers from it
                    _hostApplicationLifeTime.StopApplication();
                    return true;
                }
                return false;
            }
            else
            {
                _logger.LogInformation($"Unsuccessful Twitter API response received");
                return false;
            }
        }

        private int ParseCaseNumbers(Tweet latestCaseTweet)
        {
            var text = latestCaseTweet.text.Substring(0, latestCaseTweet.text.IndexOf("confirmed cases"));
            var resultString = Regex.Match(text, @"\d{1,}(?:\,?\d{3})*", RegexOptions.RightToLeft).Value;
            resultString = resultString.Replace(",", "");

            if (resultString != null && int.TryParse(resultString, out int result))
                return result;

            return -1;
        }

        //try not to spam the arcgis server too often when we start polling at 2pm as it's unlikely cases will be released until 6pm on most days
        private int GetWaitTime(DateTime currentTime)
        {
            var hour = currentTime.ToLocalTime().Hour;
            var oneMinute = 60000;

            if (hour >= 16)
                return oneMinute;
            if (hour >= 15)
                return oneMinute * 3;

            return oneMinute * 5;
        }

        private string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}