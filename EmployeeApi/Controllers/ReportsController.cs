using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ReportFilter filter)
        {
            var result = await _service.Build(filter);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}