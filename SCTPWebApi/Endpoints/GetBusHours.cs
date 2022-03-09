using ConsoleApp1.Models;
using HtmlAgilityPack;
using SCTPWebApi.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SCTPWebApi.Endpoints
{
    public class GetBusHours : Endpoint<BusHourRequest, List<NextBus>>
    {
        public override void Configure()
        {
            Get("/api/getTime");
            AllowAnonymous();
        }
        public async Task<HtmlDocument> GetSchedules(string url)
        {
            HttpClient httpClient = new();
            var response = await httpClient.GetAsync(url);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(await response.Content.ReadAsStringAsync());
            return htmlDoc;
        }
        public override async Task HandleAsync(BusHourRequest req, CancellationToken ct)
        {

            //&linha=0&hash123=WTnUpZydgrp4NdN6RXQQEMvC9nL8CuEbf372D56UBGA
            HtmlDocument htmlDoc = await GetSchedules($"http://www.stcp.pt/itinerarium/soapclient.php?codigo={req.BusStopCode}&linha=0&hash123=WTnUpZydgrp4NdN6RXQQEMvC9nL8CuEbf372D56UBGA");

            var result = new List<NextBus>();
            //msgBox warning
            IEnumerable<HtmlNode> nodes =
                htmlDoc.DocumentNode.Descendants(0)
                    .Where(n => n.HasClass("warning"));
            if (nodes.Any())
            {
                await SendAsync(result);
                return;
            }


            var tableResults = htmlDoc.GetElementbyId("smsBusResults");
            foreach (HtmlNode row in tableResults.SelectNodes("tr"))
            {
                var cells = row.SelectNodes("td");
                if (cells != null)
                {
                    var busNumber = cells[0].InnerText.Replace("&nbsp;", "- ").Replace("\t", string.Empty).Replace("\n", string.Empty);
                    var nextTime = cells[1].InnerText;
                    var waitTime = cells[2].InnerText;

                    result.Add(new NextBus { BusName = busNumber, NextHour = nextTime, WaitTime = waitTime });
                }
            }
            await SendAsync(result);
        }

    }
}
