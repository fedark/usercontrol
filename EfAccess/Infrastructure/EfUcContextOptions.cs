using Data.Infrastructure.Services;

namespace EfAccess.Infrastructure;
public class EfUcContextOptions
{
    public EfDbProvider DbProvider { get; set; }
    public string ConnectionString { get; set; } = default!;
    public IdentitySeedOptions SeedOptions { get; set; } = default!;
}
