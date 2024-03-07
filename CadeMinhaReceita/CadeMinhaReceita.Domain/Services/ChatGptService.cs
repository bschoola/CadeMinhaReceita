using CadeMinhaReceita.Domain.Contracts.Anticorruption;
using CadeMinhaReceita.Domain.Contracts.Domain;

namespace CadeMinhaReceita.Domain.Services
{
    public class ChatGptService : IChatGptService
    {
        private const string _Context = "You are a professional cook.";
        private readonly IChatGptAdapter _chatGptAdapter;
        public ChatGptService(IChatGptAdapter chatGptAdapter)
        {
            _chatGptAdapter = chatGptAdapter;
        }

        public async Task<string> TalkWith(string message)
        {
            return await _chatGptAdapter.TalkWith(message, _Context);
        }
    }
}
