using AwqatSalaat.Data;
using AwqatSalaat.Helpers;
using AwqatSalaat.Services.Nominatim;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    public class MawaqitApiSelfHostedClient : IServiceClient
    {
        public bool SupportMonthlyData => true;

        public async Task<ServiceData> GetDataAsync(IRequest request)
        {
            var req = (MawaqitApiSelfHostedRequest)request;
            Log.Debug("[Mawaqit] Getting data for request: {@request}", req);

            if (string.IsNullOrWhiteSpace(req.BaseUrl))
            {
                throw new MawaqitApiSelfHostedApiException("Base URL is not configured.");
            }

            if (string.IsNullOrWhiteSpace(req.MasjidId))
            {
                throw new MawaqitApiSelfHostedApiException("Masjid ID is not configured.");
            }

            var mosqueInfo = await GetAsync<MawaqitMosqueInfo>(req.GetMosqueInfoUrl());
            var monthData = await GetAsync<MawaqitDayTimes[]>(req.GetUrl());

            if (monthData == null || monthData.Length == 0)
            {
                throw new MawaqitApiSelfHostedApiException("No prayer times returned by the service.");
            }

            var times = BuildMonthlyTimes(monthData, req.Date);
            var location = await ResolveLocationAsync(mosqueInfo);

            return new ServiceData
            {
                Times = times,
                Location = location,
            };
        }

        private static Dictionary<DateTime, PrayerTimes> BuildMonthlyTimes(MawaqitDayTimes[] monthData, DateTime requestDate)
        {
            int year = requestDate.Year;
            int month = requestDate.Month;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int daysToRead = Math.Min(monthData.Length, daysInMonth);

            var result = new Dictionary<DateTime, PrayerTimes>(daysToRead);

            for (int i = 0; i < daysToRead; i++)
            {
                var day = monthData[i];

                if (day == null)
                {
                    continue;
                }

                var date = new DateTime(year, month, i + 1);
                var dict = new Dictionary<string, DateTime>(6)
                {
                    [nameof(PrayerTimes.Fajr)] = ParseTimeToLocal(day.Fajr, date),
                    [nameof(PrayerTimes.Shuruq)] = ParseTimeToLocal(day.Sunrise, date),
                    [nameof(PrayerTimes.Dhuhr)] = ParseTimeToLocal(day.Dohr, date),
                    [nameof(PrayerTimes.Asr)] = ParseTimeToLocal(day.Asr, date),
                    [nameof(PrayerTimes.Maghrib)] = ParseTimeToLocal(day.Maghreb, date),
                    [nameof(PrayerTimes.Isha)] = ParseTimeToLocal(day.Icha, date),
                };

                result.Add(date, new PrayerTimes(dict));
            }

            return result;
        }

        private static DateTime ParseTimeToLocal(string time, DateTime baseDate)
        {
            var parsed = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture);
            return baseDate.Date + parsed.TimeOfDay;
        }

        private static async Task<Location> ResolveLocationAsync(MawaqitMosqueInfo info)
        {
            string country = null;
            string city = null;

            if (!string.IsNullOrEmpty(info?.CountryCode))
            {
                country = CountriesProvider.GetCountries()
                    .FirstOrDefault(c => string.Equals(c.Code, info.CountryCode, StringComparison.OrdinalIgnoreCase))?.Name
                    ?? info.CountryCode;
            }

            try
            {
                var place = await NominatimClient.Reverse(info.Latitude, info.Longitude, CancellationToken.None);
                city = place?.Address?.City;

                if (string.IsNullOrEmpty(country))
                {
                    country = place?.Address?.Country;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[Mawaqit] Reverse geocoding failed: {message}", ex.Message);
            }

            return new Location { Country = country, City = city };
        }

        private static async Task<T> GetAsync<T>(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Awqat Salaat");
                    Log.Debug($"[Mawaqit] Getting data from: {url}");

                    var response = await client.GetAsync(url);
                    Log.Debug($"[Mawaqit] Response status code: {response.StatusCode}");

                    string body = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        if (string.IsNullOrEmpty(body))
                        {
                            throw new MawaqitApiSelfHostedApiException("The service returned an empty response.");
                        }

                        return JsonConvert.DeserializeObject<T>(body);
                    }

                    string detail = null;

                    if (!string.IsNullOrEmpty(body))
                    {
                        try
                        {
                            detail = JsonConvert.DeserializeObject<MawaqitErrorResponse>(body)?.Detail;
                        }
                        catch
                        {
                            // response body is not a standard error payload
                        }
                    }

                    string message = !string.IsNullOrEmpty(detail)
                        ? detail
                        : $"Something went wrong: {response.ReasonPhrase} (StatusCode={(int)response.StatusCode})";

                    throw new MawaqitApiSelfHostedApiException(message);
                }
            }
            catch (HttpRequestException hre)
            {
                throw new NetworkException("Could not reach the server.", hre);
            }
            catch (WebException wex)
            {
                throw new NetworkException("Could not reach the server.", wex);
            }
        }
    }
}
