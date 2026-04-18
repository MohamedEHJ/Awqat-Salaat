using System;

namespace AwqatSalaat.Services.MawaqitApiSelfHosted
{
    internal class MawaqitApiSelfHostedApiException : Exception
    {
        public MawaqitApiSelfHostedApiException(string message) : base(message) { }
    }
}
