using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var employee = await _service.GetOne(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Hire([FromBody] Employee emp)
        {
            var result = await _service.Hire(emp);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { result.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Employee emp)
        {
            var result = await _service.Edit(id, emp);
            if (result.NotFound)
                return NotFound();
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(int id, [FromBody] TransferRequest request)
        {
            var result = await _service.Transfer(id, request);
            if (result.NotFound)
                return NotFound();
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("{id}/fire")]
        public async Task<IActionResult> Fire(int id, [FromBody] FireRequest request)
        {
            var result = await _service.Fire(id, request);
            if (result.NotFound)
                return NotFound();
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("{id}/photo")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            var result = await _service.UploadPhoto(id, file);
            if (result.NotFound)
                return NotFound();
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { PhotoPath = result.Value });
        }
    }
}