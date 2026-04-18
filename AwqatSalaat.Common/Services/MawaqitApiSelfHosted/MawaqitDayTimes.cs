using Newtonsoft.Json;

namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    internal class MawaqitDayTimes
    {
        [JsonProperty("fajr")]
        public string Fajr { get; set; }

        [JsonProperty("sunrise")]
        public string Sunrise { get; set; }

        [JsonProperty("dohr")]
        public string Dohr { get; set; }

        [JsonProperty("asr")]
        public string Asr { get; set; }

        [JsonProperty("maghreb")]
        public string Maghreb { get; set; }

        [JsonProperty("icha")]
        public string Icha { get; set; }
    }
}
