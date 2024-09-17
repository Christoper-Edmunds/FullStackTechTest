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
