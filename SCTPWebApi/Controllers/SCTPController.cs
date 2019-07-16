using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ConsoleApp1.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace SCTPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SCTPController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("GetTime")]
        public ActionResult<List<NextBus>> GetTime(string station)
        {
            var client = new RestClient($"http://www.stcp.pt/itinerarium/soapclient.php?codigo={station}");
            var request = new RestRequest(string.Empty, Method.GET);
            var response = client.Execute(request);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content);
            var result = new List<NextBus>();
            var tableResults = htmlDoc.GetElementbyId("smsBusResults");
            foreach (HtmlNode row in tableResults.SelectNodes("tr"))
            {
                HtmlNodeCollection cells = row.SelectNodes("td");
                if (cells != null)
                {
                    var busNumber = cells[0].InnerText.Replace("&nbsp;", "- ").Replace("\t", string.Empty).Replace("\n", string.Empty);
                    var nextTime = cells[1].InnerText;
                    var waitTime = cells[2].InnerText;

                    result.Add(new NextBus { BusName = busNumber, NextHour = nextTime, WaitTime = waitTime });
                }

            }

            return result;
        }

        [HttpGet]
        [Route("ObterParagens")]
        public ActionResult<List<RootObject>> ObterParagens(string pesq)
        {
            var client = new RestClient($"https://www.stcp.pt/pt/itinerarium/callservice.php?action=srchstoplines&stopname=a");
            var request = new RestRequest(string.Empty, Method.GET);
            var response = client.Execute(request);
            var content = JsonConvert.DeserializeObject<List<RootObject>>(response.Content);

            return content.Where(c => c.code.Contains(pesq.ToUpper())).ToList();
        }


    }
}
