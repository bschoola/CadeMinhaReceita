namespace CadeMinhaReceita.Api.Models
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Messages = new List<string> { };
        }

        public ErrorResponse(string message)
        {
            Messages = new List<string> { message };
        }

        public ErrorResponse(List<string> messages) => Messages = messages;

        public ErrorResponse(Exception exception)
        {
            Messages = new List<string>
            {
                "Unfortunately an error occurred while processing your request."
            };

            Exception = exception;
        }

        public IList<string> Messages { get; private set; }

        public Exception Exception { get; private set; }
    }
}
