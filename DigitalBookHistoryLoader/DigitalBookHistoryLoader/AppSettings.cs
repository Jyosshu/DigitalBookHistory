
namespace DigitalBookHistoryLoader
{
    public static class AppSettings
    {
        public static class ConnectionStrings
        {
            public static string DefaultConnection = "Server=.\\SQLEXPRESS;Database=digital_book_history;Trusted_Connection=True;MultipleActiveResultSets=true;";

            public static string PostgresConnection = "Server=localhost;Port=5432;Database=digital_book_history;User Id=digital_book_history_appuser;Password=AAtjSUbLTRJVbu95LH3T";
        }

        public static string RemoteImageUrl
        {
            get => "https://d2snwnmzyr8jue.cloudfront.net/";
        }
    }
}
