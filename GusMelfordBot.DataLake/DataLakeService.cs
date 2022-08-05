namespace GusMelfordBot.DataLake;

public class DataLakeService : IDataLakeService
{
    public Task WriteFile(string path, byte[] bytes)
    {
        return File.WriteAllBytesAsync(path, bytes);
    }
    
    public Task<byte[]> ReadFileAsync(string path)
    {
        return File.ReadAllBytesAsync(path);
    }
}