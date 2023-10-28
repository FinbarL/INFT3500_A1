using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace INFT3500.Models;

public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsInOrder> ProductsInOrders { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<Stocktake> Stocktakes { get; set; }

    public virtual DbSet<To> Tos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private async Task<User> CreateUser(string userName, string email, string password,
        bool isAdmin = false, bool isStaff = false)
    {
        var salt = GenerateSalt();
        var newUser = new User
        {
            UserName = userName,
            Email = email,
            Salt = salt,
            HashPw = HashPassword(password, salt),
            IsAdmin = isAdmin,
            IsStaff = isStaff
        };

        Users.Add(newUser);
        await SaveChangesAsync();
        var userTo = await Tos.FirstOrDefaultAsync(t => t.UserName == userName);

        if (userTo == null)
        {
            var newUserDetails = new To
            {
                UserName = newUser.UserName,
                Email = email,
                PhoneNumber = "1234567890",
                StreetAddress = "3500 INFT St",
                PostCode = 1337,
                Suburb = "Suburb",
                State = "State",
                CardNumber = "374245455400126",
                CardOwner = "John Doe",
                Expiry = "12/25",
                Cvv = 123
            };
            Tos.Add(newUserDetails);
        }
        else
        {
            userTo.Email = email;
            userTo.PhoneNumber = "1234567890";
            userTo.StreetAddress = "3500 INFT St";
            userTo.PostCode = 1234;
            userTo.Suburb = "Suburb";
            userTo.State = "State";
            userTo.CardNumber = "374245455400126";
            userTo.CardOwner = "John Doe";
            userTo.Expiry = "12/25";
            userTo.Cvv = 123;
        }
        await SaveChangesAsync();

        return newUser;
    }
    private string GenerateSalt()
    {
        //I stole this from https://stackoverflow.com/questions/45220359/encrypting-and-verifying-a-hashed-password-with-salt-using-pbkdf2-encryption

        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return Convert.ToBase64String(salt);
    }
    private string HashPassword(string password, string salt)
    {
        var bytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        return Convert.ToBase64String(bytes);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genre");

            entity.Property(e => e.GenreId)
                .ValueGeneratedNever()
                .HasColumnName("genreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });
        
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Customer).HasColumnName("customer");
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(255);

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customer)
                .HasConstraintName("FK_Orders_TO");
        });
        

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SubGenre).HasColumnName("subGenre");

            entity.HasOne(d => d.GenreNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Genre)
                .HasConstraintName("FK_Product_Genre");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK_Product_Users");
        });

        modelBuilder.Entity<ProductsInOrder>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ProduktId).HasColumnName("produktId");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_ProductsInOrders_Orders");

            entity.HasOne(d => d.Produkt).WithMany()
                .HasForeignKey(d => d.ProduktId)
                .HasConstraintName("FK_ProductsInOrders_Stocktake");
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.ToTable("Source");

            entity.Property(e => e.Sourceid).HasColumnName("sourceid");
            entity.Property(e => e.ExternalLink).HasColumnName("externalLink");
            entity.Property(e => e.SourceName).HasColumnName("Source_name");

            entity.HasOne(d => d.GenreNavigation).WithMany(p => p.Sources)
                .HasForeignKey(d => d.Genre)
                .HasConstraintName("FK_Source_Genre");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.SourceId).HasColumnName("SourceId");
            entity.Property(e => e.ProductId).HasColumnName("ProductId");
            entity.Property(e => e.ItemId).HasColumnName("ItemId");

            entity.ToTable("Stocktake");

            entity.HasOne(d => d.Product).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Stocktake_Product")
                .OnDelete(DeleteBehavior.Cascade);;

            entity.HasOne(d => d.Source).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_Stocktake_Source");
        });

        modelBuilder.Entity<To>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("TO");

            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.UserName).HasColumnName("UserName").HasMaxLength(50);
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.CardOwner).HasMaxLength(50);
            entity.Property(e => e.Cvv).HasColumnName("CVV");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Expiry)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(50);
            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserName)
                .HasConstraintName("FK_TO_Users");
            /*entity.HasOne(d => d.User).WithMany(p => p.)
                .HasForeignKey(d => d.PatronId)
                .HasConstraintName("FK_TO_Patrons");*/
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK_Users");
            entity.ToTable("User");

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HashPw)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("HashPW");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.IsStaff).HasColumnName("isStaff");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Salt)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public async Task SeedDataAsync()
    {
        var adminExists = await Users.AnyAsync(u => u.UserName == "admin");
        var staffExists = await Users.AnyAsync(u => u.UserName == "staff");
        var regularExists = await Users.AnyAsync(u => u.UserName == "user");

        if (!adminExists)
        {
            Console.WriteLine("CREATING ADMIN");
            await CreateUser("admin", "admin@admin.com", "admin", true, false);
        }

        if (!staffExists)
        {
            await CreateUser("staff", "staff@staff.com", "staff", false, true);
        }

        if (!regularExists)
        {
            await CreateUser("user", "user@user.com", "user", false, false);
        }
    }
}
