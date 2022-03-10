﻿using ConsoleApp1.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using SCTPWebApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SCTPWebApi.Endpoints
{
    public class GetBusHours : Endpoint<BusHourRequest, List<NextBus>>
    {
        private readonly IMemoryCache _memoryCache;
        const string CacheKey = "BusHours";
        public GetBusHours(IMemoryCache memoryCache) =>
        _memoryCache = memoryCache;

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
        private async Task<string> GetHash(string busStopCode)
        {
            string cacheEntry;
            if (!_memoryCache.TryGetValue(CacheKey, out cacheEntry))
            {
                HttpClient httpClient = new();
                var response = await httpClient.GetAsync($"https://www.stcp.pt/pt/viajar/horarios/?paragem={busStopCode}&t=smsbus");
                var html = await response.Content.ReadAsStringAsync();
                var ini = html.IndexOf("getParagemInfo(");
                var newHtml = html.Substring(ini);
                var end = newHtml.IndexOf(")");
                var hash = html.Substring(ini, end).Split(',')[2].Replace("'", "");
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                 // Keep in cache for this time, reset time if accessed.
                 .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                // Save data in cache.
                _memoryCache.Set(CacheKey, hash, cacheEntryOptions);
                cacheEntry = hash;

            }
            return cacheEntry;
        }
        public override async Task HandleAsync(BusHourRequest req, CancellationToken ct)
        {
            var hash = await GetHash(req.BusStopCode);
            //&linha=0&hash123=WTnUpZydgrp4NdN6RXQQEMvC9nL8CuEbf372D56UBGA
            HtmlDocument htmlDoc = await GetSchedules($"http://www.stcp.pt/itinerarium/soapclient.php?codigo={req.BusStopCode}&linha=0&hash123={hash}");

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
