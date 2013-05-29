using System;
using System.ServiceModel;

using Entities;

namespace Contracts
{
    [ServiceContract]
    public interface IImageService : IDisposable
    {
        #region Public Methods and Operators

        [OperationContract]
        string DownloadImage(long imageId);

        [OperationContract]
        long[] GetAllUserImageIds(string userName);

        [OperationContract]
        Image GetMetadata(long imageId);

        #endregion
    }
}