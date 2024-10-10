using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.DataServices.Com.Interfaces;
using Skillitory.Api.DataStore.Entities.Com.Enumerations;

namespace Skillitory.Api.DataStore.Common.DataServices.Com;

[ExcludeFromCodeCoverage]
public class CommunicationTemplateDataService : ICommunicationTemplateDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public CommunicationTemplateDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetCommunicationTemplateAsync(string name, CommunicationTemplateTypeEnum communicationTemplateType,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CommunicationTemplates
            .Where(x => x.Name == name && x.CommunicationTemplateTypeId == communicationTemplateType)
            .Select(x => x.Template)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
