namespace Acme.Foundation.Domain.Auditing;

/// <summary>
/// This interface can be implemented to store creation information (who and when created).
/// </summary>
public interface ICreationAuditedObject : IHasCreationTime
{
    /// <summary>
    /// Id of the creator.
    /// </summary>
    Guid? CreatorId { get; set; }
}
