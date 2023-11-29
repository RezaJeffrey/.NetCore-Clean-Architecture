using Application.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebFater.Controllers
{
    [ApiController]
    [Route("Role")]
    public class RoleTestController : ControllerBase
    {
        private TestRoleService _roleService;
        public RoleTestController(TestRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("getRoles")]
        public async Task<ActionResult<List<Role>>> getRoles()
        {

            var result = await _roleService.GetRolesTest();
            return Ok(result);

        }

        [HttpGet("GetRoleById")]
        public async Task<ActionResult<List<Role>>> GetRole(long Id)
        {

            var result = await _roleService.GetRoleTest(Id);
            return Ok(result);

        }

        [HttpGet("GetUserRoles")]
        public async Task<ActionResult<List<UserRole>>> GetUserRoles()
        {

            var result = await _roleService.GetUserRolesTest();
            return Ok(result);

        }

        [HttpPost("AddRole")]
        public async Task<ActionResult> AddRole(RoleDTO role)
        {

            await _roleService.CreateRole(role);
            return Ok("successfully created");

        }

        [HttpDelete("DeleteRole")]
        public async Task<ActionResult> DeleteRole(RoleDTO role)
        {
            await _roleService.DeleteRoleByDTO(role);
            return Ok("successfully deleted");
        }
    }
}
