using System.Configuration;
using System.Data;
using System.Text;
using Models;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using static DAL.UploadRepository;
using System;


namespace DAL;

public class UploadRepository : IUploadRepository
{
    public async Task DeserializeJsonResult()
    {
        string fileName = @"C:\Users\crris\source\repos\FullStackTechTest\data.json";
        using FileStream openStream = File.OpenRead(fileName);

        ImportData[] importData = JsonSerializer.Deserialize<ImportData[]>(openStream);

        foreach(var individualPerson in importData)
        {
            //Console.WriteLine(individualPerson.FirstName);
            SaveDataToPersonTable(individualPerson);
        }

    }

    public async Task SaveDataToPersonTable(ImportData individualPerson)
    {
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO people (FirstName, LastName, GMC)");
        sql.Append("VALUES (");
        sql.Append("@firstName, ");
        sql.Append("@lastName, ");
        sql.Append("@Gmc");
        sql.Append(");");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("@firstName", individualPerson.FirstName);
            command.Parameters.AddWithValue("@lastName", individualPerson.LastName);
            command.Parameters.AddWithValue("@Gmc", individualPerson.Gmc);

            Console.WriteLine(sql.ToString());

            await command.ExecuteNonQueryAsync();
        }
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