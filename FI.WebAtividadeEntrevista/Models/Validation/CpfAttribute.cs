using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FI.WebAtividadeEntrevista.Models.Validation
{
    public class CpfAttribute : ValidationAttribute
    {
        public CpfAttribute() { }

        public override bool IsValid(object value)
        {
            if (value == null) return false;

            string cpf = value as string;
            if (!string.IsNullOrEmpty(cpf))
            {
                cpf = cpf.Trim().Replace(".", "").Replace("-", "");

                if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                    return false;

                int[] multiplicadores1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicadores2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int soma = 0;

                for (int i = 0; i < 9; i++)
                    soma += int.Parse(cpf[i].ToString()) * multiplicadores1[i];

                int resto = soma % 11;
                int primeiroDigitoVerificador = resto < 2 ? 0 : 11 - resto;

                if (int.Parse(cpf[9].ToString()) != primeiroDigitoVerificador)
                    return false;

                soma = 0;

                for (int i = 0; i < 10; i++)
                    soma += int.Parse(cpf[i].ToString()) * multiplicadores2[i];

                resto = soma % 11;
                int segundoDigitoVerificador = resto < 2 ? 0 : 11 - resto;

                if (int.Parse(cpf[10].ToString()) != segundoDigitoVerificador)
                    return false;
            }

            return true;
        }
    }
}
