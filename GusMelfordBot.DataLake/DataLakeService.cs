using System;

namespace GusMelfordBot.DataLake;

public class DataLakeService : IDataLakeService
{
    public void CreateDirectoryIfNotExist(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
    
    public async Task WriteFile2(string path, byte[] bytes)
    {
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        await fs.WriteAsync(bytes);
    }
    
    public Task WriteFile(string path, byte[] bytes)
    {
        return File.WriteAllBytesAsync(path, bytes);
    }
    
    public Task<byte[]> ReadFileAsync(string path)
    {
        return File.ReadAllBytesAsync(path);
    }
}