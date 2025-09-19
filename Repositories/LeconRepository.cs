using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class LeconRepository : IRepository<Lecon>
    {
        private readonly SqlConnectionFactory _factory;

        public LeconRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Lecon> GetAll()
        {
            List<Lecon> lecons = new List<Lecon>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM LECON", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lecon lecon = new Lecon
                            {
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                DateHeure = (DateTime)reader["date heure"],
                                IdEleve = Convert.ToInt32(reader["id élève"]),
                                IdMoniteur = Convert.ToInt32(reader["id moniteur"]),
                                Duree = Convert.ToInt32(reader["durée"])
                            };

                            lecons.Add(lecon);
                        }
                    }
                }
            }

            return lecons;
        }

        public Lecon? GetById(int id)
        {
            // Clé composite, pas un seul int
            throw new NotSupportedException("Utiliser GetByKey(...) avec les 4 clés pour cette entité.");
        }

        public Lecon? GetByKey(string modeleVehicule, DateTime dateHeure, int idEleve, int idMoniteur)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    SELECT * FROM LECON
                    WHERE [modèle véhicule] = @modele
                      AND [date heure] = @date
                      AND [id élève] = @eleve
                      AND [id moniteur] = @moniteur", connection))
                {
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = modeleVehicule;
                    command.Parameters.Add("@date", SqlDbType.Date).Value = dateHeure.Date;
                    command.Parameters.Add("@eleve", SqlDbType.Int).Value = idEleve;
                    command.Parameters.Add("@moniteur", SqlDbType.Int).Value = idMoniteur;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Lecon
                            {
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                DateHeure = (DateTime)reader["date heure"],
                                IdEleve = Convert.ToInt32(reader["id élève"]),
                                IdMoniteur = Convert.ToInt32(reader["id moniteur"]),
                                Duree = Convert.ToInt32(reader["durée"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Lecon? Add(Lecon lecon)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO LECON ([modèle véhicule],[date heure],[id élève],[id moniteur],[durée])
                    VALUES (@modele,@date,@eleve,@moniteur,@duree);", connection))
                {
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = lecon.ModeleVehicule;
                    command.Parameters.Add("@date", SqlDbType.Date).Value = lecon.DateHeure.Date;
                    command.Parameters.Add("@eleve", SqlDbType.Int).Value = lecon.IdEleve;
                    command.Parameters.Add("@moniteur", SqlDbType.Int).Value = lecon.IdMoniteur;
                    command.Parameters.Add("@duree", SqlDbType.Int).Value = lecon.Duree;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetByKey(lecon.ModeleVehicule, lecon.DateHeure, lecon.IdEleve, lecon.IdMoniteur);

                    return null;
                }
            }
        }

        public bool Update(Lecon lecon)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    UPDATE LECON
                    SET [durée] = @duree
                    WHERE [modèle véhicule] = @modele
                      AND [date heure] = @date
                      AND [id élève] = @eleve
                      AND [id moniteur] = @moniteur", connection))
                {
                    command.Parameters.Add("@duree", SqlDbType.Int).Value = lecon.Duree;
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = lecon.ModeleVehicule;
                    command.Parameters.Add("@date", SqlDbType.Date).Value = lecon.DateHeure.Date;
                    command.Parameters.Add("@eleve", SqlDbType.Int).Value = lecon.IdEleve;
                    command.Parameters.Add("@moniteur", SqlDbType.Int).Value = lecon.IdMoniteur;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            throw new NotSupportedException("Utiliser DeleteByKey(...) avec les 4 clés pour cette entité.");
        }

        public bool DeleteByKey(string modeleVehicule, DateTime dateHeure, int idEleve, int idMoniteur)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    DELETE FROM LECON
                    WHERE [modèle véhicule] = @modele
                      AND [date heure] = @date
                      AND [id élève] = @eleve
                      AND [id moniteur] = @moniteur", connection))
                {
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = modeleVehicule;
                    command.Parameters.Add("@date", SqlDbType.Date).Value = dateHeure.Date;
                    command.Parameters.Add("@eleve", SqlDbType.Int).Value = idEleve;
                    command.Parameters.Add("@moniteur", SqlDbType.Int).Value = idMoniteur;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
