using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Event> Events { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Rsvp> Rsvps { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Seat> Seats { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User configuration
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            });
            
            // Event configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Events)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(e => e.EventDate).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // 🆕 Package and payment configurations
                entity.Property(e => e.PackageType)
                      .HasDefaultValue(PackageType.Essential);
                      
                entity.Property(e => e.PackagePrice)
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0);
                      
                entity.Property(e => e.TotalAmount)
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0);
                      
                entity.Property(e => e.EnabledAddOns)
                      .HasDefaultValue("[]");
                      
                entity.Property(e => e.PaymentStatus)
                      .HasDefaultValue("Pending");
                      
                entity.Property(e => e.IsPaid)
                      .HasDefaultValue(false);
            });
            
            // Guest configuration
            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasIndex(g => g.CustomLink).IsUnique();
                entity.HasOne(g => g.Event)
                      .WithMany(e => e.Guests)
                      .HasForeignKey(g => g.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
            
            // RSVP configuration
            modelBuilder.Entity<Rsvp>(entity =>
            {
                entity.HasOne(r => r.Guest)
                      .WithMany(g => g.Rsvps)
                      .HasForeignKey(r => r.GuestId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(r => r.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(r => r.Status).HasDefaultValue("Pending");
                entity.Property(r => r.PartySize).HasDefaultValue(1);
            });
            
            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Payments)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(p => p.Amount).HasPrecision(18, 2);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(p => p.Status).HasDefaultValue("Pending");
            });
            
            // Media configuration
            modelBuilder.Entity<Media>(entity =>
            {
                entity.HasOne(m => m.Event)
                      .WithMany(e => e.Media)
                      .HasForeignKey(m => m.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(m => m.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(m => m.IsPublic).HasDefaultValue(true);
            });
            
            // Table configuration
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasOne(t => t.Event)
                      .WithMany(e => e.Tables)
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(t => t.IsActive).HasDefaultValue(true);
            });
            
            // Seat configuration
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasOne(s => s.Table)
                      .WithMany(t => t.Seats)
                      .HasForeignKey(s => s.TableId)
                      .OnDelete(DeleteBehavior.NoAction);
                
                entity.HasOne(s => s.Guest)
                      .WithMany(g => g.Seats)
                      .HasForeignKey(s => s.GuestId)
                      .OnDelete(DeleteBehavior.SetNull);
                
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(s => s.IsReserved).HasDefaultValue(false);
            });
            
            // Additional constraints
            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint("CK_Event_EventDate", "EventDate > GETUTCDATE()"));
            
            modelBuilder.Entity<Rsvp>()
                .ToTable(t => t.HasCheckConstraint("CK_Rsvp_Status", "Status IN ('Pending', 'Attending', 'Declined')"));
            
            modelBuilder.Entity<Payment>()
                .ToTable(t => t.HasCheckConstraint("CK_Payment_Status", "Status IN ('Pending', 'Completed', 'Failed', 'Refunded')"))
                .ToTable(t => t.HasCheckConstraint("CK_Payment_Amount", "Amount >= 0"));
                
            // 🆕 Event package and payment constraints
            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint("CK_Event_PaymentStatus", 
                    "PaymentStatus IN ('Pending', 'Completed', 'Failed', 'Refunded')"))
                .ToTable(t => t.HasCheckConstraint("CK_Event_PackagePrice", "PackagePrice >= 0"))
                .ToTable(t => t.HasCheckConstraint("CK_Event_TotalAmount", "TotalAmount >= 0"));
        }
    }
}