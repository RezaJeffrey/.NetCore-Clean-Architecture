using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Statics;

namespace Web.Controllers
{
    [ApiController]
    [Route("Roles")]
    [Authorize]
    public class RoleController
    {
        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        ///     Get List of available roles
        /// </summary>
        /// <remarks>
        ///     <strong>Role: </strong> Anonymous <br/>
        ///     <strong>Description: </strong>
        ///         if user is Anonymous or not admin, won't see Admin and SuperAdmin roles
        /// </remarks>
        /// <returns></returns>
        [HttpGet("BindRoles")]
        [AllowAnonymous]
        public async Task<List<RoleDTO>> BindRoles()
        {
            return await _roleService.BindRoles();
        }

    }
}
