using _991733370_IT_ASSET_API.Data;
using _991733370_IT_ASSET_API.DTOs;
using _991733370_IT_ASSET_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _991733370_IT_ASSET_API.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    [Authorize]
    public class EquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/equipment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipment = await _context.Equipment
                .Select(e => MapToResponse(e))
                .ToListAsync();

            return Ok(equipment);
        }

        // GET /api/equipment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null) return NotFound();

            return Ok(MapToResponse(equipment));
        }

        // POST /api/equipment — ITAdmin only
        [HttpPost]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var equipment = new Equipment
            {
                AssetTag = dto.AssetTag,
                DeviceName = dto.DeviceName,
                Description = dto.Description,
                IsAvailable = dto.IsAvailable
            };

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = equipment.EquipmentId }, MapToResponse(equipment));
        }

        // PUT /api/equipment/{id} — ITAdmin only
        [HttpPut("{id}")]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentDto dto)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null) return NotFound();

            if (dto.AssetTag != null) equipment.AssetTag = dto.AssetTag;
            if (dto.DeviceName != null) equipment.DeviceName = dto.DeviceName;
            if (dto.Description != null) equipment.Description = dto.Description;
            if (dto.IsAvailable.HasValue) equipment.IsAvailable = dto.IsAvailable.Value;

            await _context.SaveChangesAsync();
            return Ok(MapToResponse(equipment));
        }

        // DELETE /api/equipment/{id} — ITAdmin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null) return NotFound();

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Equipment removed." });
        }

        private static EquipmentResponse MapToResponse(Equipment e) => new EquipmentResponse
        {
            EquipmentId = e.EquipmentId,
            AssetTag = e.AssetTag,
            DeviceName = e.DeviceName,
            Description = e.Description,
            IsAvailable = e.IsAvailable
        };
    }
}
