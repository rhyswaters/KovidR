using System;
using System.Net.Http;
using CaseNumbersWorker.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Messages.Events;
using MassTransit;

namespace CaseNumbersWorker.Services
{
    public class CaseNumbersService : ICaseNumbersService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<CaseNumbersService> _logger;
        private readonly IPublishEndpoint _publishEndPoint;

        public CaseNumbersService(IHttpClientFactory clientFactory, ILogger<CaseNumbersService> logger, IPublishEndpoint publishEndPoint)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _publishEndPoint = publishEndPoint;
        }

        public async Task PublishCaseNumbers()
        {
            //this worker is set to run daily at 2pm but it will run immediately each time the container hosting it is restarted
            //wait until 2pm to run as there is no point in spamming the arcgis server until there is a chance the case numbers have been published

            if(DateTime.Now.Hour < 14)
            {
                var timeToWait = (DateTime.Now.Date.AddHours(14) - DateTime.Now);
                Thread.Sleep(timeToWait);
            }

            int count = 0;

            var baseUrl = "https://services1.arcgis.com/eNO7HHeQ3rUcBllm/arcgis/rest/services/";

            var latestDateUrl = "CovidStatisticsProfileHPSCIreland_DailyDateView/FeatureServer/0/query?f=json&where=1%3D1&outFields=*&returnGeometry=false&outStatistics=%5B%7B%22onStatisticField" +
                                "%22%3A%22Date%22%2C%22outStatisticFieldName%22%3A%22Date_max%22%2C%22statisticType%22%3A%22max%22%7D%5D";

            var casesUrl = "Covid19StatisticsProfileHPSCIrelandView/FeatureServer/0/query?f=json&where=1%3D1&outFields=*&returnGeometry=false&outStatistics=" +
                           "%5B%7B%22onStatisticField%22%3A%22ConfirmedCovidCases%22%2C%22outStatisticFieldName%22%3A%22ConfirmedCovidCases_sum%22%2C%22statisticType%22%3A%22sum%22%7D%5D";

            var client = _clientFactory.CreateClient();

            while (true)
            {
                if (await NewCasesNumbersArePublished(baseUrl + latestDateUrl, client))
                {
                    await CreateCaseNumbersPublishedEvent(baseUrl + casesUrl, client);
                    return;
                }
                else Thread.Sleep(GetWaitTime(DateTime.Now));

                _logger.LogInformation($"PublishCaseNumbers executed {count++} times");
            }
        }

        private async Task<bool> NewCasesNumbersArePublished(string url, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ArcGisResponse>(content);
                if (model.features.Any())
                {
                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var latestDate = start.AddMilliseconds(model.features.FirstOrDefault().attributes.Date_max).ToLocalTime();
                    return latestDate.Date == DateTime.Now.Date;
                }
                else
                {
                    _logger.LogInformation($"Unexpected ArcGis response received");
                    return false;
                }
            }
            else
            {
                _logger.LogInformation($"Unsuccessful ArcGis response received");
                return false;
            }
        }

        private async Task CreateCaseNumbersPublishedEvent(string url, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ArcGisResponse>(content);
                if (model.features.Any())
                {
                    var caseNumbers = model.features.FirstOrDefault().attributes.ConfirmedCovidCases_sum;
                    var eventMessage = new CaseNumbersPublishedEvent(caseNumbers);
                    await _publishEndPoint.Publish(eventMessage);
                    _logger.LogInformation($"CaseNumbersPublihsedEvent for {eventMessage.Date.ToString("dd/MM/yyyy")} successfully queued");
                }
                else
                {
                    _logger.LogInformation($"Unexpected ArcGis response received");
                }
            }
            else
            {
                _logger.LogInformation($"Unsuccessful ArcGis response received");
            }
        }

        //try not to spam the arcgis server too often when we start polling at 2pm as it's unlikely cases will be released until 6pm on most days
        private int GetWaitTime(DateTime currentTime)
        {
            var hour = currentTime.Hour;
            var oneMinute = 60000;

            if (hour > 17)
                return oneMinute;
            if (hour > 16)
                return oneMinute * 5;

            return oneMinute * 10;
        }
    }
}