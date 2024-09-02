using System.Configuration;
using System.Data;
using System.Text;
using Models;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DAL;

public class UploadRepository : IUploadRepository
{
    public async Task DeserializeJsonResult()
    {
        string fileName = @"C:\Users\crris\source\repos\FullStackTechTest\data.json";
        using FileStream openStream = File.OpenRead(fileName);

        ImportData[] importData = JsonSerializer.Deserialize<ImportData[]>(openStream);

        foreach(var individualperson in importData)
        {
            Console.WriteLine(individualperson.FirstName);
        }

    }

    public async Task SaveDataToPersonTable()
    {

    }

    public async Task SaveDataToAddressTable()
    {

    }


    public partial class ImportData
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("GMC")]
        public long Gmc { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("address")]
        public Address[] Address { get; set; }
    }

    public partial class Address
    {
        [JsonPropertyName("line1")]
        public string Line1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
    }

}