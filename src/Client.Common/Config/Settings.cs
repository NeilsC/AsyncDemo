using System;
using System.Configuration;
using System.IO;

namespace Client.Common.Config
{
    public class Settings
    {
        #region Fields

        private string scratchDirectory;

        #endregion

        #region Public Properties

        public long? MaxRecordsToRetrieve
        {
            get
            {
                long retVal;

                return long.TryParse(ConfigurationManager.AppSettings["MaxRecordsToRetrieve"], out retVal)
                           ? retVal
                           : default(long?);
            }
        }

        public string OutputDirectory
        {
            get
            {
                try
                {
                    return Directory.GetCurrentDirectory();
                }
                catch (ConfigurationErrorsException)
                {
                    return Environment.CurrentDirectory;
                }
            }
        }

        public string ScratchDirectory
        {
            get
            {
                if (this.scratchDirectory == null)
                {
                    DateTime now = DateTime.UtcNow;
                    this.scratchDirectory = string.Format("{0}-{1}-{2}_{3}", now.Year, now.Month, now.Day, now.Ticks);
                }

                return this.scratchDirectory;
            }
        }

        public int? ZipSegmentSizeMb
        {
            get
            {
                int retVal;
                return int.TryParse(ConfigurationManager.AppSettings["ZipSegmentSizeMB"], out retVal)
                           ? retVal
                           : default(int?);
            }
        }

        #endregion
    }
}