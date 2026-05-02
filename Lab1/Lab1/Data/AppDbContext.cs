using Microsoft.EntityFrameworkCore;
using Lab1.Models;

namespace Lab1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<CardEffect> CardEffects { get; set; }
        public DbSet<Evaluator> Evaluators { get; set; }
        public DbSet<CardRanking> CardRankings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE
            modelBuilder.Entity<Card>()
                .HasIndex(c => c.CardName)
                .IsUnique();

            modelBuilder.Entity<Effect>()
                .HasIndex(e => e.EffectCode)
                .IsUnique();

            modelBuilder.Entity<Evaluator>()
                .HasIndex(e => e.EvaluatorName)
                .IsUnique();

            modelBuilder.Entity<CardEffect>()
                .HasIndex(ce => new { ce.CardID, ce.EffectID })
                .IsUnique();

            modelBuilder.Entity<CardRanking>()
                .HasIndex(cr => new { cr.UserID, cr.CardID })
                .IsUnique();

            modelBuilder.Entity<CardRanking>()
                .HasIndex(cr => new { cr.UserID, cr.AlternativeRank })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // FOREIGN KEYS + DELETE
            modelBuilder.Entity<CardEffect>()
                .HasOne(ce => ce.Card)
                .WithMany(c => c.CardEffects)
                .HasForeignKey(ce => ce.CardID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardEffect>()
                .HasOne(ce => ce.Effect)
                .WithMany(e => e.CardEffects)
                .HasForeignKey(ce => ce.EffectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardRanking>()
                .HasOne(cr => cr.Card)
                .WithMany(c => c.CardRankings)
                .HasForeignKey(cr => cr.CardID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardRanking>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CardRankings)
                .HasForeignKey(cr => cr.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Evaluator)
                .WithMany()
                .HasForeignKey(u => u.EvaluatorID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}