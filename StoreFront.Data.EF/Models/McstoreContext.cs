using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StoreFront.Data.EF.Models;

public partial class McstoreContext : DbContext {
	public McstoreContext() {
	}

	public McstoreContext(DbContextOptions<McstoreContext> options)
		: base(options) {
	}

	public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

	public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

	public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

	public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

	public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

	public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

	public virtual DbSet<Block> Blocks { get; set; }

	public virtual DbSet<Category> Categories { get; set; }

	public virtual DbSet<Order> Orders { get; set; }

	public virtual DbSet<OrderProduct> OrderProducts { get; set; }

	public virtual DbSet<Product> Products { get; set; }

	public virtual DbSet<Source> Sources { get; set; }

	public virtual DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
		=> optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=MCStore;Trusted_Connection=true;Encrypt=false;MultipleActiveResultSets=true;");

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<AspNetRole>(entity => {
			entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
				.IsUnique()
				.HasFilter("([NormalizedName] IS NOT NULL)");

			entity.Property(e => e.Name).HasMaxLength(256);
			entity.Property(e => e.NormalizedName).HasMaxLength(256);
		});

		modelBuilder.Entity<AspNetRoleClaim>(entity => {
			entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

			entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
		});

		modelBuilder.Entity<AspNetUser>(entity => {
			entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

			entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
				.IsUnique()
				.HasFilter("([NormalizedUserName] IS NOT NULL)");

			entity.Property(e => e.Email).HasMaxLength(256);
			entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
			entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
			entity.Property(e => e.UserName).HasMaxLength(256);

			entity.HasMany(d => d.Roles).WithMany(p => p.Users)
				.UsingEntity<Dictionary<string, object>>(
					"AspNetUserRole",
					r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
					l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
					j => {
						j.HasKey("UserId", "RoleId");
						j.ToTable("AspNetUserRoles");
						j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
					});
		});

		modelBuilder.Entity<AspNetUserClaim>(entity => {
			entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

			entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
		});

		modelBuilder.Entity<AspNetUserLogin>(entity => {
			entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

			entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

			entity.Property(e => e.LoginProvider).HasMaxLength(128);
			entity.Property(e => e.ProviderKey).HasMaxLength(128);

			entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
		});

		modelBuilder.Entity<AspNetUserToken>(entity => {
			entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

			entity.Property(e => e.LoginProvider).HasMaxLength(128);
			entity.Property(e => e.Name).HasMaxLength(128);

			entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
		});

		modelBuilder.Entity<Block>(entity => {
			entity.Property(e => e.BlockId).HasColumnName("BlockID");
			entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
			entity.Property(e => e.Description)
				.HasMaxLength(200)
				.IsUnicode(false);
			entity.Property(e => e.Image).HasColumnType("image");
			entity.Property(e => e.Name)
				.HasMaxLength(20)
				.IsUnicode(false);
			entity.Property(e => e.ProperTool)
				.HasMaxLength(20)
				.IsUnicode(false);
			entity.Property(e => e.RelatedBlockId).HasColumnName("RelatedBlockID");
			entity.Property(e => e.SourceId).HasColumnName("SourceID");

			entity.HasOne(d => d.Category).WithMany(p => p.Blocks)
				.HasForeignKey(d => d.CategoryId)
				.HasConstraintName("FK_Blocks_Categories");

			entity.HasOne(d => d.RelatedBlock).WithMany(p => p.InverseRelatedBlock)
				.HasForeignKey(d => d.RelatedBlockId)
				.HasConstraintName("FK_Blocks_Blocks");

			entity.HasOne(d => d.Source).WithMany(p => p.Blocks)
				.HasForeignKey(d => d.SourceId)
				.HasConstraintName("FK_Blocks_Source");
		});

		modelBuilder.Entity<Category>(entity => {
			entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
			entity.Property(e => e.Description)
				.HasMaxLength(200)
				.IsUnicode(false);
			entity.Property(e => e.Name)
				.HasMaxLength(20)
				.IsUnicode(false);
		});

		modelBuilder.Entity<Order>(entity => {
			entity.Property(e => e.OrderId).HasColumnName("OrderID");
			entity.Property(e => e.BuyerId).HasColumnName("BuyerID");
			entity.Property(e => e.DateOrdered).HasColumnType("datetime");
			entity.Property(e => e.DeliveryCoords)
				.HasMaxLength(20)
				.IsUnicode(false);
			entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

			entity.HasOne(d => d.Buyer).WithMany(p => p.OrderBuyers)
				.HasForeignKey(d => d.BuyerId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Orders_Users");

			entity.HasOne(d => d.Supplier).WithMany(p => p.OrderSuppliers)
				.HasForeignKey(d => d.SupplierId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Orders_Users1");
		});

		modelBuilder.Entity<OrderProduct>(entity => {
			entity.Property(e => e.OrderProductId).HasColumnName("OrderProductID");
			entity.Property(e => e.OrderId).HasColumnName("OrderID");
			entity.Property(e => e.ProductId).HasColumnName("ProductID");

			entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
				.HasForeignKey(d => d.OrderId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_OrderProducts_Orders");

			entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
				.HasForeignKey(d => d.ProductId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_OrderProducts_Product");
		});

		modelBuilder.Entity<Product>(entity => {
			entity.HasKey(e => e.ProductId).HasName("PK_Product");

			entity.Property(e => e.ProductId).HasColumnName("ProductID");
			entity.Property(e => e.BlockId).HasColumnName("BlockID");
			entity.Property(e => e.Price).HasColumnType("money");

			entity.HasOne(d => d.Block).WithMany(p => p.Products)
				.HasForeignKey(d => d.BlockId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Product_Blocks");
		});

		modelBuilder.Entity<Source>(entity => {
			entity.ToTable("Source");

			entity.Property(e => e.SourceId).HasColumnName("SourceID");
			entity.Property(e => e.Description)
				.HasMaxLength(200)
				.IsUnicode(false);
			entity.Property(e => e.Name)
				.HasMaxLength(20)
				.IsUnicode(false);
		});

		modelBuilder.Entity<User>(entity => {
			entity.Property(e => e.UserId).HasColumnName("UserID");
			entity.Property(e => e.HouseCoords)
				.HasMaxLength(20)
				.IsUnicode(false);
			entity.Property(e => e.Name)
				.HasMaxLength(20)
				.IsUnicode(false);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
