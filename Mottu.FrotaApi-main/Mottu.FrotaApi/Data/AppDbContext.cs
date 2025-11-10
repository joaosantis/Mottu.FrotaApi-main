using Microsoft.EntityFrameworkCore;
using Mottu.FrotaApi.Models;

namespace Mottu.FrotaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FILIAL
            modelBuilder.Entity<Filial>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                entity.Property(f => f.Nome).HasMaxLength(200).IsRequired();
                entity.Property(f => f.Endereco).HasMaxLength(300).IsRequired();
                entity.HasMany(f => f.Motos)
                      .WithOne(m => m.Filial)
                      .HasForeignKey(m => m.FilialId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // MOTO
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.Placa).HasMaxLength(20).IsRequired();
                entity.Property(m => m.Modelo).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Status).HasMaxLength(50).IsRequired();
                entity.HasMany(m => m.Manutencoes)
                      .WithOne(ma => ma.Moto)
                      .HasForeignKey(ma => ma.MotoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // MANUTENCAO
            modelBuilder.Entity<Manutencao>(entity =>
            {
                entity.HasKey(ma => ma.Id);
                entity.Property(ma => ma.Id).ValueGeneratedOnAdd();
                entity.Property(ma => ma.Descricao).HasMaxLength(500).IsRequired();
                entity.Property(ma => ma.Data).IsRequired();
            });
        }
    }
}
