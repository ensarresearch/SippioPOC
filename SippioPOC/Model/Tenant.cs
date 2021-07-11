using Newtonsoft.Json;

namespace SippioPOC.Model
{
    public class Tenant
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "TenantID")]
        public string TenantID { get; set; }
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }


    }

    
   
}
