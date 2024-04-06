using CoreLayer.Interfaces;
using Domain.DTOs;
using Utils.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Utils.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Utils.Statics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CoreLayer.Extensions;

namespace Application.Services
{
    public class FileService
    {
        private readonly ICoreService<Domain.Models.File> CoreService;
        private readonly AuthUtilService AuthUtilService;
        private readonly IWebHostEnvironment WebHostEnvironment;
        public FileService(
            ICoreService<Domain.Models.File> coreService,
            AuthUtilService authUtilService,
            IWebHostEnvironment webHostEnvironment)
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
            WebHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFile(FileDTO file, DateTime Now, string? EntityType, string? EntityPK, string? fileName)
        {
            if (file.FormFile == null || file.FormFile.Length <= 0)
            {
                throw new ServiceException("Error while uploading file! File can't be null");
            }

            string ContentRoot = WebHostEnvironment.ContentRootPath;

            string fileDirectory = (string.IsNullOrWhiteSpace(EntityType))
                ? Path.Combine(ContentRoot, "StaticFiles")
                : Path.Combine(
                        ContentRoot,
                        "StaticFiles",
                        EntityType,
                        $"{EntityType}-{EntityPK ?? "unKnown"}"
                    );

            string FilePath = Path.Combine(
                    fileDirectory,
                    $"{fileName ?? "NA"}-{Now.ToString("yyyy-dd-M--HH-mm-ss")}{System.IO.Path.GetExtension(file.FormFile.FileName)}"
                );

            Directory.CreateDirectory(fileDirectory);

            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                file.FormFile.CopyTo(fileStream);
            }

            return 
                await GetFileUrl(FilePath);
        }

        public async Task UploadProjectFile(FileDTO file, string? fileName, long ProjectId)
        {
            var UserId = AuthUtilService.getUserId();
            var UserRole = AuthUtilService.GetUserRole();


            var project = await CoreService.Table<Project>()
                .FirstOrDefaultAsync(p => p.Id == ProjectId);

            if (UserRole != Policy.SuperAdmin && UserRole != Policy.Admin)  // Only Admin can upload file for project(Project Status and transitions)
            {
                throw new ServiceException("You might now have access to upload file for this project");
            }

            if (project == null)
            {
                throw new ServiceException("Project Not Found");
            }

            var url = await UploadFile(
                file,
                DateTime.Now,
                nameof(Project),
                ProjectId.ToString(),
                fileName
                );

            var File = new Domain.Models.File()
            {
                FileName = fileName ?? string.Empty,
                EntityType = nameof(Project),
                EntityId = ProjectId,
                Url = url
            };

            await CoreService.Create(File);
        }

        public async Task UploadWFTransitionFile(FileDTO file, string? fileName, long transitionId)
        {
            var UserId = AuthUtilService.getUserId();
            var UserRole = AuthUtilService.GetUserRole();


            var WFTransition = await CoreService.Table<Wftransition>()
                .FirstOrDefaultAsync(p => p.Id == transitionId)
                    ?? throw new ServiceException("وضعیت یافت نشد");

            if (!UserRole?.IsAdminOrSuperAdmin() == true)  // Only Admin can upload file for project
            {
                throw new ServiceException("شما دسترسی لازم برای آپلود فایل این پروژه را ندارید");
            }

            var url = await UploadFile(
                file,
                DateTime.Now,
                nameof(Wftransition),
                transitionId.ToString(),
                fileName
                );

            var File = new Domain.Models.File()
            {
                FileName = fileName ?? string.Empty,
                EntityType = nameof(Wftransition),
                EntityId = transitionId,
                Url = url
            };

            await CoreService.Create(File);
        }

        public async Task<string> GetFileUrl(string originUrl)
        {
            var normalized = originUrl.Replace("\\", "/");
            var index = normalized.IndexOf("StaticFiles/");

            if (index == -1)
            {
                throw new Exception("Url Format invalid");
            }

            return
                normalized.Substring(index + "StaticFiles/".Length);

        }

        public async Task<IResult> ServeFile(string fileUrl)
        {

            string ContentRoot = WebHostEnvironment.ContentRootPath;

            fileUrl = fileUrl.Replace("\\", "/");
            fileUrl = fileUrl.Replace("%2F", "/");

            fileUrl = WebUtility.UrlDecode(fileUrl);


            var filePath = Path.Combine(ContentRoot, "StaticFiles", fileUrl);

            var fileDownLoadName = Path.GetFileName(filePath);
            //var index = filePath.LastIndexOf("/");
            //var fileDownLoadName = filePath.Substring(index + 1);

            if (!System.IO.File.Exists(filePath))
            {
                Dictionary<object, object> items = new Dictionary<object, object>
                {
                    { "fileUrl", fileUrl },
                    { "filePath", filePath },
                    { "ContentRoot", ContentRoot },
                    { "fileDownLoadName", fileDownLoadName }

                };
                throw new ServiceException("File not found", items: items, code: 400);
            }

            return
                TypedResults.PhysicalFile(filePath, fileDownloadName: $"{fileDownLoadName}");

        }

        public async Task<List<FileDTO>> GetProjectFiles(long ProjectId = 0)
        {
            var Files = await CoreService.Table()
                .Where(f => f.EntityType == nameof(Project) && f.EntityId == ProjectId)
                .ToListAsync();

            return Files.Select(f => new FileDTO()
            {
                Id = ProjectId ,
                Url = f.Url
            })
            .ToList();
        }

        public async Task<List<FileDTO>> GetEntityFilesDto(string EntityType, long EntityId)
        {
            var Files = await CoreService.Table()
                .Where(f => f.EntityType == EntityType && f.EntityId == EntityId)
                .ToListAsync();

            return Files.Select(f => new FileDTO()
            {
                Id = EntityId,
                Url = f.Url
            })
            .ToList();
        }

        public async Task<List<Domain.Models.File>> GetEntityFiles(string EntityType, long EntityId)
        {
            var Files = await CoreService.Table()
                .Where(f => f.EntityType == EntityType && f.EntityId == EntityId)
                .ToListAsync();

            return Files;
        }

    }
}
