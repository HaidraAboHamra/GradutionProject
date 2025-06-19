using GradutionProject.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Data;


public static class EntityConfiguration
{
    /// <summary>
    ///     This Extension Method Configure the properties inside every entity in this project
    /// </summary>
    ///     The same <see cref="ModelBuilder" /> instance so that additional configuration calls can be chained.
    /// </returns>
    public static ModelBuilder ConfigureEntity(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                var entity = builder.Entity(entityType.ClrType);
            }
        }
        return builder;
    }
}
