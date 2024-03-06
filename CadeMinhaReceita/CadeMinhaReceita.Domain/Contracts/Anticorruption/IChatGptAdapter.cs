namespace CadeMinhaReceita.Domain.Contracts.Anticorruption
{
    public interface IChatGptAdapter
    {
        Task<string> TalkWith(string message);
    }
}
