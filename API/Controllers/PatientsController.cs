using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly InMemoryPatientRepository _repository;

        public PatientsController(InMemoryPatientRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Patient> GetById(int id)
        {
            var patient = _repository.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public ActionResult Add(Patient patient)
        {
            _repository.Add(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            _repository.Update(patient);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return NoContent();
        }

        [HttpGet("paged")]
        public ActionResult<IEnumerable<Patient>> GetPaged(int pageNumber, int pageSize, string? firstName, string? lastName, string? city, bool? active, string sortBy)
        {
            var patients = _repository.GetPaged(pageNumber, pageSize, firstName, lastName, city, active, sortBy);
            return Ok(patients);
        }
    }
}
