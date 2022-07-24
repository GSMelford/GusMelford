using System.Net;
using ContentCollector.MircoService.Domain.System;
using ContentCollector.Settings;

namespace ContentCollector.Services.System;

public class FtpServerService : IFtpServerService
{
    private readonly FtpSettings _ftpSettings;
    private readonly ILogger<FtpServerService> _logger;

    public FtpServerService(AppSettings appSettings, ILogger<FtpServerService> logger)
    {
        _ftpSettings = appSettings.FtpSettings;
        _logger = logger;
    }
    
    public async Task<bool> UploadFile(string path, MemoryStream fileStream)
    {
        Stream? requestStream = null;
        try
        {
            FtpWebRequest? ftpWebRequest = FtpWebRequest.Create(_ftpSettings.FtpUrl + path) as FtpWebRequest;
            ftpWebRequest!.Method = WebRequestMethods.Ftp.UploadFile;
            ftpWebRequest.Credentials = new NetworkCredential(_ftpSettings.Username, _ftpSettings.Password);
            requestStream = ftpWebRequest.GetRequestStream();
            _logger.LogInformation("Start uploading file {FtpUrl}", _ftpSettings.FtpUrl + path);
            int read;
            byte[] buffer = new byte[8092];
            while ((read = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                await requestStream.WriteAsync(buffer, 0, read)!;
            }

            await requestStream.FlushAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError("Failed to upload file {FtpUrl} Exception Message: {Exception}", 
                _ftpSettings.FtpUrl + path, exception.Message);
            return false;
        }
        finally
        {
            fileStream.Close();
            await fileStream.DisposeAsync();

            if (requestStream != null)
            {
                requestStream.Close();
                await requestStream.DisposeAsync();
            }
        }

        _logger.LogError("Successfully uploaded {FtpUrl}", _ftpSettings.FtpUrl + path);
        return true;
    }

    public async Task<MemoryStream?> DownloadFile(string path)
    {
        try
        {
            FtpWebRequest? ftpWebRequest = FtpWebRequest.Create(_ftpSettings.FtpUrl + path) as FtpWebRequest;
            ftpWebRequest!.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpWebRequest.Credentials = new NetworkCredential(_ftpSettings.Username, _ftpSettings.Password);

            await using Stream ftpStream = ftpWebRequest.GetResponse().GetResponseStream();
            MemoryStream memoryStream = new MemoryStream();
            await ftpStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch
        {
            return null;
        }
    }
}