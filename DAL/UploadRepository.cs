using System.Configuration;
using System.Data;
using System.Text;
using Models;
using MySql.Data.MySqlClient;
using System.Text.Json;


namespace DAL;

public class UploadRepository : IUploadRepository
{
    public async Task DeserializeJsonResult()
    {
        //C:\Users\crris\source\repos\FullStackTechTest
        string fileName = "@\"C:\\Users\\crris\\source\\repos\\FullStackTechTest\\data.json\"";
        using FileStream openStream = File.OpenRead(fileName);
        ImportData? importData =
            await JsonSerializer.DeserializeAsync<ImportData>(openStream);

        Console.WriteLine($"firstName: {importData?.firstName}");
        Console.WriteLine($"lastName: {importData?.lastName}");
        Console.WriteLine($"GMC: {importData?.GMC}");
        Console.WriteLine($"line1: {importData?.line1}");
        Console.WriteLine($"city: {importData?.city}");
        Console.WriteLine($"postcode: {importData?.postcode}");

        var deserializedjsonfile = importData;

    }


    public class ImportData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int GMC { get; set; }
        public string line1 { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }

        //{
        // "firstName": "LANNA",
        // "lastName": "Southerns",
        // "GMC": 3875914,
        // "address": [
        //     {
        //         "line1": "Belmont Cottage",
        //         "city": "Grizebeck",
        //         "postcode": "LA17 7XH"
        //     }
        // ]
    }
}