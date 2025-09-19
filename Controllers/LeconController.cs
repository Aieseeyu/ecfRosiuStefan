using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Business;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeconsController : ControllerBase
    {
        private readonly LeconBusiness _business;

        public LeconsController(IConfiguration configuration)
        {
            // instanciation simple (pas de DI)
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            var leconRepo = new LeconRepository(factory);
            var eleveRepo = new EleveRepository(factory);
            var moniteurRepo = new MoniteurRepository(factory);
            var modeleRepo = new ModeleRepository(factory);
            var calendrierRepo = new CalendrierRepository(factory);

            _business = new LeconBusiness(leconRepo, eleveRepo, moniteurRepo, modeleRepo, calendrierRepo);
        }

        // GET: api/lecons
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _business.GetAll();
            return Ok(data);
        }

        // GET: api/lecons/{modele}/{date}/{idEleve}/{idMoniteur}
        // ex: /api/lecons/Clio%202/2023-01-13/2/3
        [HttpGet("{modele}/{date:datetime}/{idEleve:int}/{idMoniteur:int}")]
        public IActionResult GetByKey(string modele, DateTime date, int idEleve, int idMoniteur)
        {
            var l = _business.GetByKey(modele, date, idEleve, idMoniteur);
            if (l == null) return NotFound();
            return Ok(l);
        }

        // POST: api/lecons
        // Body = objet Lecon (toutes les clés + durée)
        [HttpPost]
        public IActionResult Create([FromBody] Lecon model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetByKey),
                new { modele = data!.ModeleVehicule, date = data.DateHeure.ToString("yyyy-MM-dd"), idEleve = data.IdEleve, idMoniteur = data.IdMoniteur },
                data);
        }

        // PUT: api/lecons/{modele}/{date}/{idEleve}/{idMoniteur}
        // On n’autorise la modif que de la durée
        [HttpPut("{modele}/{date:datetime}/{idEleve:int}/{idMoniteur:int}")]
        public IActionResult Update(string modele, DateTime date, int idEleve, int idMoniteur, [FromBody] Lecon model)
        {
            if (model == null) return BadRequest("corps vide");

            // les clés route doivent matcher le corps
            if (!string.Equals(modele, model.ModeleVehicule, System.StringComparison.Ordinal) ||
                model.DateHeure.Date != date.Date ||
                model.IdEleve != idEleve ||
                model.IdMoniteur != idMoniteur)
            {
                return BadRequest("les clés de la route doivent correspondre au corps");
            }

            var (ok, error) = _business.Update(model);
            if (!ok) return BadRequest(error ?? "mise à jour impossible");
            return NoContent();
        }

        // DELETE: api/lecons/{modele}/{date}/{idEleve}/{idMoniteur}
        [HttpDelete("{modele}/{date:datetime}/{idEleve:int}/{idMoniteur:int}")]
        public IActionResult Delete(string modele, DateTime date, int idEleve, int idMoniteur)
        {
            var (ok, error) = _business.Delete(modele, date, idEleve, idMoniteur);
            if (!ok)
            {
                if (error == "leçon introuvable") return NotFound();
                return BadRequest(error ?? "suppression impossible");
            }
            return NoContent();
        }
    }
}
