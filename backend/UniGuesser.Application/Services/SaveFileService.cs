using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.Services.SaveFileService
{

    public interface ISaveFileService
    {
        Task<string> SaveFile(IFormFile imageFile);
    }

    public class SaveFileService(IConfiguration configuration) : ISaveFileService
    {
        private string folderPath = configuration["FileSaveData:SaveFolder"];
        private string serverIp = configuration["FileSaveData:ServerIp"];

        public async Task<string> SaveFile(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return  $"{serverIp}/{folderPath}/{uniqueFileName}";

        }
    }
}
