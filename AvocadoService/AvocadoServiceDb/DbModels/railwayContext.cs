﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AvocadoService.AvocadoServiceDb.DbModels
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
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseNpgsql("Host=roundhouse.proxy.rlwy.net;Port=35316;Username=postgres;Password=IqhHNAjWczuVvNzNbnfdViENrzwdkvyG;Database=railway");
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
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('product_id_auto_inc'::regclass)");

                entity.Property(e => e.Brand)
                    .HasMaxLength(255)
                    .HasColumnName("brand");

                entity.Property(e => e.Brandinfo).HasColumnName("brandinfo");

                entity.Property(e => e.Consist).HasColumnName("consist");

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .HasColumnName("country");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Extra).HasColumnName("extra");

                entity.Property(e => e.Howtouse).HasColumnName("howtouse");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasPrecision(10, 2)
                    .HasColumnName("price");

                entity.Property(e => e.Source)
                    .HasMaxLength(255)
                    .HasColumnName("source");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .HasColumnName("url");

                entity.Property(e => e.Volume)
                    .HasPrecision(10, 3)
                    .HasColumnName("volume");

                entity.Property(e => e.Weight)
                    .HasPrecision(10, 3)
                    .HasColumnName("weight");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("subscription");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('sub_id_auto_inc'::regclass)");

                entity.Property(e => e.AccountId).HasMaxLength(255);

                entity.Property(e => e.Amount).HasPrecision(10, 2);

                entity.Property(e => e.DateCreate).HasColumnType("timestamp(6) without time zone");

                entity.Property(e => e.DateTime).HasColumnType("timestamp(6) without time zone");

                entity.Property(e => e.DateUpdate).HasColumnType("timestamp(6) without time zone");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Extra).IsRequired();

                entity.Property(e => e.InvoiceId).HasMaxLength(255);

                entity.Property(e => e.Rrn).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.SubscriptionId).HasMaxLength(255);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'unknown'::character varying");

                entity.Property(e => e.UserTgId).HasColumnName("userTgId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('user_id_auto_inc'::regclass)");

                entity.Property(e => e.Activity)
                    .HasMaxLength(255)
                    .HasColumnName("activity");

                entity.Property(e => e.Age)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("age");

                entity.Property(e => e.Allergy)
                    .HasMaxLength(255)
                    .HasColumnName("allergy");

                entity.Property(e => e.Bodyattentionareas)
                    .HasMaxLength(255)
                    .HasColumnName("bodyattentionareas");

                entity.Property(e => e.Bodygoals)
                    .HasMaxLength(255)
                    .HasColumnName("bodygoals");

                entity.Property(e => e.Bodyhairissues)
                    .HasMaxLength(255)
                    .HasColumnName("bodyhairissues");

                entity.Property(e => e.Bodyskincondition)
                    .HasMaxLength(255)
                    .HasColumnName("bodyskincondition");

                entity.Property(e => e.Bodyskinsensitivity)
                    .HasMaxLength(255)
                    .HasColumnName("bodyskinsensitivity");

                entity.Property(e => e.Bodyskintype)
                    .HasMaxLength(255)
                    .HasColumnName("bodyskintype");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Faceskincondition)
                    .HasMaxLength(255)
                    .HasColumnName("faceskincondition");

                entity.Property(e => e.Faceskingoals)
                    .HasMaxLength(255)
                    .HasColumnName("faceskingoals");

                entity.Property(e => e.Faceskinissues)
                    .HasMaxLength(255)
                    .HasColumnName("faceskinissues");

                entity.Property(e => e.Faceskintype)
                    .HasMaxLength(255)
                    .HasColumnName("faceskintype");

                entity.Property(e => e.Gender)
                    .HasMaxLength(255)
                    .HasColumnName("gender");

                entity.Property(e => e.Habits)
                    .HasMaxLength(255)
                    .HasColumnName("habits");

                entity.Property(e => e.Haircondition)
                    .HasMaxLength(255)
                    .HasColumnName("haircondition");

                entity.Property(e => e.Haircurrentproducts)
                    .HasMaxLength(255)
                    .HasColumnName("haircurrentproducts");

                entity.Property(e => e.Hairgoals)
                    .HasMaxLength(255)
                    .HasColumnName("hairgoals");

                entity.Property(e => e.Hairlength)
                    .HasMaxLength(255)
                    .HasColumnName("hairlength");

                entity.Property(e => e.Hairproducttexture)
                    .HasMaxLength(255)
                    .HasColumnName("hairproducttexture");

                entity.Property(e => e.Hairscalptype)
                    .HasMaxLength(255)
                    .HasColumnName("hairscalptype");

                entity.Property(e => e.Hairsensitivity)
                    .HasMaxLength(255)
                    .HasColumnName("hairsensitivity");

                entity.Property(e => e.Hairstructure)
                    .HasMaxLength(255)
                    .HasColumnName("hairstructure");

                entity.Property(e => e.Hairstylingtools)
                    .HasMaxLength(255)
                    .HasColumnName("hairstylingtools");

                entity.Property(e => e.Hairthickness)
                    .HasMaxLength(255)
                    .HasColumnName("hairthickness");

                entity.Property(e => e.Hairwashingfrequency)
                    .HasMaxLength(255)
                    .HasColumnName("hairwashingfrequency");

                entity.Property(e => e.Lifestyle)
                    .HasMaxLength(255)
                    .HasColumnName("lifestyle");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .HasColumnName("location");

                entity.Property(e => e.Stress)
                    .HasMaxLength(255)
                    .HasColumnName("stress");

                entity.Property(e => e.UserTgId).HasColumnName("userTgId");

                entity.Property(e => e.Vote)
                    .HasColumnName("vote")
                    .HasDefaultValueSql("11");

                entity.Property(e => e.WaterIntake)
                    .HasMaxLength(255)
                    .HasColumnName("water_intake");
            });

            modelBuilder.HasSequence("product_id_auto_inc");

            modelBuilder.HasSequence("sub_id_auto_inc");

            modelBuilder.HasSequence("user_id_auto_inc");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
