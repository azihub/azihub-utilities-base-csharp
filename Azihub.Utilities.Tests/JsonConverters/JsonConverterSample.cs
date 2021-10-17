using Azihub.Utilities.Base.JsonConverters;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Azihub.Utilities.Tests.JsonConverters
{
    public class JsonConverterSample
    {
        [JsonProperty("email")]
        [JsonConverter(typeof(MailAddressConverter))]
        public MailAddress Email { get; set; }
    }
}
