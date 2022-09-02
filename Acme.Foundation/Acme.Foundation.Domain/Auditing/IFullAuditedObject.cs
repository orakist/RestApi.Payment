namespace Acme.Foundation.Domain.Auditing;

/// <summary>
/// This interface adds <see cref="IDeletionAuditedObject"/> to <see cref="IAuditedObject"/>.
/// </summary>
public interface IFullAuditedObject : IAuditedObject, IDeletionAuditedObject
{

}
