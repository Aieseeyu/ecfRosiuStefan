using System;
using System.Collections.Generic;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class LeconBusiness
    {
        private readonly LeconRepository _leconRepo;
        private readonly EleveRepository _eleveRepo;
        private readonly MoniteurRepository _moniteurRepo;
        private readonly ModeleRepository _modeleRepo;
        private readonly CalendrierRepository _calRepo;

        public LeconBusiness(
            LeconRepository leconRepo,
            EleveRepository eleveRepo,
            MoniteurRepository moniteurRepo,
            ModeleRepository modeleRepo,
            CalendrierRepository calRepo)
        {
            _leconRepo = leconRepo;
            _eleveRepo = eleveRepo;
            _moniteurRepo = moniteurRepo;
            _modeleRepo = modeleRepo;
            _calRepo = calRepo;
        }

        private string? Validate(Lecon l, bool checkExists)
        {
            if (l == null) return "objet leçon manquant";

            if (string.IsNullOrWhiteSpace(l.ModeleVehicule)) return "modèle véhicule requis";
            if (l.ModeleVehicule.Length > 50) return "modèle véhicule trop long (50 max)";

            if (l.IdEleve <= 0) return "id élève invalide";
            if (l.IdMoniteur <= 0) return "id moniteur invalide";
            if (l.Duree <= 0) return "durée doit être > 0";

            // FK et règles de base (selon ta BDD)
            if (_modeleRepo.GetByModele(l.ModeleVehicule) == null) return "modèle inconnu";
            if (_eleveRepo.GetById(l.IdEleve) == null) return "élève introuvable";
            var moniteur = _moniteurRepo.GetById(l.IdMoniteur);
            if (moniteur == null) return "moniteur introuvable";
            if (!moniteur.Activite) return "moniteur inactif";

            if (_calRepo.GetByDate(l.DateHeure.Date) == null) return "date absente du calendrier";

            if (checkExists)
            {
                var exists = _leconRepo.GetByKey(l.ModeleVehicule, l.DateHeure.Date, l.IdEleve, l.IdMoniteur);
                if (exists != null) return "leçon déjà existante (doublon de clé)";
            }

            return null;
        }

        public List<Lecon> GetAll() => _leconRepo.GetAll();

        public Lecon? GetByKey(string modele, DateTime date, int idEleve, int idMoniteur)
            => _leconRepo.GetByKey(modele, date.Date, idEleve, idMoniteur);

        public (Lecon? data, string? error) Create(Lecon l)
        {
            string? err = Validate(l, checkExists: true);
            if (err != null) return (null, err);

            Lecon? inserted = _leconRepo.Add(l);
            return inserted == null ? (null, "échec insertion") : (inserted, null);
        }

        public (bool ok, string? error) Update(Lecon l)
        {
            // pour Update, on valide mais on n’exige pas “non-existence” (c’est la même clé)
            string? err = Validate(l, checkExists: false);
            if (err != null) return (false, err);

            var current = _leconRepo.GetByKey(l.ModeleVehicule, l.DateHeure.Date, l.IdEleve, l.IdMoniteur);
            if (current == null) return (false, "leçon introuvable");

            bool ok = _leconRepo.Update(l); // met à jour uniquement la durée
            return (ok, ok ? null : "échec mise à jour");
        }

        public (bool ok, string? error) Delete(string modele, DateTime date, int idEleve, int idMoniteur)
        {
            var current = _leconRepo.GetByKey(modele, date.Date, idEleve, idMoniteur);
            if (current == null) return (false, "leçon introuvable");

            bool ok = _leconRepo.DeleteByKey(modele, date.Date, idEleve, idMoniteur);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
