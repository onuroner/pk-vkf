using Microsoft.EntityFrameworkCore;
using Vk.Data.Domain;

namespace Vk.Data.Context;

public class VkDbContext : DbContext
{
    public VkDbContext(DbContextOptions<VkDbContext> options) : base(options)
    {

    }


    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfigruration());
        modelBuilder.ApplyConfiguration(new AccountConfigruration());
        modelBuilder.ApplyConfiguration(new AccountTransactionConfigruration());
        modelBuilder.ApplyConfiguration(new AddressConfigruration());
        modelBuilder.ApplyConfiguration(new EftTransactionConfigruration());
        modelBuilder.ApplyConfiguration(new CardConfigruration());

        base.OnModelCreating(modelBuilder);
    }

}
