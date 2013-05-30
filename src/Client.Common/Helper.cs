using System;
using System.IO;

using Client.Common.Config;

using Ionic.Zip;
using Ionic.Zlib;

namespace Client.Common
{
    public class Helper
    {
        #region Fields

        private readonly Settings _settings;

        #endregion

        #region Constructors and Destructors

        public Helper(Settings settings)
        {
            this._settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        public void Cleanup()
        {
            string folder = this.GetRootFolderPath(this._settings.ScratchDirectory);

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

        public StreamWriter GetOutputWriter()
        {
            string rootFolderName = this._settings.ScratchDirectory;
            string rootFolder = this.SetupRootFolder(rootFolderName);
            string imgFolder = Path.Combine(rootFolder, "img\\");
            this.SetupFolder(imgFolder);

            string dataFile = Path.Combine(rootFolder, "data.csv");

            return new StreamWriter(dataFile, false);
        }

        public string GetRootFolderPath(string rootFolderName)
        {
            return Path.Combine(this._settings.OutputDirectory, rootFolderName);
        }

        public void SetupFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public string SetupRootFolder(string rootFolderName)
        {
            string rootFolderPath = this.GetRootFolderPath(rootFolderName);

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

        public void WriteImageFile(string data, string fileName)
        {
            string filePath = Path.Combine(this._settings.OutputDirectory, this._settings.ScratchDirectory, "img", fileName);
            byte[] bytes = Convert.FromBase64String(data);
            var fileStream = new FileStream(filePath, FileMode.Create);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }

        public void WriteZipFile()
        {
            string outputFileName = this._settings.ScratchDirectory + ".zip";
            string outputFilePath = Path.Combine(this._settings.OutputDirectory, outputFileName);

            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.BestCompression;

                if (this._settings.ZipSegmentSizeMb.HasValue)
                {
                    zip.MaxOutputSegmentSize = this._settings.ZipSegmentSizeMb.Value * 1024 * 1024;
                }

                zip.AddDirectory(this.GetRootFolderPath(this._settings.ScratchDirectory));
                zip.Save(outputFilePath);

                Console.WriteLine("Created zip file: " + outputFilePath);
            }
        }

        #endregion
    }
}