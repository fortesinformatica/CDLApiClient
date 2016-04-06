using Newtonsoft.Json;

namespace CDLApiClient.Models
{
    internal sealed class ConsultaResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("cnpjcpf")]
        public string Cnpjcpf { get; set; }
    }
}