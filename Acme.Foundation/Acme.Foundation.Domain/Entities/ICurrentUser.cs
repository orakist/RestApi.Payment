namespace Acme.Foundation.Domain.Entities;

public interface ICurrentUser
{
    Guid? Id { get; }

    string Name { get; }

    bool IsAuthenticated { get; }
}
