using System;
using System.Collections.Generic;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class EleveBusiness
    {
        private readonly EleveRepository _repo;

        public EleveBusiness(EleveRepository repo)
        {
            _repo = repo;
        }

        // règles simples: champs obligatoires + longueurs max + date cohérente
        private static string? Validate(Eleve e)
        {
            if (e == null) return "objet élève manquant";
            if (e.IdEleve <= 0) return "id élève doit être > 0";
            if (string.IsNullOrWhiteSpace(e.NomEleve)) return "nom élève requis";
            if (e.NomEleve.Length > 50) return "nom élève trop long (50 max)";
            if (string.IsNullOrWhiteSpace(e.PrenomEleve)) return "prénom élève requis";
            if (e.PrenomEleve.Length > 50) return "prénom élève trop long (50 max)";
            // date de naissance raisonnable (entre 1900 et aujourd'hui)
            DateTime min = new DateTime(1900, 1, 1);
            if (e.DateNaissance < min || e.DateNaissance > DateTime.Today) return "date de naissance invalide";
            return null;
        }

        public List<Eleve> GetAll()
        {
            return _repo.GetAll();
        }

        public Eleve? GetById(int id)
        {
            if (id <= 0) return null;
            return _repo.GetById(id);
        }

        public (Eleve? data, string? error) Create(Eleve e)
        {
            string? err = Validate(e);
            if (err != null) return (null, err);

            // s’assurer qu’il n’existe pas déjà
            Eleve? existing = _repo.GetById(e.IdEleve);
            if (existing != null) return (null, "un élève avec cet id existe déjà");

            Eleve? inserted = _repo.Add(e);
            if (inserted == null) return (null, "échec insertion");
            return (inserted, null);
        }

        public (bool ok, string? error) Update(Eleve e)
        {
            string? err = Validate(e);
            if (err != null) return (false, err);

            Eleve? existing = _repo.GetById(e.IdEleve);
            if (existing == null) return (false, "élève introuvable");

            bool ok = _repo.Update(e);
            return (ok, ok ? null : "échec mise à jour");
        }

        public (bool ok, string? error) Delete(int id)
        {
            if (id <= 0) return (false, "id invalide");
            Eleve? existing = _repo.GetById(id);
            if (existing == null) return (false, "élève introuvable");

            bool ok = _repo.Delete(id);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
