using System.Net;
using GusMelfordBot.Core.Domain.System;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.System;

public class FtpServerService : IFtpServerService
{
    private readonly string _ftpUrl;
    private readonly string _userName;
    private readonly string _password;

    private readonly ILogger<FtpServerService> _logger;

    public FtpServerService(
        string ftpUrl, 
        string userName, 
        string password, 
        ILogger<FtpServerService> logger)
    {
        _ftpUrl = ftpUrl;
        _userName = userName;
        _password = password;
        _logger = logger;
    }
    
    public async Task<bool> UploadFile(string path, MemoryStream fileStream)
    {
        Stream? requestStream = null;
        try
        {
            _logger.LogInformation("UploadFile starts {FtpUrl}", _ftpUrl + path);
            FtpWebRequest? ftpWebRequest = FtpWebRequest.Create(_ftpUrl + path) as FtpWebRequest;
            ftpWebRequest!.Method = WebRequestMethods.Ftp.UploadFile;
            ftpWebRequest.Credentials = new NetworkCredential(_userName, _password);
            requestStream = ftpWebRequest.GetRequestStream();
            _logger.LogInformation("Start uploading file {FtpUrl}", _ftpUrl + path);
            int read;
            byte[] buffer = new byte[8092];
            while ((read = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                await requestStream.WriteAsync(buffer, 0, read)!;
            }

            await requestStream.FlushAsync()!;
        }
        catch (global::System.Exception exception)
        {
            _logger.LogError("Failed to upload file {FtpUrl} Exception Message: {Exception}", 
                _ftpUrl + path, exception.Message);
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

        _logger.LogError("Successfully uploaded {FtpUrl}", _ftpUrl + path);
        return true;
    }

    public async Task<MemoryStream?> DownloadFile(string path)
    {
        try
        {
            FtpWebRequest? ftpWebRequest = FtpWebRequest.Create(_ftpUrl + path) as FtpWebRequest;
            ftpWebRequest!.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpWebRequest.Credentials = new NetworkCredential(_userName, _password);

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