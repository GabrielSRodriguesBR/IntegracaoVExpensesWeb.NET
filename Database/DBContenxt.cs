using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
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

			//modelBuilder.Entity<RelatorioModel>().HasKey(r => r.ID);

			modelBuilder.Entity<RelatorioModel>()
	   .HasKey(r => r.RelatorioId);

			modelBuilder.Entity<DespesaModel>()
				.HasKey(d => d.ID);

			modelBuilder.Entity<RelatorioModel>()
				.HasMany(r => r.Despesas)
				.WithRequired(d => d.Relatorio)  // Ou .WithOne(d => d.Relatorio) dependendo da versão do Entity Framework
				.HasForeignKey(d => d.RelatorioId)
				.WillCascadeOnDelete(false);





			//modelBuilder.Entity<RelatorioModel>()
			//    .HasMany(r => r.Despesas)
			//    .WithRequired(d => d.Relatorio)
			//    .HasForeignKey(d => d.RelatorioId);



			base.OnModelCreating(modelBuilder);
		}
	}
}
