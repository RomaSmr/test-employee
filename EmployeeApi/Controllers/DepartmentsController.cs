using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department dep)
        {
            var result = await _service.Create(dep);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { result.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Department dep)
        {
            var result = await _service.Edit(id, dep);
            if (result.NotFound)
                return NotFound();
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var result = await _service.Close(id);
            if (result.NotFound)
                return NotFound();

            return Ok();
        }
    }
}