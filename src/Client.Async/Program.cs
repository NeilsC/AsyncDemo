using System;
using System.IO;
using System.Threading.Tasks;

using Client.Async.ImageService;
using Client.Common;
using Client.Common.Config;

using Entities;

namespace Client.Async
{
    internal class Program
    {
        #region Methods

        private static Tuple<Image, string> AcquireImage(IImageService imageService, long imageId)
        {
            return new Tuple<Image, string>(
                GetMetadataAsync(imageService, imageId).Result, DownloadImage(imageService, imageId).Result);
        }

        private static async Task<string> DownloadImage(IImageService imageService, long imageId)
        {
            Console.WriteLine("Downloading image ({0})", imageId);
            return await imageService.DownloadImageAsync(imageId);
        }

        private static async Task<Image> GetMetadataAsync(IImageService imageService, long imageId)
        {
            Console.WriteLine("Getting image metadata");
            return await imageService.GetMetadataAsync(imageId);
        }

        private static void Main()
        {
            DateTime startTime = DateTime.Now;
            var settings = new Settings();
            var helper = new Helper(settings);
            int recordCount = 0;

            using (StreamWriter writer = helper.GetOutputWriter())
            using (var imageService = new ImageServiceClient())
            {
                writer.WriteLine(Image.GetHeaderString());

                Console.WriteLine("Getting image IDs");
                long[] imageIds = imageService.GetAllUserImageIds("neils");

                foreach (long imageId in imageIds)
                {
                    Tuple<Image, string> results = AcquireImage(imageService, imageId);

                    Image imageMetadata = results.Item1;
                    string image = results.Item2;

                    helper.WriteImageFile(image, imageMetadata.FileName);

                    writer.WriteLine(imageMetadata);

                    recordCount++;
                }
            }

            Console.WriteLine("{0} records written to file.", recordCount);
            Console.WriteLine("Creating zip file.");

            helper.WriteZipFile();
            helper.Cleanup();

            Console.WriteLine("Done.");

            TimeSpan elapsed = DateTime.Now - startTime;
            Console.WriteLine("Elapsed time: {0}", elapsed);
#if DEBUG
            Console.Write("<Enter> to quit.");
            Console.ReadLine();
#endif
        }

        #endregion
    }
}