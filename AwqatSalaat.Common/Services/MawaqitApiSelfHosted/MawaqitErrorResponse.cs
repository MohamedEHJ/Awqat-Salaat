using Newtonsoft.Json;

namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    internal class MawaqitErrorResponse
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }
    }
}
