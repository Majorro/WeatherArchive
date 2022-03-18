using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    /// <summary>
    /// Extensions for <see cref="IHtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        public const string HttpMethodOverrideFormName = "X-HTTP-Method-Override";

        public static IHtmlContent HttpMethodOverride(
            this IHtmlHelper helper,
            System.Net.Http.HttpMethod method,
            string name = HttpMethodOverrideFormName)
        {
            // kostyl' for put requests from the upload form
            return helper.Raw($"<input type=\"hidden\" name=\"{name}\" value=\"{method}\" />");
        }
    }
}