using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class MoniteurRepository : IRepository<Moniteur>
    {
        private readonly SqlConnectionFactory _factory;

        public MoniteurRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Moniteur> GetAll()
        {
            List<Moniteur> moniteurs = new List<Moniteur>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM MONITEUR", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Moniteur moniteur = new Moniteur
                            {
                                IdMoniteur = Convert.ToInt32(reader["id moniteur"]),
                                NomMoniteur = reader["nom moniteur"].ToString() ?? string.Empty,
                                PrenomMoniteur = reader["prénom moniteur"].ToString() ?? string.Empty,
                                DateNaissance = (DateTime)reader["date naissance"],
                                DateEmbauche = (DateTime)reader["date embauche"],
                                Activite = Convert.ToBoolean(reader["activité"])
                            };

                            moniteurs.Add(moniteur);
                        }
                    }
                }
            }

            return moniteurs;
        }

        public Moniteur? GetById(int id)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM MONITEUR WHERE [id moniteur] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Moniteur
                            {
                                IdMoniteur = Convert.ToInt32(reader["id moniteur"]),
                                NomMoniteur = reader["nom moniteur"].ToString() ?? string.Empty,
                                PrenomMoniteur = reader["prénom moniteur"].ToString() ?? string.Empty,
                                DateNaissance = (DateTime)reader["date naissance"],
                                DateEmbauche = (DateTime)reader["date embauche"],
                                Activite = Convert.ToBoolean(reader["activité"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Moniteur? Add(Moniteur moniteur)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO MONITEUR ([id moniteur],[nom moniteur],[prénom moniteur],[date naissance],[date embauche],[activité])
                    VALUES (@id,@nom,@prenom,@naiss,@embauche,@actif);", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = moniteur.IdMoniteur;
                    command.Parameters.Add("@nom", SqlDbType.VarChar, 50).Value = moniteur.NomMoniteur;
                    command.Parameters.Add("@prenom", SqlDbType.VarChar, 50).Value = moniteur.PrenomMoniteur;
                    command.Parameters.Add("@naiss", SqlDbType.Date).Value = moniteur.DateNaissance.Date;
                    command.Parameters.Add("@embauche", SqlDbType.Date).Value = moniteur.DateEmbauche.Date;
                    command.Parameters.Add("@actif", SqlDbType.Bit).Value = moniteur.Activite;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetById(moniteur.IdMoniteur);

                    return null;
                }
            }
        }

        public bool Update(Moniteur moniteur)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    UPDATE MONITEUR
                    SET [nom moniteur] = @nom,
                        [prénom moniteur] = @prenom,
                        [date naissance] = @naiss,
                        [date embauche] = @embauche,
                        [activité] = @actif
                    WHERE [id moniteur] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = moniteur.IdMoniteur;
                    command.Parameters.Add("@nom", SqlDbType.VarChar, 50).Value = moniteur.NomMoniteur;
                    command.Parameters.Add("@prenom", SqlDbType.VarChar, 50).Value = moniteur.PrenomMoniteur;
                    command.Parameters.Add("@naiss", SqlDbType.Date).Value = moniteur.DateNaissance.Date;
                    command.Parameters.Add("@embauche", SqlDbType.Date).Value = moniteur.DateEmbauche.Date;
                    command.Parameters.Add("@actif", SqlDbType.Bit).Value = moniteur.Activite;

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

                using (SqlCommand command = new SqlCommand("DELETE FROM MONITEUR WHERE [id moniteur] = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
