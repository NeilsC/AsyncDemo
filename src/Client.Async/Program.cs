using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Client.Async.ImageService;
using Client.Common;
using Client.Common.Config;

using Entities;

namespace Client.Async
{
    internal class Program
    {
        private class ImageDetails
        {
            public string ImageBytes;

            public Image Metadata;
        }

        #region Methods
        
        private static async Task<ImageDetails> AcquireImage(IImageService imageService, long imageId)
        {
            return
                new ImageDetails
                    {
                        ImageBytes = await DownloadImageAsync(imageService, imageId),
                        Metadata = await GetMetadataAsync(imageService, imageId)
                    };
        }

        private static async Task<string> DownloadImageAsync(IImageService imageService, long imageId)
        {
            Console.WriteLine("[{1}] Downloading image with ID ({0})", imageId, AppDomain.GetCurrentThreadId());
            return await imageService.DownloadImageAsync(imageId);
        }

        private static async Task<Image> GetMetadataAsync(IImageService imageService, long imageId)
        {
            Console.WriteLine("[{1}] Getting image metadata with ID ({0})", imageId, AppDomain.GetCurrentThreadId());
            return await imageService.GetMetadataAsync(imageId);
        }

        private static void Main()
        {
            var startTime = DateTime.Now;
            var settings = new Settings();
            var helper = new Helper(settings);            
            var tasks = new List<Task<ImageDetails>>();

            // Get the ID's
            var imageIds = GetImageIds();

            // Create the Tasks
            
            foreach (var imageId in imageIds)
            {
                var task = new Task<ImageDetails>(() => AcquireImage(new ImageServiceClient(), imageId).Result);
                task.Start();
                tasks.Add(task);
            }

            // wait for the tasks
            Task.WaitAll(tasks.ToArray());

            // write out the files
            using (var writer = helper.GetOutputWriter())
            {
                writer.WriteLine(Image.GetHeaderString());
              
                foreach (var task in tasks)
                {
                    helper.WriteImageFile(task.Result.ImageBytes, task.Result.Metadata.FileName);
                    writer.WriteLine(task.Result.Metadata);
                }
            }

            Console.WriteLine("{0} images processed.", tasks.Count);
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

        private static long[] GetImageIds()
        {
            long[] imageIds;
            using (var imageService = new ImageServiceClient())
            {
                Console.WriteLine("Getting image IDs");
                imageIds = imageService.GetAllUserImageIds("neils");
            }
            return imageIds;
        }

        #endregion
    }
}