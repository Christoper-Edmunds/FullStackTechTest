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
        //"C:\Users\crris\source\repos\FullStackTechTest\data.json"
        string fileName = @"C:\Users\crris\source\repos\FullStackTechTest\data.json";
        using FileStream openStream = File.OpenRead(fileName);
        //ImportData? importData =
        //    await JsonSerializer.DeserializeAsync<ImportData>(openStream);

        //ImportData[] FromJson(string fileName) => JsonSerializer.Deserialize<ImportData[]>(fileName);
        ImportData[] importData = JsonSerializer.Deserialize<ImportData[]>(openStream);


        //foreach(var individualperson in importData.)
        //Console.WriteLine($"firstName: {importData?.FirstName}");
        //Console.WriteLine($"lastName: {importData?.lastName}");
        //Console.WriteLine($"GMC: {importData?.GMC}");
        //Console.WriteLine($"line1: {importData?.line1}");
        //Console.WriteLine($"city: {importData?.city}");
        //Console.WriteLine($"postcode: {importData?.postcode}");


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