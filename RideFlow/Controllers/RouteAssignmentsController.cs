using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideFlow.Core.Application.DTOs.Assignments;
using RideFlow.Core.Application.Interfaces;

namespace RideFlow.Core.Presentation.Controllers
{
    [ApiController]
    [Route("api/Assignments")]
    [Authorize(Roles = "Admin")]
    public class RouteAssignmentsController : ControllerBase
    {
        private readonly IRouteAssignmentService _service;

        public RouteAssignmentsController(IRouteAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "No se pudieron cargar las asignaciones.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result == null)
                    return NotFound("La asignación no fue encontrada.");

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "No se pudo cargar la asignación.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RouteAssignmentCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "No fue posible crear la asignación.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RouteAssignmentUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound("La asignación no fue encontrada.");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "No fue posible actualizar la asignación.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound("La asignación no fue encontrada.");

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "No fue posible eliminar la asignación.");
            }
        }
    }
}