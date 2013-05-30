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
        private struct ImageDetails
        {
            public string ImageBytes;

            public Image Metadata;
        }
        #region Methods

        private static async Task<ImageDetails> AcquireImage(IImageService imageService, long imageId)
        {
            return new ImageDetails
                       {
                           ImageBytes = await DownloadImage(imageService, imageId),
                           Metadata = await GetMetadataAsync(imageService, imageId)
                       };
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
                    var task = AcquireImage(imageService, imageId);

                    var imageMetadata = task.Result.Metadata;
                    var image = task.Result.ImageBytes;

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