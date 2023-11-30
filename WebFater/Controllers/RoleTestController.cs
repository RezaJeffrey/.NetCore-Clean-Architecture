using Application.Services;
using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;
using Utils.Mappings;

namespace WebFater.Controllers
{
    [ApiController]
    [Route("Role")]
    [Authorize]
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
        [Authorize]
        public async Task<List<RoleDTO>> GetUserRoles(long Id)
        {
            var userRoles = await _authenticateService.FetchUserRoles(Id);
            // Test Authentication
            User? user = await _authenticateService.GetUserById(1);
            if (user == null) throw new AppRuleException("user not found");

            var token = _authenticateService.CreateToken(user, userRoles);
            var roles = User.FindAll(a => a.Type == "Role");
            var role = roles.FirstOrDefault()?.Value;
            
            //
            
            return ObjectMapper.MapList<Role, RoleDTO>(userRoles);
        }
    }
}
