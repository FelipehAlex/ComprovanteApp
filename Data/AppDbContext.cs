using ComprovantesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ComprovantesApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
        public DbSet<Comprovante> Comprovantes => Set<Comprovante>();
        public DbSet<HistoricoComprovante> Historicos => Set<HistoricoComprovante>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comprovante>()
                .HasIndex(c => new { c.FornecedorId, c.NumeroDocumento })
                .IsUnique();

            modelBuilder.Entity<Comprovante>()
                .HasOne(c => c.Fornecedor)
                .WithMany(f => f.Comprovantes)
                .HasForeignKey(c => c.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoricoComprovante>()
                .HasOne(h => h.Comprovante)
                .WithMany(c => c.Historicos)
                .HasForeignKey(h => h.ComprovanteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
