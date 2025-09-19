using System;
using System.Collections.Generic;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class MoniteurBusiness
    {
        private readonly MoniteurRepository _repo;

        public MoniteurBusiness(MoniteurRepository repo)
        {
            _repo = repo;
        }

        private static string? Validate(Moniteur m)
        {
            if (m == null) return "objet moniteur manquant";
            if (m.IdMoniteur <= 0) return "id moniteur doit être > 0";
            if (string.IsNullOrWhiteSpace(m.NomMoniteur)) return "nom moniteur requis";
            if (m.NomMoniteur.Length > 50) return "nom moniteur trop long (50 max)";
            if (string.IsNullOrWhiteSpace(m.PrenomMoniteur)) return "prénom moniteur requis";
            if (m.PrenomMoniteur.Length > 50) return "prénom moniteur trop long (50 max)";

            DateTime min = new DateTime(1900, 1, 1);
            if (m.DateNaissance < min || m.DateNaissance > DateTime.Today) return "date de naissance invalide";
            return null;
        }

        public List<Moniteur> GetAll() => _repo.GetAll();

        public Moniteur? GetById(int id)
        {
            if (id <= 0) return null;
            return _repo.GetById(id);
        }

        public (Moniteur? data, string? error) Create(Moniteur m)
        {
            string? err = Validate(m);
            if (err != null) return (null, err);

            if (_repo.GetById(m.IdMoniteur) != null)
                return (null, "un moniteur avec cet id existe déjà");

            Moniteur? inserted = _repo.Add(m);
            return inserted == null ? (null, "échec insertion") : (inserted, null);
        }

        public (bool ok, string? error) Update(Moniteur m)
        {
            string? err = Validate(m);
            if (err != null) return (false, err);

            if (_repo.GetById(m.IdMoniteur) == null)
                return (false, "moniteur introuvable");

            bool ok = _repo.Update(m);
            return (ok, ok ? null : "échec mise à jour");
        }

        public (bool ok, string? error) Delete(int id)
        {
            if (id <= 0) return (false, "id invalide");
            if (_repo.GetById(id) == null) return (false, "moniteur introuvable");

            bool ok = _repo.Delete(id);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
