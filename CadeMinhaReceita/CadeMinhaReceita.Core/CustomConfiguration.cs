using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CadeMinhaReceita.Common
{
    public static class CustomConfiguration
    {
        public static void ConfigureIoC(this IServiceCollection services, IConfiguration configuration)
        {
            #region Domain

            services.AddScoped<Domain.Contracts.Domain.IChatGptService, Domain.Services.ChatGptService>();

            #endregion

            #region Anticorruption

            services.AddScoped<Domain.Contracts.Anticorruption.IChatGptAdapter, Anticorruption.Adapters.ChatGptAdapter>();

            #endregion
        }
    }
}
