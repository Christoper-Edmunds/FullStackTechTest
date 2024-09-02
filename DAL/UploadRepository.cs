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
            await SaveDataToPersonTable(individualPerson);
        }

    }

    private async Task SaveDataToPersonTable(ImportData individualPerson)
    {

        //---- save person to database
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

            await command.ExecuteNonQueryAsync();
        }

        //--- grab record ID from person saved
        var idOfPersonLastSaved = 0;

        sql = new StringBuilder();
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand(sql.ToString(), connection);

            idOfPersonLastSaved = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        //---- save address to database
        sql = new StringBuilder();
        sql.AppendLine("INSERT INTO addresses (PersonId, Line1, City, Postcode)");
        sql.Append("VALUES (");
        sql.Append("@PersonId, ");
        sql.Append("@Line1, ");
        sql.Append("@City");
        sql.Append("@Postcode");
        sql.Append(");");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("@PersonId", idOfPersonLastSaved);
            command.Parameters.AddWithValue("@Line1", individualPerson.Address[0].Line1);
            command.Parameters.AddWithValue("@City", individualPerson.Address[0].City);
            command.Parameters.AddWithValue("@Postcode", individualPerson.Address[0].Postcode);

            await command.ExecuteNonQueryAsync();
        }

    }

    public partial class ImportData //Move me to models
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

    public partial class Address //Move me to models 
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