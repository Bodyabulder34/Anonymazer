
using System.Collections.Generic;
using System.Text.Json;
using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Anonymazer.Data;

public class AppDbContext : DbContext
{

    public DbSet<User> Users => Set<User>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AnonymizationHistory> AnonymizationHistory =>
        Set<AnonymizationHistory>();

    public DbSet<AnonymizationJob> AnonymizationJobs =>
    Set<AnonymizationJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
    .HasIndex(user => user.Username)
    .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<AnonymizationHistory>()
    .HasOne(history => history.User)
    .WithMany(user => user.AnonymizationHistory)
    .HasForeignKey(history => history.UserId)
    .OnDelete(DeleteBehavior.SetNull);

        var property = modelBuilder.Entity<AnonymizationHistory>()
            .Property(history => history.DetectedCounts);

        property
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<Dictionary<string, int>>(
                    value,
                    (JsonSerializerOptions?)null) ?? new Dictionary<string, int>())
            .HasColumnType("jsonb");

        property.Metadata.SetValueComparer(
            new ValueComparer<Dictionary<string, int>>(
                (left, right) =>
                    JsonSerializer.Serialize(left, (JsonSerializerOptions?)null) ==
                    JsonSerializer.Serialize(right, (JsonSerializerOptions?)null),

                value => JsonSerializer.Serialize(
                    value,
                    (JsonSerializerOptions?)null).GetHashCode(),

                value => JsonSerializer.Deserialize<Dictionary<string, int>>(
                    JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    (JsonSerializerOptions?)null)!
            )
        );
    }
}
