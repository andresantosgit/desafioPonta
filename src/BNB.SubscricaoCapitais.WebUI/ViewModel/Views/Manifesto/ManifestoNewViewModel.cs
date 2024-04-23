//-------------------------------------------------------------------------------------
// <copyright file="DemandaAuditoriaNewViewModel.cs" company="BNB">
//    Copyright statement. All right reserved.
// </copyright>
// <summary>
//   View Model.
// </summary>
//-------------------------------------------------------------------------------------


using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BNB.ProjetoReferencia.WebUI.ViewModel.Views.Manifesto
{
    /// <summary>
    /// ViewModel da view DemandaAuditoriaNew.
    /// </summary>
    public class ManifestoNewViewModel
    {
        public int? Id { get; set; }

        /// <summary>
        /// CPF ou CNPJ
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [StringLength(20)]
        [Display(Name = "CPF/CNPJ")]
        [Required(ErrorMessage = "O campo 'CPF/CNPJ' é obrigatório.")]
        public string? CPFOuCNPJ { get; set; }

        /// <summary>
        /// Tipo de pessoa
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [Display(Name = "Tipo de pessoa")]
        [Required(ErrorMessage = "O campo 'Tipo de pessoa' é obrigatório.")]
        public int? TipoPessoa { get; set; }

        /// <summary>
        /// Nome do investidor
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [StringLength(100)]
        [Display(Name = "Nome do investidor")]
        [Required(ErrorMessage = "O campo 'Nome do investidor' é obrigatório.")]
        public string NomeInvestidor { get; set; }


        /// <summary>
        /// Tipo demanda auditoria
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [Display(Name = "Tipo de custódia")]
        [Required(ErrorMessage = "O campo 'Tipo de custódia' é obrigatório.")]
        public int? TipoCustodia { get; set; }

        /// <summary>
        /// Endereço do investidor
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [StringLength(100)]
        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O campo 'Endereço' é obrigatório.")]
        public string? Endereco { get; set; }

        /// <summary>
        /// Endereço do investidor
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [StringLength(100)]
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O campo 'E-mail' é obrigatório.")]
        public string? Email { get; set; }

        /// <summary>
        /// Telefone do investidor
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [StringLength(15)]
        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O campo 'Telefone' é obrigatório.")]
        public string? Telefone { get; set; }

        /// <summary>
        /// Quantidade
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "O campo 'Quantidade' é obrigatório.")]
        public int? Quantidade { get; set; }

        /// <summary>
        /// Quantidade máxima
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [Display(Name = "Quantidade máxima")]
        public int? QuantidadeMaxima { get; set; }

        /// <summary>
        /// Valor da ação
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [DataType(DataType.Currency)]
        [Display(Name = "Valor da ação")]
        [Required(ErrorMessage = "O campo 'Valor da ação' é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal ValorAcao { get; set; }


        /// <summary>
        /// Valor total
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [DataType(DataType.Currency)]
        [Display(Name = "Valor total")]
        [Required(ErrorMessage = "O campo 'Valor total' é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Telefone do investidor
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        [Display(Name = "Solicitante")]
        [Required(ErrorMessage = "O campo 'Solicitante' é obrigatório.")]
        public string? MatriculaSolicitante { get; set; }

        public string? Status { get; set; }

        public DateTime? DataCriacao { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public string TipoPessoaText
        {
            get
            {
                return (this.TipoPessoa.HasValue && this.TipoPessoa == 2) ? "Jurídica" : "Física";
            }
        }

        public string DataCriacaoText
        {
            get
            {
                return this.DataCriacao.HasValue ? this.DataCriacao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DataAtualizacaoText
        {
            get
            {
                return this.DataAtualizacao.HasValue ? this.DataAtualizacao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string ValorAcaoText
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", this.ValorAcao);
            }
        }

        public string ValorTotalText
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", this.ValorTotal);
            }
        }
    }
}