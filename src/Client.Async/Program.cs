using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Client.Async.ImageService;
using Client.Common;
using Client.Common.Config;

using Entities;

namespace Client.Async
{
    internal class Program
    {
        struct ImageBytesAndMetadata
        {
            public string ImageBytes;

            public Image Metadata;
        }

        private static readonly ConcurrentQueue<ImageBytesAndMetadata> Data =
            new ConcurrentQueue<ImageBytesAndMetadata>();
        #region Methods

        private static async Task<string> DownloadImage(long imageId)
        {
            var threadId = AppDomain.GetCurrentThreadId();
            Console.WriteLine("[{1}] Downloading image with ID {0}", imageId, threadId);
            
            using (var client = new ImageServiceClient())
            {
                return await client.DownloadImageAsync(imageId);
            }
        }

        private static async Task<Image> GetMetadata(long imageId)
        {
            var threadId = AppDomain.GetCurrentThreadId();
            Console.WriteLine("[{1}] Getting image metadata with ID {0}", imageId, threadId);

            using (var client = new ImageServiceClient())
            {
                return await client.GetMetadataAsync(imageId);
            }
        }

        private static async Task<ImageBytesAndMetadata> GetMetaDataAndDownloadImage(long imageId)
        {
            return new ImageBytesAndMetadata
                       {
                           ImageBytes = await DownloadImage(imageId),
                           Metadata = await GetMetadata(imageId)
                       };
        }

        private static void AddDataToDictionary(long[] imageIds)
        {
            var tasks = new List<Task>();

            foreach (var imageId in imageIds)
            {
                var task = new Task(
                    async () =>
                        { 
                            var result = await GetMetaDataAndDownloadImage(imageId);
                            Data.Enqueue(result);
                        }
                    );

                task.Start();
                tasks.Add(task);                
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void WriteMetadata(Helper helper)
        {
            var threadId = AppDomain.GetCurrentThreadId();
            Console.WriteLine("[{0}] Writing csv file", threadId);

            using (var writer = helper.GetOutputWriter())
            {
                writer.WriteLine(Image.GetHeaderString());
                bool success;
                do
                {
                    ImageBytesAndMetadata record;
                    success = Data.TryDequeue(out record);
                    writer.WriteLine(record.Metadata);
                }
                while (success);
            }            
        }
    
        private static void Main()
        {
            var startTime = DateTime.Now;
            var settings = new Settings();
            var helper = new Helper(settings);

            long[] imageIds;
            using (var client = new ImageServiceClient())
            {
                Console.WriteLine("Getting image IDs");
                imageIds = client.GetAllUserImageIds("neils");
            }
            
            AddDataToDictionary(imageIds);
            
            Task.Run(() => WriteMetadata(helper));
            
            //helper.WriteZipFile();
            //helper.Cleanup();

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