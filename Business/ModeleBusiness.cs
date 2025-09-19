using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class ModeleBusiness
    {
        private readonly ModeleRepository _repo;

        public ModeleBusiness(ModeleRepository repo)
        {
            _repo = repo;
        }

        private static string? Validate(Modele m)
        {
            if (m == null) return "objet modèle manquant";
            if (string.IsNullOrWhiteSpace(m.ModeleVehicule)) return "modèle véhicule requis";
            if (m.ModeleVehicule.Length > 50) return "modèle véhicule trop long (50 max)";

            if (string.IsNullOrWhiteSpace(m.Marque)) return "marque requise";
            if (m.Marque.Length > 50) return "marque trop longue (50 max)";

            if (string.IsNullOrWhiteSpace(m.Annee)) return "année requise";
            if (m.Annee.Length != 4 || !Regex.IsMatch(m.Annee, @"^\d{4}$")) return "année invalide (AAAA)";

            DateTime min = new DateTime(1990, 1, 1);
            if (m.DateAchat < min || m.DateAchat > DateTime.Today.AddDays(1)) return "date d'achat invalide";

            return null;
        }

        public List<Modele> GetAll() => _repo.GetAll();

        public Modele? GetByModele(string modele)
        {
            if (string.IsNullOrWhiteSpace(modele)) return null;
            return _repo.GetByModele(modele);
        }

        public (Modele? data, string? error) Create(Modele m)
        {
            string? err = Validate(m);
            if (err != null) return (null, err);

            if (_repo.GetByModele(m.ModeleVehicule) != null)
                return (null, "un modèle avec ce nom existe déjà");

            Modele? inserted = _repo.Add(m);
            return inserted == null ? (null, "échec insertion") : (inserted, null);
        }

        public (bool ok, string? error) Update(Modele m)
        {
            string? err = Validate(m);
            if (err != null) return (false, err);

            if (_repo.GetByModele(m.ModeleVehicule) == null)
                return (false, "modèle introuvable");

            bool ok = _repo.Update(m);
            return (ok, ok ? null : "échec mise à jour");
        }

        public (bool ok, string? error) Delete(string modele)
        {
            if (string.IsNullOrWhiteSpace(modele)) return (false, "clé invalide");
            if (_repo.GetByModele(modele) == null) return (false, "modèle introuvable");

            bool ok = _repo.DeleteByModele(modele);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
