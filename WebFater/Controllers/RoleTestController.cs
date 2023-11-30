using Application.Services;
using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utils.Mappings;

namespace WebFater.Controllers
{
    [ApiController]
    [Route("Role")]
    public class RoleTestController : ControllerBase
    {
        private TestRoleService _roleService;
        private AuthenticateService _authenticateService;
        public RoleTestController(TestRoleService roleService, AuthenticateService authenticateService)
        {
            _roleService = roleService;
            _authenticateService = authenticateService;
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

        [HttpGet("GetUserRolesById")]
        public async Task<List<RoleDTO>> GetUserRoles(long Id)
        {
            var userRoles = await _authenticateService.FetchUserRoles(Id);
            
            return ObjectMapper.MapList<Role, RoleDTO>(userRoles);
        }
    }
}
