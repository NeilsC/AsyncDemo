namespace Services.ImageService.Contracts
{
    using System;

    using Entities;

    public interface IImageService : IDisposable 
    {
        string DownloadImage(long imageId);

        Image GetMetadata(long imageId);

        long[] GetAllUserImageIds(string userName);
    }
}
