using ProjectCore.Domain.Enums;
namespace ProjectCore.Domain.Entities;

public abstract class DomainEntity<TKey>
{
    public TKey Id { get; protected set; }

    public DateTime CreatedDate { get; protected set; }
    public Guid CreatedBy { get; protected set; }

    public DateTime? UpdatedDate { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public EntityStatus Status { get; protected set; } = EntityStatus.Active;

    protected DomainEntity() { }

    protected DomainEntity(TKey id, Guid createdBy)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkUpdated(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedDate = DateTime.UtcNow;
    }
}



