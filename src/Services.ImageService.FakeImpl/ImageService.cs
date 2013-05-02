namespace Services.ImageService.FakeImpl
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Entities;

    using Services.ImageService.Contracts;

    public class ImageService : IImageService
    {
        public string DownloadImage(long imageId)
        {
            this.sleep();

            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "testdata"));
            var images = files.Select(File.ReadAllBytes).ToArray();
            var random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            
            return Convert.ToBase64String(images[random.Next(0, images.Length - 1)]);
        }

        public Image GetMetadata(long imageId)
        {
            this.sleep();
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);            
            
            return new Image { FileName = random.Next(1, 100).ToString() + ".png" };
        }

        public long[] GetAllUserImageIds(string userName)
        {
            this.sleep();
            return new long[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        private void sleep()
        {
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            var millisecondsTimeout = random.Next(100, 1000);

            Console.WriteLine("Sleeping {0} ms", millisecondsTimeout);
            Thread.Sleep(millisecondsTimeout);
        }
    }
}
