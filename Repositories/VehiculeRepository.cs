using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class VehiculeRepository : IRepository<Vehicule>
    {
        private readonly SqlConnectionFactory _factory;

        public VehiculeRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Vehicule> GetAll()
        {
            List<Vehicule> vehicules = new List<Vehicule>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM VEHICULE", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehicule vehicule = new Vehicule
                            {
                                Immatriculation = reader["n°immatriculation"].ToString() ?? string.Empty,
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                Etat = Convert.ToBoolean(reader["état"])
                            };

                            vehicules.Add(vehicule);
                        }
                    }
                }
            }

            return vehicules;
        }

        public Vehicule? GetById(int id)
        {
            // La clé primaire n'est pas un int pour cette entité
            throw new NotSupportedException("Utiliser GetByImmatriculation(string immat) pour cette entité.");
        }

        public Vehicule? GetByImmatriculation(string immatriculation)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM VEHICULE WHERE [n°immatriculation] = @immat", connection))
                {
                    command.Parameters.Add("@immat", SqlDbType.VarChar, 9).Value = immatriculation;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Vehicule
                            {
                                Immatriculation = reader["n°immatriculation"].ToString() ?? string.Empty,
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                Etat = Convert.ToBoolean(reader["état"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Vehicule? Add(Vehicule vehicule)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO VEHICULE ([n°immatriculation],[modèle véhicule],[état])
                    VALUES (@immat,@modele,@etat);", connection))
                {
                    command.Parameters.Add("@immat", SqlDbType.VarChar, 9).Value = vehicule.Immatriculation;
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = vehicule.ModeleVehicule;
                    command.Parameters.Add("@etat", SqlDbType.Bit).Value = vehicule.Etat;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetByImmatriculation(vehicule.Immatriculation);

                    return null;
                }
            }
        }

        public bool Update(Vehicule vehicule)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    UPDATE VEHICULE
                    SET [modèle véhicule] = @modele,
                        [état] = @etat
                    WHERE [n°immatriculation] = @immat", connection))
                {
                    command.Parameters.Add("@modele", SqlDbType.VarChar, 50).Value = vehicule.ModeleVehicule;
                    command.Parameters.Add("@etat", SqlDbType.Bit).Value = vehicule.Etat;
                    command.Parameters.Add("@immat", SqlDbType.VarChar, 9).Value = vehicule.Immatriculation;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            throw new NotSupportedException("Utiliser DeleteByImmatriculation(string immat) pour cette entité.");
        }

        public bool DeleteByImmatriculation(string immatriculation)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM VEHICULE WHERE [n°immatriculation] = @immat", connection))
                {
                    command.Parameters.Add("@immat", SqlDbType.VarChar, 9).Value = immatriculation;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
