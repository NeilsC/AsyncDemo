using System;
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
            var startTime = DateTime.Now;
            var settings = new Settings();
            var helper = new Helper(settings);
            var recordCount = 0;

            using (var writer = helper.GetOutputWriter())
            using (var imageService = new ImageServiceClient())
            {
                writer.WriteLine(Image.GetHeaderString());

                Console.WriteLine("Getting image IDs");
                var imageIds = imageService.GetAllUserImageIds("neils");

                foreach (var imageId in imageIds)
                {
                    var results = AcquireImage(imageService, imageId);

                    var imageMetadata = results.Item1;
                    var image = results.Item2;

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

            var elapsed = DateTime.Now - startTime;
            Console.WriteLine("Elapsed time: {0}", elapsed);
#if DEBUG
            Console.Write("<Enter> to quit.");
            Console.ReadLine();
#endif
        }

        #endregion
    }
}