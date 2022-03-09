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
            Verbs(Http.GET);
            Routes("/api/getTime");
            AllowAnonymous();
        }
        public override async Task HandleAsync(BusHourRequest req, CancellationToken ct)
        {
            HttpClient httpClient = new ();
            var response= await httpClient.GetAsync($"http://www.stcp.pt/itinerarium/soapclient.php?codigo={req.BusStopCode}", ct);
            HtmlDocument htmlDoc = new ();
            htmlDoc.LoadHtml(await response.Content.ReadAsStringAsync());
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
