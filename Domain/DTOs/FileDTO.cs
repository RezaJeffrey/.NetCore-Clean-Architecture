using Microsoft.AspNetCore.Http;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class FileDTO
    {
        public long? Id { get; set; }

        public IFormFile? FormFile { get; set; }

        public string? Url { get; set; }

        public string? Name { get; set; } = string.Empty;

        public static implicit operator FileDTO(Domain.Models.File file)
        {
            return new FileDTO()
            {
                Id = file.Id,
                Url = file.Url,
                Name = file.FileName
            };
        }
    }
}
