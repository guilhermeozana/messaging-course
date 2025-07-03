using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.StateMachine;

namespace Orders.Domain;

public class OrderStateMap : IEntityTypeConfiguration<OrderStateData>
{
    public void Configure(EntityTypeBuilder<OrderStateData> builder)
    {
        builder.HasKey(x => x.OrderId);

        builder.Property(x => x.CurrentState).HasMaxLength(64);
        builder.Property(x => x.OrderId);
        builder.Property(x => x.Amount);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.CanceledAt);

        // Optionally configure other aspects like indexes
        builder.HasIndex(x => x.OrderId);

    }
}