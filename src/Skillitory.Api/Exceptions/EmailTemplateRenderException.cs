namespace Skillitory.Api.Exceptions;

public class EmailTemplateRenderException : Exception
{
    public string TemplateName { get; private set; }

    public EmailTemplateRenderException(string templateName, string error)
        : base(error)
    {
        TemplateName = templateName;
    }
}
