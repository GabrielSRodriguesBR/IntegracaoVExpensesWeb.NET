using IntegracaoVExpensesWeb.Models;
using System.Data.Entity;

namespace IntegracaoVExpensesWeb.Business.DBContext
{
	public class DBContext : DbContext
    {
        public DBContext() : base("name=DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<RelatorioModel> Relatorios { get; set; }
        public DbSet<DespesaModel> Despesas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelatorioModel>()
       .        HasKey(r => r.RelatorioId);

            modelBuilder.Entity<DespesaModel>()
                .HasKey(d => d.ID);

            modelBuilder.Entity<RelatorioModel>()
                .HasMany(r => r.Despesas)
                .WithRequired(d => d.Relatorio)  
                .HasForeignKey(d => d.RelatorioId)
                .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);
        }
    }
}