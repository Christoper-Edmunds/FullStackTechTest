using Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SpecialitiesRepository : ISpecialityRepository
    {
        public async Task<List<Specialities>> ListAllSpecialitiesAsync()
        {
            var specialitiesList = new List<Specialities>();

            var sql = new StringBuilder();
            sql.AppendLine("SELECT * FROM specialities");

            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand(sql.ToString(), connection);

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    specialitiesList.Add(PopulateSpeciality(reader));
                }
            }

            return specialitiesList;
        }


        public async Task<List<Specialities>> ListAllSpecialitiesByIdAsync(int personId)
        {
            var specialitiesList = new List<Specialities>();

            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();

                const string listSpecialitiesByIdSQL = @"
                SELECT Specialities.Id, Specialities.SpecialityName
                FROM Specialities
                JOIN PeopleSpecialitiesLinkTable ON Specialities.Id = PeopleSpecialitiesLinkTable.SpecialityID
                JOIN People ON PeopleSpecialitiesLinkTable.PersonID = People.Id
                WHERE People.Id = @personId;";

                await using (var command = new MySqlCommand(listSpecialitiesByIdSQL, connection))
                {
                    command.Parameters.AddWithValue("@personId", personId);

                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        specialitiesList.Add(new Specialities
                        {
                            Id = reader.GetInt32("Id"),
                            SpecialityName = reader.GetString("SpecialityName")
                        });
                    }
                }
            }

            return specialitiesList;
        }

        public async Task SaveSpecialitiesToLinkTable(int personId, int specialityId)
        {
            await ExecuteLinkTableCommandWithDuplicateCheckAsync(
                @"
                INSERT INTO peoplespecialitieslinktable (PersonID, SpecialityID)
                VALUES (@personId, @specialityId);",
                personId, specialityId);
        }

        public async Task RemoveSpecialitiesFromLinkTable(int personId, int specialityId)
        {
            await ExecuteLinkTableCommandAsync(
                @"
                DELETE FROM peoplespecialitieslinktable
                WHERE PersonID = @personId
                AND SpecialityID = @specialityId;",
                personId, specialityId);
        }

        private async Task ExecuteLinkTableCommandWithDuplicateCheckAsync(string sql, int personId, int specialityId)
        {
            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var checkDuplicateSql = @"
                    SELECT COUNT(1)
                    FROM peoplespecialitieslinktable
                    WHERE PersonID = @personId
                    AND SpecialityID = @specialityId;";

                    var checkDuplicateCommand = new MySqlCommand(checkDuplicateSql, connection, transaction);
                    checkDuplicateCommand.Parameters.AddWithValue("@personId", personId);
                    checkDuplicateCommand.Parameters.AddWithValue("@specialityId", specialityId);

                    var exists = (long)await checkDuplicateCommand.ExecuteScalarAsync();

                    if (exists == 0)
                    {
                        var command = new MySqlCommand(sql, connection, transaction);
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@specialityId", specialityId);

                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        Console.WriteLine("record already exists");
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

        private async Task ExecuteLinkTableCommandAsync(string sql, int personId, int specialityId)
        {
            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var command = new MySqlCommand(sql, connection, transaction);
                    command.Parameters.AddWithValue("@personId", personId);
                    command.Parameters.AddWithValue("@specialityId", specialityId);

                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public async Task SaveSpecialityToTableAsync(string speciality)
        {

            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var checkDuplicateSql = @"
                    SELECT COUNT(1)
                    FROM specialities
                    WHERE SpecialityName = @speciality";

                    var checkDuplicateCommand = new MySqlCommand(checkDuplicateSql, connection, transaction);
                    checkDuplicateCommand.Parameters.AddWithValue("@speciality", speciality);

                    var exists = (long)await checkDuplicateCommand.ExecuteScalarAsync();

                    if (exists == 0)
                    {
                        const string SaveSpecialitySQL = @"
                        INSERT INTO specialities (SpecialityName)
                        VALUES (@specialityName);";

                        var insertCommand = new MySqlCommand(SaveSpecialitySQL, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@specialityName", speciality);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        Console.WriteLine("Record already exists");
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

        public async Task DeleteSpecialityFromTableAsync(int specialityId)
        {
            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var checkIfExistsSql = @"
                    SELECT COUNT(1)
                    FROM specialities
                    WHERE Id = @specialityId";

                    var checkIfExistsCommand = new MySqlCommand(checkIfExistsSql, connection, transaction);
                    checkIfExistsCommand.Parameters.AddWithValue("@specialityId", specialityId);

                    var exists = (long)await checkIfExistsCommand.ExecuteScalarAsync();

                    if (exists > 0)
                    {
                        const string deleteSpecialitySql = @"
                        DELETE FROM specialities
                        WHERE Id = @specialityId;";

                        var deleteCommand = new MySqlCommand(deleteSpecialitySql, connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@specialityId", specialityId);

                        await deleteCommand.ExecuteNonQueryAsync();
                        Console.WriteLine("Record deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Record not found.");
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

        public async Task UpdateSpecialityAsync(Specialities speciality)
        {
            await using (var connection = new MySqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var checkIfExistsSql = @"
                    SELECT COUNT(1)
                    FROM specialities
                    WHERE SpecialityID = @specialityId";

                    var checkIfExistsCommand = new MySqlCommand(checkIfExistsSql, connection, transaction);
                    checkIfExistsCommand.Parameters.AddWithValue("@specialityId", speciality.Id);

                    var exists = (long)await checkIfExistsCommand.ExecuteScalarAsync();

                    if (exists > 0)
                    {
                        const string updateSpecialitySql = @"
                        UPDATE specialities
                        SET SpecialityName = @specialityName
                        WHERE SpecialityID = @specialityId;";

                        var updateCommand = new MySqlCommand(updateSpecialitySql, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@specialityName", speciality.SpecialityName);
                        updateCommand.Parameters.AddWithValue("@specialityId", speciality.Id);

                        await updateCommand.ExecuteNonQueryAsync();
                        Console.WriteLine("Record updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Record not found.");
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

        private Specialities PopulateSpeciality(IDataRecord data)
        {
            var speciality = new Specialities
            {
                Id = Convert.ToInt32(data["Id"]),
                SpecialityName = data["SpecialityName"].ToString(),
            };
            return speciality;
        }
    }
}
