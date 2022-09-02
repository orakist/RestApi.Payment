namespace Acme.Foundation.Domain.Auditing;

/// <summary>
/// This interface can be implemented to store modification information (who and when modified lastly).
/// </summary>
public interface IModificationAuditedObject : IHasModificationTime
{
    /// <summary>
    /// Last modifier user for this entity.
    /// </summary>
    Guid? LastModifierId { get; set; }
}
