
namespace DigitalBookHistoryAPI.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string AltText { get; set; }
        public string ArtKey { get; set; }
        public string RemoteUrl { get; set; }
        public string LocalUrl { get; set; }
    }
}
