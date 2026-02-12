using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Http.Extensions;

namespace uSupport.Extensions
{
    public abstract class uSupportTemplate<T> : RazorPage<T>
    {
        public string GetBaseUrl(HttpRequest request)
        {
            var uriString = request?.GetEncodedUrl() ?? null;

            return !string.IsNullOrWhiteSpace(uriString) ? new Uri(uriString).GetLeftPart(UriPartial.Authority) : null;
        }
    }
}