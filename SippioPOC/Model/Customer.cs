using Newtonsoft.Json;

namespace SippioPOC.Model
{
    public class Customer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
       
        public Product[] Products { get; set; }
        public Address Address { get; set; }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    
    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Country { get; set; }
       
    }


    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }
}
