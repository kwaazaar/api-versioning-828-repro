using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace api_versioning_828_repro
{
    public class CustomApiVersionSelector : IApiVersionSelector
    {
        private readonly ApiVersioningOptions _options;

        public CustomApiVersionSelector(ApiVersioningOptions options)
        {
            _options = options;
        }

        public ApiVersion SelectVersion(HttpRequest request, ApiVersionModel model)
        {
            // Not very logical maybe, but for this repro it should just return a supported version
            var version = model.SupportedApiVersions.FirstOrDefault()
                ?? model.ImplementedApiVersions.FirstOrDefault()
                ?? _options.DefaultApiVersion;

            return version;
        }
    }
}
