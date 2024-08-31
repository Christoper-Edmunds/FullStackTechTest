using System.Configuration;
using System.Data;
using System.Text;
using Models;
using MySql.Data.MySqlClient;
using System.Text.Json;


namespace DAL;

public class UploadRepository : IUploadRepository
{
    public async Task<List<Person>> ListAllAsync()
    {
        var peopleList = new List<Person>();
        
        var sql = new StringBuilder();
        sql.AppendLine("SELECT * FROM people");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();
            
            var command = new MySqlCommand(sql.ToString(), connection);
            
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                peopleList.Add(PopulatePerson(reader));
            }
        }

        return peopleList;
    }

    public async Task SaveAsync(Person person)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE people SET");
        sql.AppendLine("FirstName = @firstName,");
        sql.AppendLine("LastName = @lastName,");
        sql.AppendLine("GMC = @gmc");
        sql.AppendLine("WHERE Id = @personId");
        
        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("firstName", person.FirstName);
            command.Parameters.AddWithValue("lastName", person.LastName);
            command.Parameters.AddWithValue("gmc", person.GMC);
            command.Parameters.AddWithValue("personId", person.Id);

            await command.ExecuteNonQueryAsync();
        }
    }

    private Person PopulatePerson(IDataRecord data)
    {
        var person = new Person
        {
            Id = int.Parse(data["Id"].ToString()),
            FirstName = data["FirstName"].ToString(),
            LastName = data["LastName"].ToString(),
            GMC = int.Parse(data["GMC"].ToString())
        };
        return person;
    }

    public async Task<Address> GetForPersonIdAsync(int personId)
    {
        var address = new Address();

        var sql = new StringBuilder();
        sql.AppendLine("SELECT * FROM addresses");
        sql.AppendLine("WHERE PersonId = @personId");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("personId", personId);

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                address = PopulateAddress(reader);
            }
        }

        return address;
    }

    public async Task SaveAsync(Address address)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE addresses SET");
        sql.AppendLine("Line1 = @line1,");
        sql.AppendLine("City = @city,");
        sql.AppendLine("Postcode = @postcode");
        sql.AppendLine("WHERE Id = @addressId");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("line1", address.Line1);
            command.Parameters.AddWithValue("city", address.City);
            command.Parameters.AddWithValue("postcode", address.Postcode);
            command.Parameters.AddWithValue("addressId", address.Id);

            await command.ExecuteNonQueryAsync();
        }
    }

    private Address PopulateAddress(IDataRecord data)
    {
        var address = new Address
        {
            Id = int.Parse(data["Id"].ToString()),
            PersonId = int.Parse(data["Id"].ToString()),
            Line1 = data["Line1"].ToString(),
            City = data["City"].ToString(),
            Postcode = data["Postcode"].ToString()
        };
        return address;
    }

    public async Task JSONDeserialize()
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