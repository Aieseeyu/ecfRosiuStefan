using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Business;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelesController : ControllerBase
    {
        private readonly ModeleBusiness _business;

        public ModelesController(IConfiguration configuration)
        {
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            ModeleRepository repo = new ModeleRepository(factory);
            _business = new ModeleBusiness(repo);
        }

        // GET: api/modeles
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _business.GetAll();
            return Ok(data);
        }

        // GET: api/modeles/Clio%202
        [HttpGet("{modele}")]
        public IActionResult GetByModele(string modele)
        {
            var m = _business.GetByModele(modele);
            if (m == null) return NotFound();
            return Ok(m);
        }

        // POST: api/modeles
        [HttpPost]
        public IActionResult Create([FromBody] Modele model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);
            // la clé est le texte du modèle
            return CreatedAtAction(nameof(GetByModele), new { modele = data!.ModeleVehicule }, data);
        }

        // PUT: api/modeles/Clio%202
        [HttpPut("{modele}")]
        public IActionResult Update(string modele, [FromBody] Modele model)
        {
            if (model == null) return BadRequest("corps vide");
            if (!string.Equals(modele, model.ModeleVehicule, System.StringComparison.Ordinal))
                return BadRequest("clé route != clé corps");

            var (ok, error) = _business.Update(model);
            if (!ok) return BadRequest(error ?? "mise à jour impossible");
            return NoContent();
        }

        // DELETE: api/modeles/Clio%202
        [HttpDelete("{modele}")]
        public IActionResult Delete(string modele)
        {
            var (ok, error) = _business.Delete(modele);
            if (!ok)
            {
                if (error == "modèle introuvable") return NotFound();
                return BadRequest(error ?? "suppression impossible");
            }
            return NoContent();
        }
    }
}
