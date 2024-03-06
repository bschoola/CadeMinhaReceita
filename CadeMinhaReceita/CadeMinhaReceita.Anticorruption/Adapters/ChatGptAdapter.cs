using CadeMinhaReceita.Domain.Contracts.Anticorruption;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CadeMinhaReceita.Anticorruption.Adapters
{
    public class ChatGptAdapter : IChatGptAdapter
    {
        private const string AppSettingsKey = "Endpoints:ChatGPT35";
        private const string ChatGptVariableName = "OpenAI_Key";
        private readonly ILogger<ChatGptAdapter> _logger;
        private readonly string _endpointUrl;
        private readonly string _chatGptKey;

        public ChatGptAdapter(IConfiguration configuration, ILogger<ChatGptAdapter> logger)
        {
            _endpointUrl = configuration.GetSection(AppSettingsKey)?.Value;
            _chatGptKey = Environment.GetEnvironmentVariable(ChatGptVariableName);
            _logger = logger;
        }

        public async Task<string> TalkWith(string message)
        {
            if (string.IsNullOrWhiteSpace(_chatGptKey)) {
                throw new InvalidOperationException("Key is required.");
            }

            if (string.IsNullOrWhiteSpace(_endpointUrl))
            {
                throw new InvalidOperationException("URL is required.");
            }

            try
            {
                var objRequest = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role  = "system", content = "You are a professional cook." },
                        new { role  = "user", content = message }
                    }
                };

                //var jsonIn = JsonConvert.SerializeObject(objRequest);

                string requestUrl = $"{_endpointUrl}?message={Uri.EscapeDataString(message)}";

                var response = await requestUrl.WithOAuthBearerToken(_chatGptKey).PostJsonAsync(objRequest);

                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();

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
