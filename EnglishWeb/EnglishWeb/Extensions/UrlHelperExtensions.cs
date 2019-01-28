using Microsoft.AspNetCore.Mvc;

namespace EnglishWeb.Extensions
{
    public static class UrlHelperExtensions
    {
        //  For reset
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string action, string controller, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: action,
                controller: controller,
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
