using Microsoft.AspNetCore.Mvc;
using ECFautoecole.Data;
using ECFautoecole.Repositories;
using ECFautoecole.Business;
using ECFautoecole.Models;

namespace ECFautoecole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendriersController : ControllerBase
    {
        private readonly CalendrierBusiness _business;

        public CalendriersController(IConfiguration configuration)
        {
            SqlConnectionFactory factory = new SqlConnectionFactory(configuration);
            CalendrierRepository repo = new CalendrierRepository(factory);
            _business = new CalendrierBusiness(repo);
        }

        // GET: api/calendriers
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _business.GetAll();
            return Ok(data);
        }

        // GET: api/calendriers/2023-01-13
        [HttpGet("{date:datetime}")]
        public IActionResult GetByDate(DateTime date)
        {
            var c = _business.GetByDate(date);
            if (c == null) return NotFound();
            return Ok(c);
        }

        // POST: api/calendriers
        // Body: { "dateHeure": "2023-01-20" }
        [HttpPost]
        public IActionResult Create([FromBody] Calendrier model)
        {
            if (model == null) return BadRequest("corps vide");
            var (data, error) = _business.Create(model);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetByDate),
                new { date = data!.DateHeure.ToString("yyyy-MM-dd") }, data);
        }

        // DELETE: api/calendriers/2023-01-20
        [HttpDelete("{date:datetime}")]
        public IActionResult Delete(DateTime date)
        {
            var (ok, error) = _business.Delete(date);
            if (!ok)
            {
                if (error == "date introuvable") return NotFound();
                return BadRequest(error ?? "suppression impossible");
            }
            return NoContent();
        }
    }
}
