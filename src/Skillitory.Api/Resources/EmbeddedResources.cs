using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Resources;

[ExcludeFromCodeCoverage]
public static class EmbeddedResources
{
    private const string EmbeddedResourcePathRoot = "Skillitory.Api.Resources";

    public static string EmailTemplatePathRoot => $"{EmbeddedResourcePathRoot}.EmailTemplates";
}
