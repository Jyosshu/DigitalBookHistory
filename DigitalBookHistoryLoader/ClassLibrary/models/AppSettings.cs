
namespace ClassLibrary
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DigitalBookSQL { get; set; }

        public string DigitalBookPostgres { get; set; }
    }
}
