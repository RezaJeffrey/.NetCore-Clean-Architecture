using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Statics;

namespace Web.Controllers
{
    [ApiController]
    [Route("File")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;
        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }



        /// <remarks>
        /// <strong>Role</strong>: Agent - Customer (will be changed) <br/>
        /// <strong>Description</strong>: 
        ///     pass the fileUrl which should be retrieved from Database 
        /// </remarks>
        /// <param name="fileUrl"></param>
        [HttpGet("StaticFiles")]
        [Authorize(Policy = Policy.CustomerOrAgent)]
        // TODO : permission for this should be checked Inside serve file after defining all roles,
        // also this api should be less generic later to check who is accessing what file
        public async Task<IResult> GetFile(string fileUrl)
        {
            return 
                await _fileService.ServeFile(fileUrl);
        }

        // TODO: Update EntityType values
        /// <remarks>
        /// <strong>Role</strong>: Customer <br/>
        /// <strong>Description</strong>: 
        ///     Get related files to the Entity. <br/>
        ///     Available EntityTypes: <strong>(Project, Wftransition)</strong>
        /// </remarks>
        /// <param name="EntityType"></param>
        /// <param name="EntityId"></param>
        [HttpGet("GetEntityFiles")]
        [Authorize(Policy = Policy.Customer)]
        public async Task<List<FileDTO>> GetEntityFiles(string EntityType, long EntityId)
        {
            return
                await _fileService.GetEntityFilesDto(EntityType, EntityId);
        }
    }
}
