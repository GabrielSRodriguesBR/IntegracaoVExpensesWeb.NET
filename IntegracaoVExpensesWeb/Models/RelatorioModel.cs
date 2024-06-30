using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegracaoVExpensesWeb.Models
{
    [Table("RELATORIOS")]
    public class RelatorioModel
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column("DATA_INTEGRACAO")]
        public DateTime DataIntegracao { get; set; }

        [Key]
        [Column("RELATORIO_ID", Order = 1)]
        public int RelatorioId { get; set; }

        [Column("DESCRICAO")]
        public string Descricao { get; set; }
        [Column("OBSERVACAO")]
        public string Observacao { get; set; }
        [Column("USUARIO")]
        public string Usuario { get; set; }
        [Column("TIPO_USUARIO")]
        public string TipoUsuario { get; set; }
        [Column("USUARIO_ID_SAP")]
        public string UsuarioIdSAP { get; set; }
        [Column("DOCENTRY")]
        public int? DocEntry { get; set; }
        [Column("DATA_PGTO")]
        public DateTime? DataPagamento { get; set; }

        public ICollection<DespesaModel> Despesas { get; set; }
    }

    public class RelatorioFilterViewModel
    {
        [Display(Name = "ID Relatório")]
        public int? RelatorioId { get; set; }

        [Display(Name = "Status Pagamento")]
        public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Todos;

        [Display(Name = "Data Inicio")]
        public string DataInicio { get; set; } = DateTime.Now.ToString("yyyy-MM");

        [Display(Name = "Data Fim")]
        public string DataFim { get; set; } = DateTime.Now.ToString("yyyy-MM");

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Display(Name = "Status Integração")]
        public StatusIntegracao StatusIntegracao { get; set; } = StatusIntegracao.Todos;

        public List<string> Usuarios { get; set; }
        public List<RelatorioModel> Relatorios { get; set; }
    }

    public enum StatusPagamento
    {
        [Display(Name = "Todos")]
        Todos,
        [Display(Name = "Pagos")]
        Pago,
        [Display(Name = "Não Pagos")]
        NaoPago
    }

    public enum StatusIntegracao
    {
        [Display(Name = "Todos")]
        Todos,
        [Display(Name = "Integrados")]
        Integrado,
        [Display(Name = "Não Integrados")]
        NaoIntegrado
    }
}
    