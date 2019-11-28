using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public Int64 Borrowed { get; set; }
        public Int64 Returned { get; set; }

        public Image Image { get; set; }
        public Kind Kind { get; set; }
    }
}
