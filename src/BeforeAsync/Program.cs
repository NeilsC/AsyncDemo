namespace BeforeAsync
{
    using System;
    using System.IO;

    using BeforeAsync.Config;

    using Entities;

    using Ionic.Zip;
    using Ionic.Zlib;

    using Services.ImageService.Contracts;
    using Services.ImageService.FakeImpl;

    class Program
    {
        static void Main()
        {
            var recordCount = 0;
          
            var rootFolderName = Settings.ScratchDirectory;
            var rootFolder = SetupRootFolder(rootFolderName);
            var imgFolder = Path.Combine(rootFolder, "img\\");
            SetupFolder(imgFolder);

            var dataFile = Path.Combine(rootFolder, "data.csv");
                       
            using (var writer = new StreamWriter(dataFile, false))
            using (IImageService imageService = new ImageService())
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
                    
                    WriteImageFile(image, imgFolder, imageMetadata.FileName);
                    
                    writer.WriteLine(imageMetadata);

                    recordCount++;                    
                }
            }

            Console.WriteLine("{0} records written to file.", recordCount);
            Console.WriteLine("Creating zip file.");

            WriteZipFile(rootFolder, rootFolderName + ".zip");
            Cleanup(rootFolder);

            Console.WriteLine("Done.");
#if DEBUG
            Console.Write("<Enter> to quit.");
            Console.ReadLine();
#endif
        }

        private static void Cleanup(string folder)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception)
                {
                    Console.WriteLine("Couldn't delete working directory.");
                }
            }
        }

        private static void WriteZipFile(string rootFolder, string outputFileName)
        {
            var outputFilePath = Path.Combine(Settings.OutputDirectory, outputFileName);

            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.BestCompression;

                if (Settings.ZipSegmentSizeMb.HasValue)
                {
                    zip.MaxOutputSegmentSize = Settings.ZipSegmentSizeMb.Value * 1024 * 1024;
                }

                zip.AddDirectory(rootFolder);
                zip.Save(outputFilePath);

                Console.WriteLine("Created zip file: " + outputFilePath);
            }
        }

        private static string SetupRootFolder(string rootFolderName)
        {
            var rootFolderPath = Path.Combine(Settings.OutputDirectory, rootFolderName);

            if (Directory.Exists(rootFolderPath))
            {
                try
                {
                    Console.WriteLine("Directory already exists: {0}. Attempting to delete.", rootFolderPath);
                    Directory.Delete(rootFolderPath, true);
                    Console.WriteLine("Directory deleted.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Failed to remove existing directory. Error Message: {0}", exception.Message);
                    throw;
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("Attempting to create directory: {0}", rootFolderPath);
                    Directory.CreateDirectory(rootFolderPath);
                    Console.WriteLine("Directory created.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to create directory. Error Message: {0}", rootFolderPath);
                    throw;
                }
            }

            return rootFolderPath;
        }

        private static void SetupFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }   

        private static void WriteImageFile(string data, string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);
            var bytes = Convert.FromBase64String(data);
            var fileStream = new FileStream(filePath, FileMode.Create);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }      

    }
}
