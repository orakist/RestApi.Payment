namespace Acme.Foundation.Domain.Entities;

public class CurrentUser : ICurrentUser
{
    public Guid? Id { get; init; }

    public string Name { get; init; }

    public bool IsAuthenticated { get; init; }
}
