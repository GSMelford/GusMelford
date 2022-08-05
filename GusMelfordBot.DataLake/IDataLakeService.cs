﻿namespace GusMelfordBot.DataLake;

public interface IDataLakeService
{
    Task WriteFile(string path, byte[] bytes);
    Task<byte[]> ReadFileAsync(string path);
}