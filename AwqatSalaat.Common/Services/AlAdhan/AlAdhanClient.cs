using AwqatSalaat.Data;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwqatSalaat.Services.AlAdhan
{
    public class AlAdhanClient : IServiceClient
    {
        public bool SupportMonthlyData => true;

        // Use a static HttpClient to avoid port-exhaustion problem
        private static readonly HttpClient _httpClient = new HttpClient();

        static AlAdhanClient()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Awqat Salaat");
        }

        public async Task<ServiceData> GetDataAsync(IRequest request)
        {
            var req = (AlAdhanRequest)request;
            Log.Debug("[Al-Adhan] Getting data for request: {@request}", req);

            if (req.GetEntireMonth)
            {
                var res = await GetDataAsync<MonthResponse>(req);

                return new ServiceData
                {
                    Location = new Location { Country = req.Country, City = req.City },
                    Times = res.Times,
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static async Task<T> GetDataAsync<T>(IWebRequest request)
        {
            try
            {
                var url = request.GetUrl();
                Log.Debug($"[Al-Adhan] Getting data from: {url}");
                var httpResponse = await _httpClient.GetAsync(url);
                Log.Debug($"[Al-Adhan] Response status code: {httpResponse.StatusCode}");

                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();
                    T apiResponse = JsonConvert.DeserializeObject<T>(responseBody);
                    return apiResponse;
                }
                else
                {
                    try
                    {
                        string responseBody = await httpResponse.Content.ReadAsStringAsync();
                        ErrorResponse apiResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBody);
                        throw new AlAdhanApiException(apiResponse.Data);
                    }
                    catch (Exception ex) when (!(ex is AlAdhanApiException))
                    {
                        throw new AlAdhanApiException($"Something went wrong: {httpResponse.ReasonPhrase} (StatusCode={httpResponse.StatusCode})");
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                throw new NetworkException("Could not reach the server.", hre);
            }
        }
    }
}