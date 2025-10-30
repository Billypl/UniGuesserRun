using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UniGuesser.Application.Services;

public interface IFileService
{
    Task<string> SaveFile(IFormFile imageFile);
    Task DeleteFile(string fileUrl);
}

public class FileService(IConfiguration configuration) : IFileService
{
    private readonly string folderPath = configuration["FileSaveData:SaveFolder"];
    private readonly string serverIp = configuration["FileSaveData:ServerIp"];

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

        return $"{serverIp}/{folderPath}/{uniqueFileName}";
    }

    public async Task DeleteFile(string fileUrl)
    {
        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);
        var filePath = Path.Combine(uploadsFolder, fileName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}