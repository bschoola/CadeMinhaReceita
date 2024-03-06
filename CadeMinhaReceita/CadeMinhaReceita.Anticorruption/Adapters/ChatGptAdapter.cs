using CadeMinhaReceita.Domain.Contracts.Anticorruption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CadeMinhaReceita.Anticorruption.Adapters
{
    public class ChatGptAdapter : IChatGptAdapter
    {
        private const string AppSettingsKey = "Endpoints:ChatGPT35";
        private readonly HttpClient httpClient;
        private readonly string endpointUrl;
        private readonly ILogger<ChatGptAdapter> _logger;

        public ChatGptAdapter(IConfiguration configuration, ILogger<ChatGptAdapter> logger)
        {
            this.httpClient = new HttpClient();
            endpointUrl = configuration.GetSection(AppSettingsKey).Value;
            _logger = logger;
        }

        public async Task<string> TalkWith(string message)
        {
            try
            {
                // Construa a URL com a mensagem como parâmetro
                var requestUrl = $"{endpointUrl}?message={Uri.EscapeDataString(message)}";

                // Faça uma solicitação GET para o endpoint
                var response = await httpClient.GetAsync(requestUrl);

                // Verifique se a solicitação foi bem-sucedida
                response.EnsureSuccessStatusCode();

                // Leia o conteúdo da resposta
                var responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Erro ao fazer solicitação HTTP: {ex.Message}", ex);
                throw;
            }
        }
    }
}
