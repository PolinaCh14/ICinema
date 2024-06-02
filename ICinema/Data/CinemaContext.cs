using System;
using System;
using System.Collections.Generic;
using ICinema.Models;
using Microsoft.EntityFrameworkCore;

namespace ICinema.Data;

public partial class CinemaContext(DbContextOptions<CinemaContext> options) : DbContext(options)
{
    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatType> SeatTypes { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionType> SessionTypes { get; set; }

    public virtual DbSet<Technology> Technologies { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.HallId).HasName("PK__Hall__7E60E2143786DCC1");

            entity.ToTable("Hall");

            entity.HasIndex(e => e.HallName, "AK_Hall_HallName").IsUnique();

            entity.HasIndex(e => e.HallName, "UQ_HallName").IsUnique();

            entity.Property(e => e.HallName)
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.Technology).WithMany(p => p.Halls)
                .HasForeignKey(d => d.TechnologyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Hall_Technology");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movie__4BD2941A56227639");

            entity.ToTable("Movie");

            entity.HasIndex(e => e.MovieName, "AK_Movie_MovieName");

            entity.Property(e => e.Cast)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Directors)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Genres)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IMDBRate)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("IMBDRate");
            entity.Property(e => e.MovieName)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.PosterPath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Writers)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.PaymentCredentials)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UserFirstName)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.UserLastName)
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713F330A4C5B0");

            entity.ToTable("Seat");

            entity.HasOne(d => d.Hall).WithMany(p => p.Seats)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Seat_Hall");

            entity.HasOne(d => d.SeatType).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Seat_SeatType");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.HasKey(e => e.SeatTypeId).HasName("PK__SeatType__7468C4FE3462AF36");

            entity.ToTable("SeatType");

            entity.HasIndex(e => e.SeatTypeName, "AK_SeatType_SeatTypeName").IsUnique();

            entity.Property(e => e.BasePrice).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.SeatTypeName)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Session__C9F4929003F835D2");

            entity.ToTable("Session");

            entity.HasIndex(e => new { e.Date, e.Time, e.HallId }, "AK_Session_Date_Time_Hall").IsUnique();

            entity.Property(e => e.Format)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Hall).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Session_Hall");

            entity.HasOne(d => d.Movie).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Session_Movie");

            entity.HasOne(d => d.SessionType).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SessionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Session_SessionType");
        });

        modelBuilder.Entity<SessionType>(entity =>
        {
            entity.HasKey(e => e.SessionTypeId).HasName("PK__SessionT__D774FFCD4F23A1AC");

            entity.ToTable("SessionType");

            entity.HasIndex(e => e.SessionTypeName, "AK_SessionType_SessionTypeName").IsUnique();

            entity.Property(e => e.Coefficient)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(6, 2)");
            entity.Property(e => e.SessionTypeName)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Technology>(entity =>
        {
            entity.HasKey(e => e.TechnologyId).HasName("PK__Technolo__7057015850C4FB99");

            entity.ToTable("Technology");

            entity.HasIndex(e => e.TechnologyName, "AK_Technology_TechnologyName").IsUnique();

            entity.Property(e => e.Coefficient)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(6, 2)");
            entity.Property(e => e.TechnologyName)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("XPKTicket");

            entity.ToTable("Ticket");

            entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Ticket_Order");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Seat");

            entity.HasOne(d => d.Session).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK_Ticket_Session");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("XPKUser");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_Email").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserStatus)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
