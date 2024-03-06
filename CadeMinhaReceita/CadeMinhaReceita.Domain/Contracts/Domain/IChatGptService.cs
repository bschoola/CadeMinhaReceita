namespace CadeMinhaReceita.Domain.Contracts.Domain
{
    public interface IChatGptService
    {
        Task<string> TalkWith(string message);
    }
}
