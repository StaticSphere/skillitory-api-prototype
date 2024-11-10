using FluentAssertions;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Com.Interfaces;
using Skillitory.Api.DataStore.Entities.Com.Enumerations;
using Skillitory.Api.Exceptions;
using Skillitory.Api.Services;

namespace Skillitory.Api.Tests.Services;

public class EmailServiceTests
{
    private readonly ICommunicationTemplateDataService _communicationTemplateDataService;
    private readonly IFluentEmail _fluentEmail;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private const string TestTemplate = "<p>{{name}}</p>";
    private const string TestBadTemplate = "<p>{{name}</p>";

    public EmailServiceTests()
    {
        _communicationTemplateDataService = Substitute.For<ICommunicationTemplateDataService>();
        _fluentEmail = Substitute.For<IFluentEmail>();
        _configuration = Substitute.For<IConfiguration>();

        _emailService = new EmailService(
            _communicationTemplateDataService,
            _fluentEmail,
            _configuration
        );
    }

    [Theory]
    [InlineData("https://www.test.com")]
    [InlineData("https://www.test.com/")]
    [InlineData("https://www.test.com\\")]
    public void UrlRootEndsWithForwardSlash(string url)
    {
        _configuration["WebAppUrl"].Returns(url);

        _emailService.UrlRoot.Should().Be("https://www.test.com/");
    }

    [Fact]
    public async Task SendEmailInternalAsync_GetsCommunicationTemplate()
    {
        await _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", null);

        await _communicationTemplateDataService.Received(1)
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email);
    }

    [Fact]
    public async Task SendEmailInternalAsync_ThrowsWhenTemplateNotFound()
    {
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email)
            .Returns((string?)null!);

        var action = () => _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", null);

        await action.Should().ThrowAsync<EmailTemplateNotFoundException>()
            .Where(x => x.TemplateName == "Test_Template");
    }

    [Fact]
    public async Task SendEmailInternalAsync_MailsParsedTemplateToCorrectEmail()
    {
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email)
            .Returns(TestTemplate);

        await _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", new { Name = "Foobar" });

        _fluentEmail.Received(1).To("test@test.com");
    }

    [Fact]
    public async Task SendEmailInternalAsync_MailsParsedTemplateWithCorrectSubject()
    {
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email)
            .Returns(TestTemplate);
        _fluentEmail.To("test@test.com").Returns(_fluentEmail);

        await _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", new { Name = "Foobar" });

        _fluentEmail.Received(1).Subject("Test_Subject");
    }

    [Fact]
    public async Task SendEmailInternalAsync_MailsParsedTemplateWithCorrectBody()
    {
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email)
            .Returns(TestTemplate);
        _fluentEmail.To("test@test.com").Returns(_fluentEmail);
        _fluentEmail.Subject("Test_Subject").Returns(_fluentEmail);

        await _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", new { name = "Foobar" });

        _fluentEmail.Received(1).Body("<p>Foobar</p>", true);
    }

    [Fact]
    public async Task SendEmailInternalAsync_MailsParsedTemplate()
    {
        var cancellationToken = new CancellationToken();
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email, cancellationToken)
            .Returns(TestTemplate);
        _fluentEmail.To("test@test.com").Returns(_fluentEmail);
        _fluentEmail.Subject("Test_Subject").Returns(_fluentEmail);
        _fluentEmail.Body("<p>Foobar</p>", true).Returns(_fluentEmail);

        await _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", new { name = "Foobar" });

        await _fluentEmail.Received(1).SendAsync(cancellationToken);
    }

    [Fact]
    public async Task SendEmailInternalAsync_ThrowsWhenTemplateCannotBeRendered()
    {
        var cancellationToken = new CancellationToken();
        _communicationTemplateDataService
            .GetCommunicationTemplateAsync("Test_Template", CommunicationTemplateTypeEnum.Email, cancellationToken)
            .Returns(TestBadTemplate);

        var action = () => _emailService.SendEmailInternalAsync("test@test.com", "Test_Subject", "Test_Template", new { name = "Foobar" }, cancellationToken);

        await action.Should().ThrowAsync<EmailTemplateRenderException>()
            .Where(x => x.TemplateName == "Test_Template");
    }
}
