using Microsoft.AspNetCore.Mvc;
using MultiTenantAuth.Application.DTOs;
using MultiTenantAuth.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MultiTenantAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public RolesController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var result = await _permissionService.CreateRoleAsync(dto.Name, dto.Description, dto.TenantId,dto.IsSystemRole);
            return Ok(result);
        }

        [HttpPost("role-with-permissions")]
        public async Task<IActionResult> CreateRoleWithPermissions([FromBody] CreateRoleWithPermissionDto dto)
        {
            var result = await _permissionService.CreateRoleWithPermissionAsync(dto);
            return Ok(result);
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            var result = await _permissionService.CreatePermissionAsync(dto.Name, dto.Description);
            return Ok(result);
        }

        [HttpPost("assign-permission")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDto dto)
        {
            await _permissionService.AssignPermissionToRoleAsync(dto.RoleId, dto.PermissionId);
            return Ok();
        }

        [HttpPost("assign-permissions")]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionsDto dto)
        {
            await _permissionService.AssignPermissionsToRoleAsync(dto.RoleId, dto.PermissionIds);
            return Ok();
        }

        [HttpPost("assign-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto dto)
        {
            await _permissionService.AssignRoleToUserAsync(dto.UserId, dto.TenantId, dto.RoleId);
            return Ok();
        }

        [HttpGet("user-permissions")]
        public async Task<IActionResult> GetUserPermissions(Guid userId, Guid tenantId)
        {
            var result = await _permissionService.GetUserPermissionsAsync(userId, tenantId);
            return Ok(result);
        }
    }
}
