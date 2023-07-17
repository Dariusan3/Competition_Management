using System;
using System.Collections.Generic;
using Competition_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Data;

public partial class CompetitionManagementContext : DbContext
{
    public CompetitionManagementContext()
    {
    }

    public CompetitionManagementContext(DbContextOptions<CompetitionManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CompetitionType> CompetitionTypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-U525FP3\\SQLEXPRESS; Initial catalog=Competition Management; trusted_connection=yes; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Competit__3214EC2779E0C9B2");

            entity.HasOne(d => d.CompetitionTypeNavigation).WithMany(p => p.Competitions).HasConstraintName("FK__Competiti__Compe__6383C8BA");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TeamCompe__TeamI__40058253"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TeamCompe__Compe__3F115E1A"),
                    j =>
                    {
                        j.HasKey("CompetitionId", "TeamId");
                        j.ToTable("TeamCompetition");
                        j.IndexerProperty<int>("CompetitionId").HasColumnName("CompetitionID");
                        j.IndexerProperty<int>("TeamId").HasColumnName("TeamID");
                    });
        });

        modelBuilder.Entity<CompetitionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Type__3214EC2782A36E73");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Game__3214EC27C7D6692A");

            entity.HasOne(d => d.Competition).WithMany(p => p.Games).HasConstraintName("FK__Game__Competitio__7E37BEF6");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s).HasConstraintName("FK__Game__Team1ID__7C4F7684");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s).HasConstraintName("FK__Game__Team2ID__7D439ABD");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Player__3214EC27817BFFD4");

            entity.HasOne(d => d.Team).WithMany(p => p.Players).HasConstraintName("FK__Player__TeamID__01142BA1");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Team__3214EC27866397DA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
