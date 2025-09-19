using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class CalendrierRepository : IRepository<Calendrier>
    {
        private readonly SqlConnectionFactory _factory;

        public CalendrierRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Calendrier> GetAll()
        {
            List<Calendrier> dates = new List<Calendrier>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM CALENDRIER ORDER BY [date heure]", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Calendrier c = new Calendrier
                            {
                                DateHeure = (DateTime)reader["date heure"]
                            };

                            dates.Add(c);
                        }
                    }
                }
            }

            return dates;
        }

        public Calendrier? GetById(int id)
        {
            // La clé n'est pas un int
            throw new NotSupportedException("Utiliser GetByDate(DateTime date) pour cette entité.");
        }

        public Calendrier? GetByDate(DateTime date)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM CALENDRIER WHERE [date heure] = @d", connection))
                {
                    command.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Calendrier
                            {
                                DateHeure = (DateTime)reader["date heure"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Calendrier? Add(Calendrier calendrier)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO CALENDRIER ([date heure]) VALUES (@d);", connection))
                {
                    command.Parameters.Add("@d", SqlDbType.Date).Value = calendrier.DateHeure.Date;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetByDate(calendrier.DateHeure);

                    return null;
                }
            }
        }

        public bool Update(Calendrier calendrier)
        {
            // Pas vraiment utile : la PK est la date, donc un UPDATE équivaut à un DELETE + INSERT
            throw new NotSupportedException("Update non supporté sur CALENDRIER (PK = date).");
        }

        public bool Delete(int id)
        {
            throw new NotSupportedException("Utiliser DeleteByDate(DateTime date) pour cette entité.");
        }

        public bool DeleteByDate(DateTime date)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM CALENDRIER WHERE [date heure] = @d", connection))
                {
                    command.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
