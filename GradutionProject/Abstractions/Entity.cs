
namespace GradutionProject.Abstractions;

public abstract class Entity : Entity<int>
{
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : struct, IEquatable<TId>
{
	public TId Id { get; set; }

    public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    public DateTime CreatedDate { get; set; } = DateTime.Now;



    

	public bool Equals(Entity<TId>? other)
	{
		if (other is null)
		{
			return false;
		}
        return this == other;
    }

	public override bool Equals(object? obj)
	{
		if (obj is Entity<TId> entity)
		{
			return this == entity;
		}
		return false;
	}

	public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
	{
        if (a is null || b is null)
        {
            return ReferenceEquals(a, b);
        }
		return a.Id.Equals(b.Id);
	}

	public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
	{
        if (a is null || b is null)
        {
            return a == b;
        }
        return !a.Id.Equals(b.Id);
	}
	public override int GetHashCode() => Id.GetHashCode();
}