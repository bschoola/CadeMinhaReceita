using System.Text.Json;

namespace CadeMinhaReceita.Domain.Extensions
{
    public static class StringExtension
    {
        public static bool IsJsonObject(this string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
