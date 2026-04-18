namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    public class MawaqitApiSelfHostedRequest : WebRequestBase
    {
        public string BaseUrl { get; set; }
        public string MasjidId { get; set; }

        public override string GetUrl() => $"{GetRoot()}/calendar/{Date.Month}";

        public string GetMosqueInfoUrl() => $"{GetRoot()}/";

        private string GetRoot() => $"{BaseUrl.TrimEnd('/')}/api/v1/{MasjidId}";
    }
}
