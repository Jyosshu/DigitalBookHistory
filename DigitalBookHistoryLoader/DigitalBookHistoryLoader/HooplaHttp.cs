using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DigitalBookHistoryLoader
{
    public class HooplaHttp
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HooplaHttp> _log;

        static readonly HttpClient client = new HttpClient();

        public HooplaHttp(IConfiguration config, ILogger<HooplaHttp> log)
        {
            _config = config;
            _log = log;
        }

        public async Task<string> GetHooplaHistory()
        {
            try
            {
                // TODO: If the token fails or has expired, ask the user for login cred.  Capture the new token and save it to user env variables.
                // TOOD: if the env variable doesn't exist, ask the user for login cred.  Capture the new token and save it to user env variables.
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.79 Safari/537.36");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.GetValue<string>("HOOPLA_TOKEN"));
                return await client.GetStringAsync(_config.GetValue<string>("hooplaHistoryUrl"));
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return null;
            }
        }
    }
}
