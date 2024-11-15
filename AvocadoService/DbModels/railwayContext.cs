using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AvocadoService.DbModels
{
    public partial class railwayContext : DbContext
    {
        public railwayContext()
        {
        }

        public railwayContext(DbContextOptions<railwayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseNpgsql("Host=roundhouse.proxy.rlwy;Port=35316;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway");
#else
                        optionsBuilder.UseNpgsql("Host=postgres.railway.internal;Port=5432;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway");
#endif
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Brend)
                    .HasMaxLength(255)
                    .HasColumnName("brend");

                entity.Property(e => e.Consist).HasColumnName("consist");

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .HasColumnName("country");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");

                entity.Property(e => e.Weight).HasColumnName("weight");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
