using System;

namespace Entities
{
    public class Image
    {
        #region Public Properties

        public long AccountId { get; set; }

        public DateTime? EndDate { get; set; }

        public string FileName { get; set; }

        public string FileServiceId { get; set; }

        public bool? IsRenewalNotificationSent { get; set; }

        public string Orientation { get; set; }

        public string RenewalStatus { get; set; }

        public DateTime? RenewalStatusDate { get; set; }

        public DateTime? StartDate { get; set; }

        public string Status { get; set; }

        public int StockImageId { get; set; }

        public DateTime? ValidationDate { get; set; }

        #endregion

        #region Public Methods and Operators

        public static string GetHeaderString()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                "ACCOUNT_ID",
                "END_DATE",
                "FILENAME",
                "FILE_SERVICE_ID",
                "IS_RENEWAL_NOTIFICATION_SENT",
                "ORIENTATION",
                "RENEWAL_STATUS",
                "RENEWAL_STATUS_DATETIME",
                "START_DATE",
                "STATUS",
                "STOCK_IMAGE_ID",
                "VALIDATION_DATE");
        }

        public override string ToString()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                this.AccountId,
                this.EndDate,
                this.FileName,
                this.FileServiceId,
                this.IsRenewalNotificationSent,
                this.Orientation,
                this.RenewalStatus,
                this.RenewalStatusDate,
                this.StartDate,
                this.Status,
                this.StockImageId,
                this.ValidationDate);
        }

        #endregion
    }
}