using Microsoft.EntityFrameworkCore;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Domain.Enums;

namespace CarWatchBelgium.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SavedSearch> SavedSearches { get; set; } = null!;
    public DbSet<Listing> Listings { get; set; } = null!;
    public DbSet<PriceHistory> PriceHistories { get; set; } = null!;
    public DbSet<UserListingState> UserListingStates { get; set; } = null!;
    public DbSet<SavedSearchMatch> SavedSearchMatches { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureSavedSearches(modelBuilder);
        ConfigureListings(modelBuilder);
        ConfigurePriceHistories(modelBuilder);
        ConfigureUserListingStates(modelBuilder);
        ConfigureSavedSearchMatches(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<User>();
        
        entity.ToTable("Users");
        entity.HasKey(u => u.Id);
        
        entity.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("TEXT");
        
        entity.HasIndex(u => u.Email).IsUnique();
        
        entity.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("TEXT");
        
        entity.Property(u => u.DisplayName)
            .HasMaxLength(100)
            .HasColumnType("TEXT");
        
        entity.Property(u => u.CreatedAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(u => u.LastLoginAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // Relationships
        entity.HasMany(u => u.SavedSearches)
            .WithOne(ss => ss.User)
            .HasForeignKey(ss => ss.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(u => u.ListingStates)
            .WithOne(uls => uls.User)
            .HasForeignKey(uls => uls.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureSavedSearches(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SavedSearch>();
        
        entity.ToTable("SavedSearches");
        entity.HasKey(ss => ss.Id);
        
        entity.Property(ss => ss.Name)
            .IsRequired()
            .HasMaxLength(120)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.Make)
            .HasMaxLength(80)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.Model)
            .HasMaxLength(80)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("BE")
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.City)
            .HasMaxLength(120)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.FuelType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.Transmission)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(ss => ss.SellerType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        var requiredKeywordsProperty = entity.Property(ss => ss.RequiredKeywords)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("TEXT");
        
        requiredKeywordsProperty.Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
        
        var excludedKeywordsProperty = entity.Property(ss => ss.ExcludedKeywords)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("TEXT");
        
        excludedKeywordsProperty.Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
        
        entity.Property(ss => ss.IsActive)
            .HasDefaultValue(true);
        
        entity.Property(ss => ss.CreatedAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(ss => ss.LastMatchedAtUtc)
            .HasColumnType("DATETIME");
        
        // Indices
        entity.HasIndex(ss => new { ss.UserId, ss.Name });
        entity.HasIndex(ss => ss.IsActive);
    }

    private static void ConfigureListings(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Listing>();
        
        entity.ToTable("Listings");
        entity.HasKey(l => l.Id);
        
        entity.Property(l => l.Source)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.ExternalId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.FallbackHash)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Url)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.ImageUrl)
            .HasMaxLength(1000)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(300)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Make)
            .HasMaxLength(80)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Model)
            .HasMaxLength(80)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("EUR")
            .HasColumnType("TEXT");
        
        entity.Property(l => l.City)
            .HasMaxLength(120)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("BE")
            .HasColumnType("TEXT");
        
        entity.Property(l => l.FuelType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.Transmission)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.SellerType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnType("TEXT");
        
        entity.Property(l => l.IsAvailable)
            .HasDefaultValue(true);
        
        entity.Property(l => l.FirstSeenAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(l => l.LastSeenAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(l => l.LastPriceChangeAtUtc)
            .HasColumnType("DATETIME");
        
        // Unique constraints
        entity.HasIndex(l => new { l.Source, l.ExternalId })
            .IsUnique()
            .HasFilter("[ExternalId] IS NOT NULL AND [ExternalId] <> ''");
        
        entity.HasIndex(l => new { l.Source, l.FallbackHash })
            .IsUnique();
        
        // Indices
        entity.HasIndex(l => l.Price);
        entity.HasIndex(l => l.Year);
        entity.HasIndex(l => l.MileageKm);
        entity.HasIndex(l => l.FirstSeenAtUtc);
        entity.HasIndex(l => l.LastSeenAtUtc);
        entity.HasIndex(l => l.IsAvailable);
    }

    private static void ConfigurePriceHistories(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PriceHistory>();
        
        entity.ToTable("PriceHistories");
        entity.HasKey(ph => ph.Id);
        
        entity.Property(ph => ph.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("EUR")
            .HasColumnType("TEXT");
        
        entity.Property(ph => ph.CapturedAtUtc)
            .HasColumnType("DATETIME");
        
        // Relationships
        entity.HasOne(ph => ph.Listing)
            .WithMany(l => l.PriceHistory)
            .HasForeignKey(ph => ph.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indices
        entity.HasIndex(ph => new { ph.ListingId, ph.CapturedAtUtc });
    }

    private static void ConfigureUserListingStates(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<UserListingState>();
        
        entity.ToTable("UserListingStates");
        entity.HasKey(uls => uls.Id);
        
        entity.Property(uls => uls.CreatedAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(uls => uls.SeenAtUtc)
            .HasColumnType("DATETIME");
        
        entity.Property(uls => uls.FavoritedAtUtc)
            .HasColumnType("DATETIME");
        
        // Unique constraint
        entity.HasIndex(uls => new { uls.UserId, uls.ListingId })
            .IsUnique();
        
        // Indices
        entity.HasIndex(uls => uls.IsSeen);
        entity.HasIndex(uls => uls.IsFavorite);
        entity.HasIndex(uls => uls.IsIgnored);
        
        // Relationships
        entity.HasOne(uls => uls.Listing)
            .WithMany(l => l.UserStates)
            .HasForeignKey(uls => uls.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureSavedSearchMatches(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SavedSearchMatch>();
        
        entity.ToTable("SavedSearchMatches");
        entity.HasKey(ssm => ssm.Id);
        
        entity.Property(ssm => ssm.MatchedAtUtc)
            .HasColumnType("DATETIME");
        
        // Unique constraint
        entity.HasIndex(ssm => new { ssm.SavedSearchId, ssm.ListingId })
            .IsUnique();
        
        // Index
        entity.HasIndex(ssm => ssm.MatchedAtUtc);
        
        // Relationships
        entity.HasOne(ssm => ssm.SavedSearch)
            .WithMany(ss => ss.Matches)
            .HasForeignKey(ssm => ssm.SavedSearchId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(ssm => ssm.Listing)
            .WithMany(l => l.SavedSearchMatches)
            .HasForeignKey(ssm => ssm.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
