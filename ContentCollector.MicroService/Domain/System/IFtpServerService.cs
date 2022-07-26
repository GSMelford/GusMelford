namespace ContentCollector.Domain.System;

public interface IFtpServerService
{
    Task<bool> UploadFile(string path, MemoryStream fileStream);
    Task<MemoryStream?> DownloadFile(string path);
}