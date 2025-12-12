namespace Sample.AppHost.Extensions;

public static class ProjectResourceBuilderExtensions
{
    extension(IResourceBuilder<ProjectResource> builder)
    {
        public IResourceBuilder<ProjectResource> MapDisplayTextsForUrls(string displayTextPrefix)
            => builder.WithUrls(context => {
                foreach (var url in context.Urls)
                    if (string.IsNullOrEmpty(url.DisplayText))
                        url.DisplayText = $"{displayTextPrefix} ({url.Endpoint?.Scheme.ToUpper()})";
            });

        public IResourceBuilder<ProjectResource> WithOpenApiUrlEndpoint(string url = "/openapi/v1.json", string endpointName = "https")
            => builder.WithUrlForEndpoint(endpointName, _ => new ResourceUrlAnnotation
            {
                DisplayText = $"Open API specification ({endpointName.ToUpper()})",
                Url = url
            });
    }
}