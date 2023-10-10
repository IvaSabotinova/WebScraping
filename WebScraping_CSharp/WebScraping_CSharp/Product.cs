using Newtonsoft.Json;

namespace WebScraping_CSharp
{
    [JsonObject]
    public class Product
    {
        [JsonProperty("productName")]
        public string Name { get; set; } = null!;

        [JsonProperty("price")]
        public string Price { get; set; } = null!;

        [JsonProperty("rating")]
        public string Rating { get; set; } = null!;
    }
}
