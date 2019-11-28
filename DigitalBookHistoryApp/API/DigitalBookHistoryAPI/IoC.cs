using Microsoft.Extensions.DependencyInjection;
using DigitalBookHistoryAPI.Interface;
using DigitalBookHistoryAPI.Repository;

namespace DigitalBookHistoryAPI
{
    public static class IoC
    {
        public static void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IBookRepository, BookRepository>();
        }
    }
}
