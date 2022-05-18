namespace GusMelfordBot.Core.Domain.System;

public interface IDataLakeService
{
    Task<bool> Write(string path, byte[] bytes);
    Task<byte[]?> ReadAndDelete(string path);
    Task<byte[]?> Read(string path);
    void Delete(string path);
    MemoryStream Archive(string folderPath, string fileName);
}