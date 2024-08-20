using FI.WebAtividadeEntrevista.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Beneficiario
    /// </summary>
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        public long IdCliente { get; set; }

        [Required]
        [CpfAttribute(ErrorMessage = "Digite um CPF válido")]
        public string CPF { get; set; }

        [Required]
        public string Nome { get; set; }
    }
}