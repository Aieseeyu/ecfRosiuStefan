using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Business;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElevesController : ControllerBase
    {
        private readonly EleveBusiness _business;

        public ElevesController(IConfiguration configuration)
        {
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            EleveRepository repo = new EleveRepository(factory);
            _business = new EleveBusiness(repo);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var e = _business.GetById(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Eleve model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetById), new { id = data!.IdEleve }, data);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Eleve model)
        {
            if (model == null) return BadRequest("corps vide");
            if (id != model.IdEleve) return BadRequest("id route != id corps");
            var (ok, error) = _business.Update(model);
            if (!ok) return BadRequest(error);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var (ok, error) = _business.Delete(id);
            if (!ok)
            {
                if (error == "élève introuvable") return NotFound();
                return BadRequest(error);
            }
            return NoContent();
        }
    }
}
