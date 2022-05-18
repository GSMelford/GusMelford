using System.IO.Compression;
using GusMelfordBot.Core.Domain.System;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.System;

public class DataLakeService : IDataLakeService
{
    private readonly ILogger<DataLakeService> _logger;
    private const string DATA_LAKE_PATH = @"datalake";

    public DataLakeService(ILogger<DataLakeService> logger)
    {
        _logger = logger;
    }
    
    public async Task<bool> Write(string path, byte[] bytes)
    {
        try
        {
            await File.WriteAllBytesAsync(Path.Combine(DATA_LAKE_PATH, path), bytes);
            _logger.LogInformation("File saved by DateLake {Path} Size: {Size}", path, bytes.Length);
            return true;
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("Error writing file {Path} Error Message: {ErrorMessage}",path, e.Message);
        }
        
        return false;
    }
    
    public async Task<byte[]?> ReadAndDelete(string path)
    {
        byte[]? bytes = await Read(path);
        Delete(path);
        return bytes;
    }
    
    public async Task<byte[]?> Read(string path)
    {
        try
        {
            return await File.ReadAllBytesAsync(Path.Combine(DATA_LAKE_PATH, path));
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("Error reading file {Path} Error Message: {ErrorMessage}",path, e.Message);
            return null;
        }
    }

    public void Delete(string path)
    {
        try
        {
            File.Delete(Path.Combine(DATA_LAKE_PATH, path));
            _logger.LogInformation("Deleted a file {Path}", path);
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("Error when deleting a file {Path} Error Message: {ErrorMessage}",path, e.Message);
        }
    }
    
    public MemoryStream Archive(string folderPath, string fileName)
    {
        folderPath = Path.Combine(DATA_LAKE_PATH, folderPath);
        string zipPath = Path.Combine(DATA_LAKE_PATH, fileName);
        
        ZipFile.CreateFromDirectory(folderPath, zipPath);
        return new MemoryStream(File.ReadAllBytes(zipPath));
    }  
}