using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Business;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoniteursController : ControllerBase
    {
        private readonly MoniteurBusiness _business;

        public MoniteursController(IConfiguration configuration)
        {
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            MoniteurRepository repo = new MoniteurRepository(factory);
            _business = new MoniteurBusiness(repo);
        }

        // GET: api/moniteurs
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _business.GetAll();
            return Ok(data);
        }

        // GET: api/moniteurs/4
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var m = _business.GetById(id);
            if (m == null) return NotFound();
            return Ok(m);
        }

        // POST: api/moniteurs
        // NB : [id moniteur] n'est pas identity -> doit être fourni
        [HttpPost]
        public IActionResult Create([FromBody] Moniteur model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetById), new { id = data!.IdMoniteur }, data);
        }

        // PUT: api/moniteurs/4
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Moniteur model)
        {
            if (model == null) return BadRequest("corps vide");
            if (id != model.IdMoniteur) return BadRequest("id route != id corps");
            var (ok, error) = _business.Update(model);
            if (!ok) return BadRequest(error ?? "mise à jour impossible");
            return NoContent();
        }

        // DELETE: api/moniteurs/4
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var (ok, error) = _business.Delete(id);
            if (!ok)
            {
                if (error == "moniteur introuvable") return NotFound();
                return BadRequest(error ?? "suppression impossible");
            }
            return NoContent();
        }
    }
}
