using FluentAssertions;
using Skillitory.Api.Exceptions;

namespace Skillitory.Api.Tests.Exceptions;

public class EmailTemplateNotFoundExceptionTests
{
    [Fact]
    public void TemplateName_ShouldReturnCorrectValue()
    {
        var exception = new EmailTemplateNotFoundException("test_template");

        exception.TemplateName.Should().Be("test_template");
    }
}
