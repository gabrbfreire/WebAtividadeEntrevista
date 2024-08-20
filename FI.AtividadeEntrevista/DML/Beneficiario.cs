namespace FI.AtividadeEntrevista.DML
{
    /// <summary>
    /// Classe de cliente que representa o registo na tabela Beneficiario do Banco de Dados
    /// </summary>
    public class Beneficiario
    {
        public long Id { get; set; }
        public long IdCliente { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }

        public Beneficiario(string CPF, string Nome, long IdCliente = 0)
        {
            this.CPF = CPF;
            this.Nome = Nome;
            this.IdCliente = IdCliente;
        }
    }
}
