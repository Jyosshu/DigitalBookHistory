using System.Collections.Generic;

namespace DigitalBookHistoryLoader.models
{
    public class DigitalItem
    {
        public long TitleId { get; set; }
        public string Title { get; set; }
        public long KindId { get; set; }
        public string Kind { get; set; }
        public string ArtistName { get; set; }
        public bool Demo { get; set; }
        public bool PA { get; set; }
        public bool Edited { get; set; }
        public string ArtKey { get; set; }
        public long Borrowed { get; set; }
        public long Returned { get; set; }
        public long CircId { get; set; }
        public bool Children { get; set; }
        public bool FixedLayout { get; set; }
        public bool ReadAlong { get; set; }

        //List<Borrow> Borrows { get; set; } // This property is not relevant in the loading process.  It will be utilized when accessing the data.
    }
}
