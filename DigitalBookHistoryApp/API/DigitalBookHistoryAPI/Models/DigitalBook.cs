using System;

namespace DigitalBookHistoryAPI.Models
{
    public class DigitalBook
    {
        public int Id { get; set; }
        public int TitleId { get; set; }
        public string Title { get; set; }
        public int KindId { get; set; }
        public string ArtistName { get; set; } // TODO: Refactor into List<Artists>?
        public string ArtKey { get; set; }
        public long Borrowed { get; set; }
        public long Returned { get; set; }

        public Image Image { get; set; }
        public Kind Kind { get; set; }

        private static DateTime StartDate
        {
            get => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public string BorrowedDate
        {
            get => StartDate.AddMilliseconds(Borrowed).ToLocalTime().ToShortDateString();
        }

        public string ReturnedDate
        {
            get => StartDate.AddMilliseconds(Returned).ToLocalTime().ToShortDateString();
        }
    }
}
