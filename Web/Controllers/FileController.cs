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
        /// <strong>Role</strong>: Admin <br/>
        /// <strong>Description</strong>: 
        /// upload file , Enter optional fileName, lastly RequestId of the project which file is related to.
        /// </remarks>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="projectId"></param>
        [HttpPost("UploadProjectFile")]
        [Authorize(Policy = Policy.Admin)]
        public async Task<IActionResult> UploadProjectFile([FromForm] FileDTO file, string? fileName, long projectId = 0)
        {
            try
            {
                await _fileService.UploadProjectFile(file, fileName, projectId);
                return Ok("Uploaded");

            }
            catch (Exception)
            {
                throw;
                //return BadRequest();
            }

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

        /// <remarks>
        /// <strong>Role</strong>: Agent - Customer (will be changed) <br/>
        /// <strong>Description</strong>: 
        ///     pass RequestId of the Project to get associated files (Only the RequestId and Url of files will be send)
        /// </remarks>
        /// <param name="ProjectId"></param>
        [HttpGet("GetProjectFiles/{ProjectId}")]
        [Authorize(Policy = Policy.CustomerOrAgent)]
        public async Task<List<FileDTO>> GetProjectFiles(long ProjectId)
        {
            return
                await _fileService.GetProjectFiles(ProjectId);
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
