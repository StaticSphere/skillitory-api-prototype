using FluentAssertions;
using Skillitory.Api.Exceptions;

namespace Skillitory.Api.Tests.Exceptions;

public class EmailTemplateRenderExceptionTests
{
    [Fact]
    public void TemplateName_ShouldReturnCorrectValue()
    {
        var exception = new EmailTemplateRenderException("test_template", "test_error");

        exception.TemplateName.Should().Be("test_template");
    }

    [Fact]
    public void Message_ShouldReturnCorrectValue()
    {
        var exception = new EmailTemplateRenderException("test_template", "test_error");

        exception.Message.Should().Be("test_error");
    }
}
