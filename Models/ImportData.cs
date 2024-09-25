using System.Text.Json.Serialization;

namespace Models
{
    public partial class ImportPerson
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("GMC")]
        public long Gmc { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("address")]
        public ImportAddress[] Address { get; set; }
    }

    public partial class ImportAddress
    {

        public string PersonID { get; set; }

        [JsonPropertyName("line1")]
        public string Line1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
    }
}
