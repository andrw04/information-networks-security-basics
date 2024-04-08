using System.Web;

namespace SecureApp.Protection;

public static class XXSProtection
{
    public static string EscapeHtml(string input)
    {
        return HttpUtility.HtmlEncode(input);
    }
}