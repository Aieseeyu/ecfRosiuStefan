using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class EleveRepository : IRepository<Eleve>
    {
        private readonly SqlConnectionFactory _factory;

        public EleveRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Eleve> GetAll()
        {
            List<Eleve> eleves = new List<Eleve>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM ELEVE", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Eleve eleve = new Eleve
                            {
                                IdEleve = Convert.ToInt32(reader["id élève"]),
                                NomEleve = reader["nom élève"].ToString() ?? string.Empty,
                                PrenomEleve = reader["prénom élève"].ToString() ?? string.Empty,
                                Code = Convert.ToBoolean(reader["code"]),
                                Conduite = Convert.ToBoolean(reader["conduite"]),
                                DateNaissance = (DateTime)reader["date naissance"]
                            };

                            eleves.Add(eleve);
                        }
                    }
                }
            }

            return eleves;
        }

        public Eleve? GetById(int id)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM ELEVE WHERE [id élève] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Eleve
                            {
                                IdEleve = Convert.ToInt32(reader["id élève"]),
                                NomEleve = reader["nom élève"].ToString() ?? string.Empty,
                                PrenomEleve = reader["prénom élève"].ToString() ?? string.Empty,
                                Code = Convert.ToBoolean(reader["code"]),
                                Conduite = Convert.ToBoolean(reader["conduite"]),
                                DateNaissance = (DateTime)reader["date naissance"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Eleve? Add(Eleve eleve)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO ELEVE ([id élève],[nom élève],[prénom élève],[code],[conduite],[date naissance])
                    VALUES (@id,@nom,@prenom,@code,@conduite,@naiss);", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = eleve.IdEleve;
                    command.Parameters.Add("@nom", SqlDbType.VarChar, 50).Value = eleve.NomEleve;
                    command.Parameters.Add("@prenom", SqlDbType.VarChar, 50).Value = eleve.PrenomEleve;
                    command.Parameters.Add("@code", SqlDbType.Bit).Value = eleve.Code;
                    command.Parameters.Add("@conduite", SqlDbType.Bit).Value = eleve.Conduite;
                    command.Parameters.Add("@naiss", SqlDbType.Date).Value = eleve.DateNaissance.Date;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetById(eleve.IdEleve);

                    return null;
                }
            }
        }

        public bool Update(Eleve eleve)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    UPDATE ELEVE
                    SET [nom élève] = @nom,
                        [prénom élève] = @prenom,
                        [code] = @code,
                        [conduite] = @conduite,
                        [date naissance] = @naiss
                    WHERE [id élève] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = eleve.IdEleve;
                    command.Parameters.Add("@nom", SqlDbType.VarChar, 50).Value = eleve.NomEleve;
                    command.Parameters.Add("@prenom", SqlDbType.VarChar, 50).Value = eleve.PrenomEleve;
                    command.Parameters.Add("@code", SqlDbType.Bit).Value = eleve.Code;
                    command.Parameters.Add("@conduite", SqlDbType.Bit).Value = eleve.Conduite;
                    command.Parameters.Add("@naiss", SqlDbType.Date).Value = eleve.DateNaissance.Date;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM ELEVE WHERE [id élève] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
