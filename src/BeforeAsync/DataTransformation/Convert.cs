using System;

namespace LogoExporter.DataTransformation
{
    using System.Data;

    using Entities;

    public class Convert
    {
        public static Image FromDataReader(IDataRecord reader)
        {
            return new Image
            {
                AccountId = ConvertFromDbVal<long>(reader["ACCOUNT_ID"]),
                EndDate = ConvertFromDbVal<DateTime?>(reader["END_DATE"]),
                FileName = ConvertFromDbVal<string>(reader["FILENAME"]),
                FileServiceId = ConvertFromDbVal<string>(reader["FILE_SERVICE_ID"]),
                IsRenewalNotificationSent = ConvertFromDbVal<bool?>(reader["IS_RENEWAL_NOTIFICATION_SENT"]),
                Orientation = ConvertFromDbVal<string>(reader["ORIENTATION"]),
                RenewalStatus = ConvertFromDbVal<string>(reader["RENEWAL_STATUS"]),
                RenewalStatusDate = ConvertFromDbVal<DateTime?>(reader["RENEWAL_STATUS_DATETIME"]),
                StartDate = ConvertFromDbVal<DateTime?>(reader["START_DATE"]),
                Status = ConvertFromDbVal<string>(reader["STATUS"]),
                StockImageId = ConvertFromDbVal<int>(reader["STOCK_IMAGE_ID"]),
                ValidationDate = ConvertFromDbVal<DateTime?>(reader["VALIDATION_DATE"])
            };
        }

        private static T ConvertFromDbVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }
            return (T)obj;
        }
    }
}
