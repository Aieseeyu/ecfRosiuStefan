using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Business;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculesController : ControllerBase
    {
        private readonly VehiculeBusiness _business;

        public VehiculesController(IConfiguration configuration)
        {
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            VehiculeRepository repo = new VehiculeRepository(factory);
            _business = new VehiculeBusiness(repo);
        }

        // GET: api/vehicules
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _business.GetAll();
            return Ok(data);
        }

        // GET: api/vehicules/1%20RTT%2006
        [HttpGet("{immat}")]
        public IActionResult GetByImmatriculation(string immat)
        {
            var v = _business.GetByImmatriculation(immat);
            if (v == null) return NotFound();
            return Ok(v);
        }

        // POST: api/vehicules
        [HttpPost]
        public IActionResult Create([FromBody] Vehicule model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetByImmatriculation),
                new { immat = data!.Immatriculation }, data);
        }

        // PUT: api/vehicules/1%20RTT%2006
        [HttpPut("{immat}")]
        public IActionResult Update(string immat, [FromBody] Vehicule model)
        {
            if (model == null) return BadRequest("corps vide");
            if (!string.Equals(immat, model.Immatriculation, System.StringComparison.Ordinal))
                return BadRequest("clé route != clé corps");

            var (ok, error) = _business.Update(model);
            if (!ok) return BadRequest(error ?? "mise à jour impossible");
            return NoContent();
        }

        // DELETE: api/vehicules/1%20RTT%2006
        [HttpDelete("{immat}")]
        public IActionResult Delete(string immat)
        {
            var (ok, error) = _business.Delete(immat);
            if (!ok)
            {
                if (error == "véhicule introuvable") return NotFound();
                return BadRequest(error ?? "suppression impossible");
            }
            return NoContent();
        }
    }
}
