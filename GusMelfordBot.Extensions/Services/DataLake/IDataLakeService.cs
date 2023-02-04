namespace GusMelfordBot.Extensions.Services.DataLake;

public interface IDataLakeService
{
    Task WriteFile(string path, byte[] bytes);
    Task<byte[]> ReadFileAsync(string path);
    void CreateDirectoryIfNotExist(string directoryPath);
    void RemoveFile(string path);
}