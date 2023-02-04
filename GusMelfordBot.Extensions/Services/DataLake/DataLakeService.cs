namespace GusMelfordBot.Extensions.Services.DataLake;

public class DataLakeService : IDataLakeService
{
    public void CreateDirectoryIfNotExist(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public Task WriteFile(string path, byte[] bytes)
    {
        return File.WriteAllBytesAsync(path, bytes);
    }
    
    public Task<byte[]> ReadFileAsync(string path)
    {
        return File.ReadAllBytesAsync(path);
    }

    public void RemoveFile(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
            //
        }
    }
}