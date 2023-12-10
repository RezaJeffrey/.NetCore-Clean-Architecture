using Application.Services;
using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public RoleTestController(TestRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("getRoles")]
        [AllowAnonymous]
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


        [HttpGet("CheckHandler")]
        [AllowAnonymous]
        public void CheckHandlers()
        {
            string requiredRole = "2";
            var check = _roleService.CheckHandler(requiredRole);
        }

        [HttpGet("CheckHandlerAuth")]
        [Authorize(Policy = "RequireRole2")]
        //[Authorize]
        public void CheckHandlerauth()
        {
            string requiredRole = "2";
            var check = _roleService.CheckHandler(requiredRole);
        }

        [HttpGet("GetToken")]
        [AllowAnonymous]
        public void GetAccessToken()
        {
            User? userdb = _roleService.CoreService.Table<User>()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefault(user => user.Id == 1);
            
            var userRoles = userdb?.UserRoles
                .Where(ur => ur.DeleteDate == 0 || ur.DeleteDate == null)
                .Select(ur => ur.Role)
                .Where(r => r.DeleteDate == null || r.DeleteDate == 0)
                .ToList();
            if (userRoles == null) throw new AppRuleException();
            if (userdb == null) throw new AppRuleException();
            //TODO  return _authenticateService.GetAccessToken(userdb, userRoles) + GetRefreshToken()
        }

        [HttpGet("CheckQueryFilter")]
        [AllowAnonymous]
        public void CheckQuery()
        {
            _roleService.CheckQueryFilter();
        } 

        [HttpGet("CheckExpiryDate")]
        [Authorize]
        public async Task CheckExpiryDate()
        {
        }

    }
}
