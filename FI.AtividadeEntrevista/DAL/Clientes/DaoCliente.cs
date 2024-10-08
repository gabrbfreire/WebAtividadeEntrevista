﻿using FI.AtividadeEntrevista.DML;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Cliente
    /// </summary>
    internal class DaoCliente : AcessoDados
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal long Incluir(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", cliente.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CEP", cliente.CEP));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Estado", cliente.Estado));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Email", cliente.Email));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", cliente.CPF));

            if (cliente.Beneficiarios.Count > 0)
                parametros.Add(new System.Data.SqlClient.SqlParameter("@BENEFICIARIOS", SqlDbType.Structured)
                {
                    TypeName = "dbo.Beneficiarios",
                    Value = SqlDataRecordsBeneficiarios(cliente.Beneficiarios)
                });

            DataSet ds = base.Consultar("FI_SP_IncClienteV2", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal DML.Cliente Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli.FirstOrDefault();
        }

        internal bool VerificarExistencia(string CPF, long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));

            if (Id != 0)
                parametros.Add(new System.Data.SqlClient.SqlParameter("ID", Id));

            DataSet ds = base.Consultar("FI_SP_VerificaCliente", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        internal List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("iniciarEm", iniciarEm));
            parametros.Add(new System.Data.SqlClient.SqlParameter("quantidade", quantidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("campoOrdenacao", campoOrdenacao));
            parametros.Add(new System.Data.SqlClient.SqlParameter("crescente", crescente));

            DataSet ds = base.Consultar("FI_SP_PesqCliente", parametros);
            List<DML.Cliente> cli = ConverterConsulta(ds);

            int iQtd = 0;

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out iQtd);

            qtd = iQtd;

            return cli;
        }

        /// <summary>
        /// Lista todos os clientes
        /// </summary>
        internal List<DML.Cliente> Listar()
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", 0));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Alterar(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", cliente.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CEP", cliente.CEP));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Estado", cliente.Estado));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Email", cliente.Email));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", cliente.Id));

            base.Executar("FI_SP_AltCliente", parametros);

            foreach (var item in cliente.Beneficiarios)
            {
                parametros = new List<System.Data.SqlClient.SqlParameter>();
                parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", item.Nome));
                parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", item.CPF));
                parametros.Add(new System.Data.SqlClient.SqlParameter("ID", item.Id));

                base.Executar("FI_SP_AltBenef", parametros);
            }

            var beneficiariosCliente = this.Consultar(cliente.Id).Beneficiarios;

            var beneficiariosParaExclusao = beneficiariosCliente.Where(b => cliente.Beneficiarios.All(b2 => b2.Id != b.Id));

            foreach (var item in beneficiariosParaExclusao)
            {
                parametros = new List<System.Data.SqlClient.SqlParameter>();
                parametros.Add(new System.Data.SqlClient.SqlParameter("ID", item.Id));
                base.Executar("FI_SP_DelBenef", parametros);
            }

            var beneficiariosParaInclusao = cliente.Beneficiarios.Where(b => beneficiariosCliente.All(b2 => b2.Id != b.Id));

            foreach (var item in beneficiariosParaInclusao)
            {
                parametros = new List<System.Data.SqlClient.SqlParameter>();
                parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", item.Nome));
                parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", item.CPF));
                parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", cliente.Id));
                base.Executar("FI_SP_IncBenef", parametros);
            }
        }

        /// <summary>
        /// Excluir Cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelCliente", parametros);
        }

        private List<DML.Cliente> Converter(DataSet ds)
        {
            List<DML.Cliente> lista = new List<DML.Cliente>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var beneficiarios = new List<Beneficiario>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row.ItemArray[11].ToString() != "")
                        beneficiarios.Add(
                            new Beneficiario(
                                Id: row.Field<long>("BENEFICIARIO_ID"),
                                Nome: row.Field<string>("BENEFICIARIO_NOME"),
                                CPF: row.Field<string>("BENEFICIARIO_CPF"),
                                IdCliente: row.Field<long>("BENEFICIARIO_IDCLIENTE")
                            )
                        );

                    if (lista.Where(l => l.Id == row.Field<long>("Id")).ToList().Count == 0)
                    {
                        DML.Cliente cli = new DML.Cliente();
                        cli.Id = row.Field<long>("Id");
                        cli.CEP = row.Field<string>("CEP");
                        cli.Cidade = row.Field<string>("Cidade");
                        cli.Email = row.Field<string>("Email");
                        cli.Estado = row.Field<string>("Estado");
                        cli.Logradouro = row.Field<string>("Logradouro");
                        cli.Nacionalidade = row.Field<string>("Nacionalidade");
                        cli.Nome = row.Field<string>("Nome");
                        cli.Sobrenome = row.Field<string>("Sobrenome");
                        cli.Telefone = row.Field<string>("Telefone");
                        cli.CPF = row.Field<string>("CPF");

                        lista.Add(cli);
                    }
                }

                foreach (var item in lista)
                    item.Beneficiarios = beneficiarios.Where(b => b.IdCliente == item.Id).ToList();
            }
            return lista;
        }

        private List<DML.Cliente> ConverterConsulta(DataSet ds)
        {
            List<DML.Cliente> lista = new List<DML.Cliente>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Cliente cli = new DML.Cliente();
                    cli.Id = row.Field<long>("Id");
                    cli.CEP = row.Field<string>("CEP");
                    cli.Cidade = row.Field<string>("Cidade");
                    cli.Email = row.Field<string>("Email");
                    cli.Estado = row.Field<string>("Estado");
                    cli.Logradouro = row.Field<string>("Logradouro");
                    cli.Nacionalidade = row.Field<string>("Nacionalidade");
                    cli.Nome = row.Field<string>("Nome");
                    cli.Sobrenome = row.Field<string>("Sobrenome");
                    cli.Telefone = row.Field<string>("Telefone");
                    cli.CPF = row.Field<string>("CPF");

                    lista.Add(cli);
                }
            }
            return lista;
        }

        private List<SqlDataRecord> SqlDataRecordsBeneficiarios(IList<Beneficiario> beneficiarios)
        {
            List<SqlDataRecord> dataTable = new List<SqlDataRecord>();
            SqlMetaData[] sqlMetaData = new SqlMetaData[3];
            sqlMetaData[0] = new SqlMetaData("CPF", SqlDbType.NVarChar, 512);
            sqlMetaData[1] = new SqlMetaData("Nome", SqlDbType.NVarChar, 512);
            sqlMetaData[2] = new SqlMetaData("IdCliente", SqlDbType.BigInt);

            foreach (var item in beneficiarios)
            {
                SqlDataRecord row = new SqlDataRecord(sqlMetaData);
                row.SetValues(item.CPF, item.Nome, item.IdCliente);
                dataTable.Add(row);
            }

            return dataTable;
        }
    }
}
