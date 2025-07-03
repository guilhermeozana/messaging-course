using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.StateMachine;

namespace Orders.Domain;

public partial class OrderContext : DbContext
{
    public DbSet<OrderStateData> OrderStates { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderStateMap());
        base.OnModelCreating(modelBuilder);
    }
}