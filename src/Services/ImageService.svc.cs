using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

using Contracts;

using Entities;

namespace Services
{
    public class ImageService : IImageService
    {
        #region Public Methods and Operators

        public void Dispose()
        {
        }

        public string DownloadImage(long imageId)
        {
            return DownloadImageInternal(imageId);
        }

        public long[] GetAllUserImageIds(string userName)
        {
            Sleep();

            return Enumerable.Range(1, 20).Select(n => (long)n).ToArray();
        }

        public Image GetMetadata(long imageId)
        {
            return GetMetadataInternal(imageId);
        }

        #endregion

        #region Methods

        private static string DownloadImageInternal(long imageId)
        {
            Sleep();

            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["TestDataDirectory"]);
            byte[][] images = files.Select(File.ReadAllBytes).ToArray();
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            return Convert.ToBase64String(images[random.Next(0, images.Length - 1)]);
        }

        private static Image GetMetadataInternal(long imageId)
        {
            Sleep();
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            return new Image { FileName = random.Next(1, 100) + ".png" };
        }

        private static void Sleep()
        {
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            int millisecondsTimeout = random.Next(100, 1000);

            Console.WriteLine("Sleeping {0} ms", millisecondsTimeout);
            Thread.Sleep(millisecondsTimeout);
        }

        #endregion
    }
}