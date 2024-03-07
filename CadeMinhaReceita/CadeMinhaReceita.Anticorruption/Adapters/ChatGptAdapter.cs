using CadeMinhaReceita.Domain.Contracts.Anticorruption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace CadeMinhaReceita.Anticorruption.Adapters
{
    public class ChatGptAdapter : IChatGptAdapter
    {
        private const string AppSettingsKey = "Endpoints:ChatGPT35";
        private const string ChatGptVariableName = "OpenAI_Key";
        private const string GptV4 = "gpt-4-turbo-preview";
        private const string GptV35 = "gpt-3.5-turbo";
        private readonly ILogger<ChatGptAdapter> _logger;
        private readonly string _apiUrl;
        private readonly string _chatGptKey;

        public ChatGptAdapter(IConfiguration configuration, ILogger<ChatGptAdapter> logger)
        {
            _apiUrl = configuration.GetSection(AppSettingsKey)?.Value;
            _chatGptKey = Environment.GetEnvironmentVariable(ChatGptVariableName);
            _logger = logger;
        }

        public async Task<string> TalkWith(string message, string context)
        {
            string couldNotLoadDescription = "ChatGPT description could not be loaded";
            var Model = GptV4;
            var Temperature = 0.3;

            var messages = CreateMessageRequest(message, context);

            var data = new
            {
                model = Model,
                messages,
                temperature = Temperature
            };

            var HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chatGptKey}");

            var json = JsonSerializer.Serialize(data);

            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.PostAsync(_apiUrl, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(responseContent);
                    var choicesElement = doc.RootElement.GetProperty("choices");
                    var messageObject = choicesElement[0].GetProperty("message");
                    var content = messageObject.GetProperty("content").GetString();

                    if (content is not null)
                    {
                        return content;
                    }
                    else
                    {
                        _logger.LogError("ChatGPT API response did not contain expected content");
                        return couldNotLoadDescription;
                    }
                }
                else
                {
                    _logger.LogError("ChatGPT API request failed with status code: {StatusCode}", response.StatusCode);
                    return couldNotLoadDescription;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatGPT error occurred: {Message}", ex.Message);
                return couldNotLoadDescription;
            }
        }

        private object CreateMessageRequest(string message, string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                var messagesWithoutContext = new[]
                {
                    new { role = "user", content = $"{message}" }
                };

                return messagesWithoutContext;
            }

            var messagesWithContext = new[]
            {
                new { role = "system", content = $"{context}" },
                new { role = "user", content = $"{message}" }
            };

            return messagesWithContext;
        }

    }
}
