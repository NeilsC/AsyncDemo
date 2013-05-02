namespace BeforeAsync.Config
{
    using System;
    using System.Configuration;

    static class Settings
    {
        public static long? MaxRecordsToRetrieve
        {
            get
            {
                long retVal;

                return long.TryParse(ConfigurationManager.AppSettings["MaxRecordsToRetrieve"], out retVal) ? retVal : default(long?);
            }
        }

        public static string OutputDirectory
        {
            get
            {
                try
                {
                    return System.IO.Directory.GetCurrentDirectory();
                }
                catch (ConfigurationErrorsException)
                {
                    return Environment.CurrentDirectory;
                }
            }
        }

        public static int? ZipSegmentSizeMb
        {
            get
            {
                int retVal;
                return int.TryParse(ConfigurationManager.AppSettings["ZipSegmentSizeMB"], out retVal)
                           ? retVal
                           : default(int?);
            }
        }

        public static string ScratchDirectory
        {
            get
            {
                var now = DateTime.UtcNow;
                return string.Format("{0}-{1}-{2}_{3}", now.Year, now.Month, now.Day, now.Ticks);
            }
        }
    }
}
