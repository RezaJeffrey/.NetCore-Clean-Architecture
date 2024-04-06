using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Statics;

namespace Web.Controllers
{
    [ApiController]
    [Route("Users")]
    [Authorize]
    public class UserController
    {
        private readonly UserService UserService;
        public UserController(UserService userService)
        {
            UserService = userService;
        }


        [HttpGet("BindAgents")]
        [Authorize(Policy = Policy.Admin)]
        public async Task<List<UserDTO>> BindBrokers()
        {
            var users = await UserService.BindAgents();
            return users;
        }

        [HttpGet("BindAgentsPaging")]
        [Authorize(Policy = Policy.Admin)]
        public async Task<GridData<UserDTO>> BindAgentsPaging([FromQuery] GridData<UserDTO>? grid)
        {
            if (grid == null)
                grid = new GridData<UserDTO>();

            var paginated_agents = await UserService.AgentsToPaging(grid);
            return paginated_agents;
        }

        [HttpGet("BindUsers")]
        [Authorize(Policy = Policy.SuperAdmin)]
        public async Task<List<UserDTO>> BindUsers()
        {
            var users = await UserService.BindAllUsers();
            return users;
        }

        [HttpGet("BindUsersPaging")]
        [Authorize(Policy = Policy.SuperAdmin)]
        public async Task<GridData<UserDTO>> BindUsersPaging([FromQuery] GridData<UserDTO>? grid)
        {
            if (grid == null) 
                grid = new GridData<UserDTO>();

            var paginated_users = await UserService.UsersToPaging(grid);
            return paginated_users;
        }

        [HttpGet("BindSubUsers")]
        [Authorize(Policy = Policy.Agent)]
        public async Task<List<UserDTO>> BindSubUsers(long UserId)
        {
            return await UserService.BindSubUsers(UserId);
        }


    }
}
