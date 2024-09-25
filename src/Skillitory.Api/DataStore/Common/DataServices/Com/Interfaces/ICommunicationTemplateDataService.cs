using Skillitory.Api.DataStore.Entities.Com.Enumerations;

namespace Skillitory.Api.DataStore.Common.DataServices.Com.Interfaces;

public interface ICommunicationTemplateDataService
{
    Task<string?> GetCommunicationTemplateAsync(string name, CommunicationTemplateTypeEnum communicationTemplateType,
        CancellationToken cancellationToken = default);
}
