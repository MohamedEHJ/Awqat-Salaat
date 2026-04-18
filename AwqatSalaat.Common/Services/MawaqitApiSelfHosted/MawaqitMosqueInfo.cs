using Newtonsoft.Json;

namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    internal class MawaqitMosqueInfo
    {
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }
}
