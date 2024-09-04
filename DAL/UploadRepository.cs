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

        foreach (var individualPerson in importData)
        {
            if (!await DoesPersonExist(individualPerson))
            {
                await SaveDataToTables(individualPerson);
            }
            else
            {
                Console.WriteLine($"Person {individualPerson.FirstName} {individualPerson.LastName} with GMC {individualPerson.Gmc} already exists.");
            }
        }

    }
    private async Task<bool> DoesPersonExist(ImportData individualPerson)
    {
        const string checkPersonSql = @"
                SELECT COUNT(1)
                FROM people
                WHERE GMC = @Gmc AND FirstName = @firstName AND LastName = @lastName";

        await using var connection = new MySqlConnection(Config.DbConnectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(checkPersonSql, connection);
        command.Parameters.AddWithValue("@Gmc", individualPerson.Gmc);
        command.Parameters.AddWithValue("@firstName", individualPerson.FirstName);
        command.Parameters.AddWithValue("@lastName", individualPerson.LastName);

        var result = Convert.ToInt32(await command.ExecuteScalarAsync());
        return result > 0;
    }

    private async Task SaveDataToTables(ImportData individualPerson)
    {

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var insertPersonSql = @"
                INSERT INTO people (FirstName, LastName, GMC)
                VALUES (@firstName, @lastName, @Gmc);
                SELECT LAST_INSERT_ID();";

                var personCommand = new MySqlCommand(insertPersonSql, connection, transaction);
                personCommand.Parameters.AddWithValue("@firstName", individualPerson.FirstName);
                personCommand.Parameters.AddWithValue("@lastName", individualPerson.LastName);
                personCommand.Parameters.AddWithValue("@Gmc", individualPerson.Gmc);
                var idOfPersonLastSaved = Convert.ToInt32(await personCommand.ExecuteScalarAsync());

                if (individualPerson.Address != null && individualPerson.Address.Length > 0)
                {
                    var insertAddressSql = @"
                    INSERT INTO addresses (PersonId, Line1, City, Postcode)
                    VALUES (@PersonId, @Line1, @City, @Postcode);";

                    foreach (var address in individualPerson.Address)
                    {
                        var addressCommand = new MySqlCommand(insertAddressSql, connection, transaction);
                        addressCommand.Parameters.AddWithValue("@PersonId", idOfPersonLastSaved);
                        addressCommand.Parameters.AddWithValue("@Line1", address.Line1 ?? (object)DBNull.Value);
                        addressCommand.Parameters.AddWithValue("@City", address.City ?? (object)DBNull.Value);
                        addressCommand.Parameters.AddWithValue("@Postcode", address.Postcode ?? (object)DBNull.Value);
                        await addressCommand.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    Console.WriteLine($"No address provided for {individualPerson.FirstName} {individualPerson.LastName}");
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
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