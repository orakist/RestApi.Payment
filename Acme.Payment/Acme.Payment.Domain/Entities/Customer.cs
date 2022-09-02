using Acme.Foundation.Domain.Entities;

namespace Acme.Payment.Domain.Entities;

public class Customer : FullAuditedEntity<Guid>
{
    public string Name { get; set; }
    
    public Customer(Guid id, string name)
        : base(id)
    {
        Name = name;
    }
}
