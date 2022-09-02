using Acme.Foundation.Domain.Entities;

namespace Acme.Foundation.Domain.Auditing;
public interface IAuditPropertySetter
{
    void SetIdProperty(object targetObject);

    void SetCreationProperties(object targetObject);

    void SetModificationProperties(object targetObject);

    void SetDeletionProperties(object targetObject);
}

public class AuditPropertySetter : IAuditPropertySetter
{
    private readonly ICurrentUser _currentUser;

    public AuditPropertySetter(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public void SetCreationProperties(object targetObject)
    {
        SetCreationTime(targetObject);
        SetCreatorId(targetObject);
    }

    public void SetModificationProperties(object targetObject)
    {
        SetLastModificationTime(targetObject);
        SetLastModifierId(targetObject);
    }

    public void SetDeletionProperties(object targetObject)
    {
        SetDeletionTime(targetObject);
        SetDeleterId(targetObject);
    }

    public void SetIdProperty(object targetObject)
    {
        if (targetObject is not IEntity<Guid> entityWithGuidId)
            return;

        if (entityWithGuidId.Id == default)
        {
            entityWithGuidId.Id = Guid.NewGuid();
        }
    }

    private void SetCreationTime(object targetObject)
    {
        if (!(targetObject is IHasCreationTime objectWithCreationTime))
        {
            return;
        }

        if (objectWithCreationTime.CreationTime == default)
        {
            objectWithCreationTime.CreationTime = DateTime.Now;
        }
    }

    private void SetCreatorId(object targetObject)
    {
        if (!_currentUser.Id.HasValue)
        {
            return;
        }

        if (targetObject is ICreationAuditedObject mayHaveCreatorObject)
        {
            if (mayHaveCreatorObject.CreatorId.HasValue && mayHaveCreatorObject.CreatorId.Value != default)
            {
                return;
            }

            mayHaveCreatorObject.CreatorId = _currentUser.Id;
        }
    }

    private void SetLastModificationTime(object targetObject)
    {
        if (targetObject is IHasModificationTime objectWithModificationTime)
        {
            objectWithModificationTime.LastModificationTime = DateTime.Now;
        }
    }

    private void SetLastModifierId(object targetObject)
    {
        if (!(targetObject is IModificationAuditedObject modificationAuditedObject))
        {
            return;
        }

        modificationAuditedObject.LastModifierId = _currentUser.Id;
    }

    private void SetDeletionTime(object targetObject)
    {
        if (targetObject is IHasDeletionTime objectWithDeletionTime)
        {
            objectWithDeletionTime.DeletionTime ??= DateTime.Now;
        }

        if (targetObject is ISoftDelete objectSoftDelete)
        {
            objectSoftDelete.IsDeleted = true;
        }
    }

    private void SetDeleterId(object targetObject)
    {
        if (!(targetObject is IDeletionAuditedObject deletionAuditedObject))
        {
            return;
        }

        if (deletionAuditedObject.DeleterId != null)
        {
            return;
        }

        deletionAuditedObject.DeleterId = _currentUser.Id;
    }
}
