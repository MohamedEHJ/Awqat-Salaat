using AwqatSalaat.Data;
using AwqatSalaat.Helpers;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwqatSalaat.Services.QCH
{
    public class QchClient : IServiceClient
    {
        private const string QchEndpoint = "https://www.qatarch.com/cal";

        // Use a static HttpClient to avoid port-exhaustion problem
        private static readonly HttpClient _httpClient = new HttpClient();

        public bool SupportMonthlyData => false;

        static QchClient()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Awqat Salaat");
        }

        public async Task<ServiceData> GetDataAsync(IRequest request)
        {
            var req = (QchRequest)request;
            Log.Debug("[QCH] Getting data for request: {@request}", req);

            if (req.GetEntireMonth)
            {
                throw new NotImplementedException();
            }

            var city = QchCities.Cities.SingleOrDefault(c => c.CityId == req.CityId);

            if (city is null)
            {
                throw new ArgumentException($"Invalid city ID: {req.CityId}", nameof(request));
            }

            try
            {
                using (var stream = await _httpClient.GetStreamAsync(QchEndpoint).ConfigureAwait(false))
                {
                    var scrapData = await ScrapDataAsync(stream);

                    if (scrapData?.PrayData is null)
                    {
                        throw new QchException("No result");
                    }

                    var cityData = scrapData.PrayData.SingleOrDefault(p => p.CityId == req.CityId);

                    if (cityData is null)
                    {
                        throw new QchException("City data not available");
                    }

                    return new ServiceData
                    {
                        Location = new Location { City = city.Name, Country = city.Country },
                        Times = BuildTimes(cityData, city, scrapData.Date)
                    };
                }
            }
            catch (HttpRequestException hre)
            {
                throw new NetworkException("Could not scrap data from QCH.", hre);
            }
        }

        private static async Task<QchScrapData> ScrapDataAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                QchCalendar calendar = null;

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line is null)
                    {
                        break;
                    }

                    line = line.Trim();

                    if (line.StartsWith("var calData"))
                    {
                        var objStart = line.IndexOf('{');
                        var objStop = line.LastIndexOf("}");

                        if (objStart != -1 && objStop != -1)
                        {
                            var calData = line.Substring(objStart, objStop - objStart + 1);
                            calendar = JsonConvert.DeserializeObject<QchCalendar>(calData);
                        }
                    }
                    else if (line.StartsWith("var prayData"))
                    {
                        var arrayStart = line.IndexOf('[');
                        var arrayStop = line.LastIndexOf("]");

                        if (arrayStart != -1 && arrayStop != -1)
                        {
                            var prayData = line.Substring(arrayStart, arrayStop - arrayStart + 1);
                            var array = JsonConvert.DeserializeObject<QchPrayData[]>(prayData);

                            return new QchScrapData(array, calendar);
                        }

                        break;
                    }
                }
            }

            return null;
        }

        private static Dictionary<DateTime, PrayerTimes> BuildTimes(QchPrayData prayData, QchCity city, DateTime date)
        {
            var dict = new Dictionary<string, DateTime>
            {
                [nameof(PrayerTimes.Fajr)] = ParseTimeToLocal(prayData.Fajr, date, city.TimeZone, false),
                [nameof(PrayerTimes.Shuruq)] = ParseTimeToLocal(prayData.Shuruq, date, city.TimeZone, false),
                [nameof(PrayerTimes.Dhuhr)] = ParseTimeToLocal(prayData.Dhuhr, date, city.TimeZone, false),
                [nameof(PrayerTimes.Asr)] = ParseTimeToLocal(prayData.Asr, date, city.TimeZone, true),
                [nameof(PrayerTimes.Maghrib)] = ParseTimeToLocal(prayData.Maghrib, date, city.TimeZone, true),
                [nameof(PrayerTimes.Isha)] = ParseTimeToLocal(prayData.Isha, date, city.TimeZone, true),
            };
            var times = new PrayerTimes(dict);

            return new Dictionary<DateTime, PrayerTimes> { [date] = times };
        }

        private static DateTime ParseTimeToLocal(string time, DateTime baseDate, string timeZone, bool adjustable)
        {
            var dt = baseDate + DateTime.Parse(time, System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;

            if (adjustable && dt.Hour < 12)
            {
                dt = dt.AddHours(12);
            }

            return TimeZoneHelper.ConvertDateTimeToLocal(dt, timeZone);
        }
    }
}
