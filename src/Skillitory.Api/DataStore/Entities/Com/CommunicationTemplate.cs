using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Entities.Com.Enumerations;

namespace Skillitory.Api.DataStore.Entities.Com;

public class CommunicationTemplate : IAuditableEntity
{
    public int Id { get; set; }
    public CommunicationTemplateTypeEnum CommunicationTemplateTypeId { get; set; }
    public string Name { get; set; } = "";
    public string Template { get; set; } = "";
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }

    public CommunicationTemplateType CommunicationTemplateType { get; set; } = null!;
}
