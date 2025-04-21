using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegracaoVExpensesWeb.Models
{
    [Table("CONFIGURACOES")]
    public class ConfiguracaoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("TRANSACTION_CODE")]
        public string TransactionCode { get; set; }

        [Column("BPLID")]
        public int BPLID { get; set; }

        [Column("ACCOUNT_CODE")]
        public string AccountCode { get; set; }

        [Column("PROFIT_CODE")]
        public string ProfitCode { get; set; }

        [Column("OCR_CODE3")]
        public string OcrCode3 { get; set; }
    }
}
