using Client.Common;
using Client.Common.Config;
using Client.Common.WCF;
using Entities;
using System;

namespace Client.NoAsync
{
    class Program
    {
        static void Main()
        {
            var startTime = DateTime.Now;
            var settings = new Settings();
            var helper = new Helper(settings);
            var recordCount = 0;
                                      
            using (var writer = helper.GetOutputWriter())
            using (var imageService = ImageServiceClient.Create())
            {
                writer.WriteLine(Image.GetHeaderString());

                Console.WriteLine("Getting image IDs");
                var imageIds = imageService.GetAllUserImageIds("neils");

                foreach (var imageId in imageIds)
                {
                    Console.WriteLine("Downloading image");                    
                    var image = imageService.DownloadImage(imageId);

                    Console.WriteLine("Getting image metadata");
                    var imageMetadata = imageService.GetMetadata(imageId);
                    
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
    }
}
