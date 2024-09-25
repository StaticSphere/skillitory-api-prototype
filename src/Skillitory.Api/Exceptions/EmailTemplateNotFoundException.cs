namespace Skillitory.Api.Exceptions;

public class EmailTemplateNotFoundException : Exception
{
    public string TemplateName { get; private set; }

    public EmailTemplateNotFoundException(string templateName)
    {
        TemplateName = templateName;
    }
}
