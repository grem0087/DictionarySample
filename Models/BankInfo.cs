using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Models
{
    public class BankInfo
    {
        [BsonId]
        [JsonProperty("bik")]
        public string Bik { get; set; }
        [JsonProperty("bank-name")]
        public string Name { get; set; }
        [JsonProperty("correspondent-account")]
        public string CorrespondentAccount { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("place-type")]
        public string PlaceType { get; set; }
        [JsonProperty("is-closed")]
        public bool IsClosed { get; set; }
    }
}
