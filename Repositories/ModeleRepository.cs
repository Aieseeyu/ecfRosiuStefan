using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECFautoecole.Interfaces;
using ECFautoecole.Models;
using ECFautoecole.Data;

namespace ECFautoecole.Repositories
{
    public class ModeleRepository : IRepository<Modele>
    {
        private readonly SqlConnectionFactory _factory;

        public ModeleRepository(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Modele> GetAll()
        {
            List<Modele> modeles = new List<Modele>();

            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM MODELE", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Modele modele = new Modele
                            {
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                Marque = reader["marque"].ToString() ?? string.Empty,
                                Annee = reader["année"].ToString() ?? string.Empty,
                                DateAchat = (DateTime)reader["date achat"]
                            };

                            modeles.Add(modele);
                        }
                    }
                }
            }

            return modeles;
        }

        public Modele? GetById(int id)
        {
            // PK n'est pas un int pour MODELE
            throw new NotSupportedException("Utiliser GetByModele(string modeleVehicule) pour cette entité.");
        }

        // Méthode spécifique pour clé texte
        public Modele? GetByModele(string modeleVehicule)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM MODELE WHERE [modèle véhicule] = @m", connection))
                {
                    command.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modeleVehicule;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Modele
                            {
                                ModeleVehicule = reader["modèle véhicule"].ToString() ?? string.Empty,
                                Marque = reader["marque"].ToString() ?? string.Empty,
                                Annee = reader["année"].ToString() ?? string.Empty,
                                DateAchat = (DateTime)reader["date achat"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Modele? Add(Modele modele)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO MODELE ([modèle véhicule],[marque],[année],[date achat])
                    VALUES (@m,@marque,@annee,@achat);", connection))
                {
                    command.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modele.ModeleVehicule;
                    command.Parameters.Add("@marque", SqlDbType.VarChar, 50).Value = modele.Marque;
                    command.Parameters.Add("@annee", SqlDbType.NChar, 4).Value = modele.Annee;
                    command.Parameters.Add("@achat", SqlDbType.Date).Value = modele.DateAchat.Date;

                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                        return GetByModele(modele.ModeleVehicule);

                    return null;
                }
            }
        }

        public bool Update(Modele modele)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"
                    UPDATE MODELE
                    SET [marque] = @marque,
                        [année] = @annee,
                        [date achat] = @achat
                    WHERE [modèle véhicule] = @m", connection))
                {
                    command.Parameters.Add("@marque", SqlDbType.VarChar, 50).Value = modele.Marque;
                    command.Parameters.Add("@annee", SqlDbType.NChar, 4).Value = modele.Annee;
                    command.Parameters.Add("@achat", SqlDbType.Date).Value = modele.DateAchat.Date;
                    command.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modele.ModeleVehicule;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            // PK n'est pas un int pour MODELE
            throw new NotSupportedException("Utiliser DeleteByModele(string modeleVehicule) pour cette entité.");
        }

        // Méthode spécifique pour suppression
        public bool DeleteByModele(string modeleVehicule)
        {
            using (SqlConnection connection = _factory.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM MODELE WHERE [modèle véhicule] = @m", connection))
                {
                    command.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modeleVehicule;

                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
