using System;
using System.Collections.Generic;
using ECFautoecole.Models;
using ECFautoecole.Repositories;

namespace ECFautoecole.Business
{
    public class CalendrierBusiness
    {
        private readonly CalendrierRepository _repo;

        public CalendrierBusiness(CalendrierRepository repo)
        {
            _repo = repo;
        }

        private static string? Validate(Calendrier c)
        {
            if (c == null) return "objet calendrier manquant";
            // bornes raisonnables
            DateTime min = new DateTime(2000, 1, 1);
            if (c.DateHeure < min) return "date trop ancienne";
            return null;
        }

        public List<Calendrier> GetAll() => _repo.GetAll();

        public Calendrier? GetByDate(DateTime date) => _repo.GetByDate(date.Date);

        public (Calendrier? data, string? error) Create(Calendrier c)
        {
            string? err = Validate(c);
            if (err != null) return (null, err);

            if (_repo.GetByDate(c.DateHeure.Date) != null)
                return (null, "cette date existe déjà dans le calendrier");

            Calendrier? inserted = _repo.Add(c);
            return inserted == null ? (null, "échec insertion") : (inserted, null);
        }

        public (bool ok, string? error) Delete(DateTime date)
        {
            var existing = _repo.GetByDate(date.Date);
            if (existing == null) return (false, "date introuvable");

            bool ok = _repo.DeleteByDate(date.Date);
            return (ok, ok ? null : "échec suppression");
        }
    }
}
