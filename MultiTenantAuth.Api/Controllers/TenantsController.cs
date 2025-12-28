using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Application.DTOs;
using MultiTenantAuth.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MultiTenantAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("types")]
        public async Task<IActionResult> CreateTenantType([FromBody] CreateTenantTypeDto dto)
        {
            try
            {
                var result = await _tenantService.CreateTenantTypeAsync(dto.Name, dto.AllowedParentTypeId);
                return Ok(result);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTenantTypes()
        {
            var result = await _tenantService.GetAllTenantTypesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
        {
            try
            {
                var result = await _tenantService.CreateTenantAsync(dto.Name, dto.TenantTypeId, dto.ParentTenantId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var result = await _tenantService.GetAllTenantsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenant(Guid id)
        {
            var result = await _tenantService.GetTenantByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
