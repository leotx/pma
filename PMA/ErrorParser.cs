using System.Linq;
using System.Xml.Linq;

namespace PMA
{
    public static class ErrorParser
    {
        public static bool IsError(this string response)
        {
            var entry = XDocument.Parse(response);
            var xElement = entry.Element("response");
            if (xElement == null) return false;
            var responseType = xElement.Element("responseType");
            return responseType != null && responseType.Value.Equals("1");
        }

        public static bool IsSuccess(this string response)
        {
            return !IsError(response);
        }

        public static string GetErrorMessage(this string response)
        {
            var errorMessage = XDocument.Parse(response).Descendants("erro").FirstOrDefault();
            return errorMessage != null ? errorMessage.Value : string.Empty;
        }
    }
}