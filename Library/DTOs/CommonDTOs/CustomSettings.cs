using System;
using System.Collections.Generic;
using System.Text;

namespace CommonDTOs
{
    public sealed class CustomSettings
    {
        public ValidattionConfigurations HeaderValidation { get; set; }
        public ValidattionConfigurations AdminValidation { get; set; }
        public Dictionary<string,string> EndpointUrls { get; set; }
        public string ApiGatewayBaseUrl { get; set; }
    }

    public sealed class ValidattionConfigurations
    {
        public bool ISValidationRequired { get; set; }
        public string[] ExcludeControllers { get; set; }
        public string[] ExcludeActions { get; set; }
        public string[] ExcludeApiPath { get; set; }
    }
}
