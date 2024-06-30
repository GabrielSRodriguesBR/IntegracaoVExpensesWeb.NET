using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegracaoVExpensesWeb.Models
{
    [Table("DESPESAS")]
    public class DespesaModel
    {
        [Key]
        public int ID { get; set; }

        [Column("DESPESA_ID")]
        public int DespesaId { get; set; }

        [Column("RELATORIO_ID")]
        public int RelatorioId { get; set; }

        [Column("DATA")]
        public DateTime Data { get; set; }

        [Column("TITULO")]
        public string Titulo { get; set; }

        [Column("VALOR")]
        public decimal Valor { get; set; }

        [Column("OBSERVACAO")]
        public string Observacao { get; set; }

        [Column("TIPO")]
        public string Tipo { get; set; }

        [Column("TIPO_ID_SAP")]
        public string TipoIdSAP { get; set; }

        [Column("CENTROCUSTO")]
        public string CentroCusto { get; set; }

        [Column("CENTROCUSTO_ID_SAP")]
        public string CentroCustoIdSAP { get; set; }

        public string URL { get; set; }

        [ForeignKey("RelatorioId")]
        public RelatorioModel Relatorio { get; set; }

    }
}