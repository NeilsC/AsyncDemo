using System.ServiceModel;

using Contracts;

namespace Client.Common.WCF
{
    public static class ImageServiceClient
    {
        #region Public Methods and Operators

        public static IImageService Create()
        {
            var factory = new ChannelFactory<IImageService>("ImageService");
            return factory.CreateChannel();
        }

        #endregion
    }
}