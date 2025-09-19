using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class VehiculeBusiness
    {
        private readonly VehiculeRepository _repo;

        public VehiculeBusiness(VehiculeRepository repo)
        {
            _repo = repo;
        }

        private static string? Validate(Vehicule v)
        {
            if (v == null) return "objet véhicule manquant";
            if (string.IsNullOrWhiteSpace(v.Immatriculation)) return "immatriculation requise";
            if (v.Immatriculation.Length > 9) return "immatriculation trop longue (9 max)";

            if (string.IsNullOrWhiteSpace(v.ModeleVehicule)) return "modèle véhicule requis";
            if (v.ModeleVehicule.Length > 50) return "modèle véhicule trop long (50 max)";

            // (Optionnel) motif simple FR (tolérant aux espaces)
            // if (!Regex.IsMatch(v.Immatriculation, @"^[A-Z0-9 ]{1,9}$")) return "immatriculation invalide";
            return null;
        }

        public List<Vehicule> GetAll() => _repo.GetAll();

        public Vehicule? GetByImmatriculation(string immat)
        {
            if (string.IsNullOrWhiteSpace(immat)) return null;
            return _repo.GetByImmatriculation(immat);
        }

        public (Vehicule? data, string? error) Create(Vehicule v)
        {
            string? err = Validate(v);
            if (err != null) return (null, err);

            if (_repo.GetByImmatriculation(v.Immatriculation) != null)
                return (null, "un véhicule avec cette immatriculation existe déjà");

            Vehicule? inserted = _repo.Add(v);
            return inserted == null ? (null, "échec insertion") : (inserted, null);
        }

        public (bool ok, string? error) Update(Vehicule v)
        {
            string? err = Validate(v);
            if (err != null) return (false, err);

            if (_repo.GetByImmatriculation(v.Immatriculation) == null)
                return (false, "véhicule introuvable");

            bool ok = _repo.Update(v);
            return (ok, ok ? null : "échec mise à jour");
        }

        public (bool ok, string? error) Delete(string immat)
        {
            if (string.IsNullOrWhiteSpace(immat)) return (false, "clé invalide");
            if (_repo.GetByImmatriculation(immat) == null) return (false, "véhicule introuvable");

            bool ok = _repo.DeleteByImmatriculation(immat);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
