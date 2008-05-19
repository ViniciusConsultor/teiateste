using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections;
using SWEDA;
using System.IO.Ports;
using System.Xml;

namespace uniBaterFrenteLoja
{
    class venda
    {
        private string _conn;
        
        public string conn
        {
            get { return _conn; }
            set { _conn = value; }
        }
        
        private MySql objBanco = new MySql();

        public venda(string conection) 
        {
            _conn = conection;
        }
              
        public DataTable retornaFomasPagamentos() 
        {
            string comando = "SELECT nome,cod FROM ubformapag;";
            return objBanco.RetornaDataTable(_conn, CommandType.Text, comando);
        }
       
        public int retornaNumeroCaixa()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return Int32.Parse(doc.GetElementsByTagName("caixa")[0].InnerText);
        }

        public int verificaVendaAberta(char tipo, int loja, int operador, int caixa) 
        {
            MySqlParameter[] par = new MySqlParameter[4];
            par[0] = new MySqlParameter("?vendaCupom",tipo);
            par[1] = new MySqlParameter("?vendaLoja",loja);
            par[2] = new MySqlParameter("?vendaOperador",operador);
            par[3] = new MySqlParameter("?vendaCaixa",caixa);

            string comando = "SELECT vendaCodigo from ubvenda where vendaCupom = ?vendaCupom and vendaLoja = ?vendaLoja and vendaOperador = ?vendaOperador and vendaFinalizada = 0 and vendaCaixa = ?vendaCaixa";
            try
            {
                return Convert.ToInt32(objBanco.RetornaDataRow(_conn, CommandType.Text, comando, par)[0]);
            }
            catch
            {
                return 0;
            }
          
        }
        
        public string montaOrcamento(int cupom, int loja, int caixa, int vendedor) 
        {
            SerialPort porta = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

            StringBuilder topoCupom = new StringBuilder();

            topoCupom.Append("+----------------------------------------------+");
            topoCupom.Append("|            UNIBATER - ORCAMENTO              |");
            topoCupom.Append("|  CUPOM:ccc VENDEDOR:vvv  LOJA:lll CAIXA:xxx  |");
            topoCupom.Append("|  DATA:dd/mm/aaaa              HORA:hh:mm:ss  |");
            topoCupom.Append("+----------------------------------------------+");

            string cabecalho = topoCupom.ToString();

            string numCupom = cupom.ToString("000");
            string numLoja = loja.ToString("000");
            string numVendedor = vendedor.ToString("000");
            string numCaixa = caixa.ToString("000");

            DateTime data = new DateTime();
            data = DateTime.Now;

            string dataOrcamento = data.ToString("dd/MM/yyyy");
            string horaOrcamento = data.ToString("hh:mm:ss");

            cabecalho = cabecalho.Replace("ccc", numCupom);
            cabecalho = cabecalho.Replace("xxx", numCaixa);
            cabecalho = cabecalho.Replace("dd/mm/aaaa", dataOrcamento);
            cabecalho = cabecalho.Replace("hh:mm:ss", horaOrcamento);
            cabecalho = cabecalho.Replace("vvv", numVendedor);
            cabecalho = cabecalho.Replace("lll", numLoja);

            StringBuilder corpoCupom = new StringBuilder();
            corpoCupom.Append("DESCRICAO                 QTD  VL. U.   SUBTOTAL");
            string item = "DDDDDDDDDDDDDDDDDDDDDDDDD QQQ  VVVVVVVV VVVVVVVV";
            DataTable dt = retornaItens(cupom);

            //campos retornados pelo "retornaItens(cupom)"
            //SELECT ubitem.id ID,itemCodigo COD ,itemQuantidade QTDE,itemValorVenda UNIDADE, (itemQuantidade * itemValorVenda) SUBTOTAL,pdDescProd  from ubitem,ubprod where codVenda =27 and itemCodigo = ubprod.id
            foreach (DataRow dr in dt.Rows)
            {
                string descricao = string.Format("{0,-25}",dr["pdDescProd"].ToString());
                descricao = descricao.Replace(' ', '.');
                string novoItem = item.Replace("DDDDDDDDDDDDDDDDDDDDDDDDD", descricao);
                corpoCupom.Append(novoItem);
            }
            
            string corpo = corpoCupom.ToString();

            string cupomCompleto = cabecalho + corpo;

            porta.Open();
            porta.WriteLine(cupomCompleto);
            porta.Close();

            return cabecalho;
        }

        public string retornaNomeLoja(int idLoja) 
        {

            return objBanco.RetornaDataRow(_conn, CommandType.Text, "Select ubloja.ljNome from ubloja where ubloja.id = '" + idLoja + "'")[0].ToString(); ;
        }

        public string retornaNomeFuncionario(int idUser)
        {

            return objBanco.RetornaDataRow(_conn, CommandType.Text, "Select ubfuncionario.fcNome from ubfuncionario where ubfuncionario.id = '" + idUser + "'")[0].ToString(); ;
        }

        public DataRow consultarCliente(string chave,int tipo) 
        {
            if (!(chave == ""))
            {
                try
                {
                    MySqlParameter[] parametros = new MySqlParameter[1];
                    parametros[0] = new MySqlParameter("?chave", chave);
                    string comando = "";
                    if (tipo == 1)
                    {
                        comando = "SELECT id,cfnome,cfdtel,cftel,cfCadastroPJ,cftipocli,cfstatus FROM ubclifor where id = ?chave";
                    }
                    if (tipo == 2)
                    {
                        comando = "SELECT id,cfnome,cfdtel,cftel,cfCadastroPJ,cftipocli,cfstatus FROM ubclifor where cfCadastroPJ = ?chave;";
                    }
                    return objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public decimal retornaLista(int idLoja) 
        {
            string comando = "SELECT valor  from ubloja,ublistacomissao where ubloja.ljLista = ublistacomissao.id and ubloja.id = '" + idLoja + "'";
            return Convert.ToDecimal(objBanco.RetornaDataRow(_conn, CommandType.Text, comando)[0]);
        }

        public int abreVenda(char tipo, decimal lista, int loja, int codCliente, int operador, string veiculo, string placa,int caixa)
        {
            //Gera numero da venda
            int result = Convert.ToInt32(objBanco.RetornaDataRow(_conn, CommandType.Text, "select count(*) from ubvenda").ItemArray[0]);
            int codVenda = ++result;

            MySqlParameter[] par = new MySqlParameter[10];
            par[0] = new MySqlParameter("?vendaCod", codVenda);
            par[1] = new MySqlParameter("?vendaData", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            par[2] = new MySqlParameter("?vendaLoja", loja);
            par[3] = new MySqlParameter("?vendaOperador", operador);
            par[4] = new MySqlParameter("?vendaLista", lista);
            par[5] = new MySqlParameter("?vendaCodCli", codCliente);
            par[6] = new MySqlParameter("?veiculo", veiculo);
            par[7] = new MySqlParameter("?placa", placa);
            par[8] = new MySqlParameter("?tipo", tipo);
            par[9] = new MySqlParameter("?caixa", caixa);

            string comando = "INSERT INTO ubvenda SET vendaCodigo = ?vendaCod, vendaCupom = ?tipo, vendaLoja = ?vendaLoja,vendaCodCli = ?vendaCodCli,vendaData = ?vendaData, vendaOperador = ?vendaOperador,vendaLista = ?vendaLista,vendaVeiculo = ?veiculo,vendaPlaca = ?placa,vendaCaixa = ?caixa";
            objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);

            return codVenda;

        }

        public DataRow retornaDadosCliente(int codVenda) 
        {
            MySqlParameter[] par = new  MySqlParameter[1];
            par[0] = new MySqlParameter("?codVenda",codVenda);

            string comando = "SELECT ubclifor.id,cfNome,cfCadastroPj,cfDTel,cfTel,cfStatus,cfTipoCli,vendaVeiculo,vendaPlaca  from ubvenda, ubclifor where ubvenda.vendaCodCli = ubclifor.id and ubvenda.vendaCodigo = ?codVenda;";
            return objBanco.RetornaDataRow(_conn, CommandType.Text, comando,par);
        }
       
        public DataTable retornaItens(int codVenda) 
        {
            MySqlParameter[] par = new MySqlParameter[1];
            par[0] = new MySqlParameter("?codVenda",codVenda);
            string comando = "SELECT ubitem.id ID,itemCodigo COD ,itemQuantidade QTDE,itemValorVenda UNIDADE, (itemQuantidade * itemValorVenda) SUBTOTAL,pdDescProd  from ubitem,ubprod where codVenda = ?codVenda and itemCodigo = ubprod.id";
           return objBanco.RetornaDataTable(_conn, CommandType.Text, comando, par);
        }

        public decimal retornaSubTotal(int codVenda) 
        {
            try
            {
                MySqlParameter[] par = new MySqlParameter[1];
                par[0] = new MySqlParameter("?codVenda", codVenda);
                string comando = "SELECT sum((itemQuantidade * itemValorVenda)) TOTAL from ubitem where codVenda = ?codVenda;";
                return Convert.ToDecimal(objBanco.RetornaDataRow(_conn, CommandType.Text, comando, par)[0]);
            }
            catch
            {
                return 0;
            }

        }

        public int cancelaItem(int idItem)
        {
            MySqlParameter[] par = new MySqlParameter[1];
            par[0] = new MySqlParameter("?idItem", idItem);

            string comando = "DELETE from ubitem where id = ?idItem";
            return objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);
        }

        /// <summary>
        /// Exclui todos itens de uma venda
        /// </summary>
        /// <param name="codVenda"></param>
        /// <returns>Vetor on o índice [0] Venda</returns>
        public int[] cancelaVenda(int codVenda)
        {
            MySqlParameter[] par = new MySqlParameter[1];
            par[0] = new MySqlParameter("?codVenda", codVenda);

            int[] retorno = new int[3];

            string comando = "DELETE from ubvenda where vendaCodigo = ?codVenda";
            retorno[0] = objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);

            comando = "DELETE from ubitem where codVenda = ?codVenda";
            retorno[1] = objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);

            comando = "DELETE from ubvendapagamento where idVenda = ?codVenda";
            retorno[2] = objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);

            return retorno;
        }

        public void resetVenda(vendaOrcamento frmTela)
        {
            frmTela.txtCodigoCliente.ResetText();
            frmTela.txtCpfCnpj.ResetText();
            frmTela.txtVeiculo.ResetText();
            frmTela.txtPlaca.ResetText();
            frmTela.txtNomeCliente.ResetText();
            frmTela.txtTelefone.ResetText();
            frmTela.txtDicas.Text = "VENDA Nº " + frmTela.numVenda + " CANCELADA!";
            frmTela.dgvItens.DataSource = null;
            frmTela.dgvPagamentos.DataSource = null;
            frmTela.txtTotal.ResetText();
            frmTela.txtTroco.ResetText();
            frmTela.stCodVenda.Text = "CÓDIGO DA VENDA:";
            frmTela.numVenda = 0;
        }
       
        public int cadastraCliente(string nome, string telefone,string cpf,int tipo,int loja)
        {
            string ddd;
            string tel;
            int retorno;
            string tipoCli = "";
            string comando;

            try
            {
               ddd = telefone.Substring(1, 2);
               tel = telefone.Substring(5);
            }
            catch
            {
                ddd = "";
                tel = "";
            }

            int id;

            if (tipo == 0) { tipoCli = "A"; }
            if (tipo == 1) { tipoCli = "V"; }

            comando = "SELECT id FROM ubclifor ORDER BY id DESC limit 1";

            try
            {
                id = Convert.ToInt32(objBanco.RetornaDataRow(_conn, CommandType.Text, comando)[0]);
                id = ++id;
            }
            catch
            {
                id = 1;
            }

            MySqlParameter[] par = new MySqlParameter[7];

            par[0] = new MySqlParameter("?nome", nome);
            par[1] = new MySqlParameter("?ddd", ddd);
            par[2] = new MySqlParameter("?telefone", tel);
            par[3] = new MySqlParameter("?cpf", cpf);
            par[4] = new MySqlParameter("?tipo", tipoCli);
            par[5] = new MySqlParameter("?loja", loja);
            par[6] = new MySqlParameter("?id", id);

            comando = "INSERT INTO ubclifor SET id=?id, cfNome=?nome, cfDtel=?ddd, cfTel=?telefone, cfCadastroPJ=?cpf,cftipoCli=?tipo,cfLoja=?loja, cfStatus = 'D';";
            retorno = objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);

            if (retorno == 1)
            {
                return id;
            }
            else
            {
                return 0;
            }


        }

        public DataRow consultarItem(string chave, int tipo)
        {
            DataRow dr;

            if (!(chave == ""))
            {
                MySqlParameter[] parametros = new MySqlParameter[2];
                parametros[0] = new MySqlParameter("?id", chave);
                parametros[1] = new MySqlParameter("?loja", login.idLoja);

                string comando = "";
                if (tipo == 1)
                {
                    comando = "SELECT pdDescProd,prodquant, cod_prod, pdOriginal, etqvalor, etqCompra,valor lista,ubprod.pdTributo, (etqvalor * (valor  / 100)) valor  FROM ubloja,ubestoque,ubprod,ubestoq_lj,ublistacomissao where ubestoq_lj.cod_lj = ?loja and ubloja.id = ubestoq_lj.cod_lj  and  ubestoq_lj.cod_prod = ubprod.id and ublistacomissao.id = ubloja.ljLista  and etqcod_prod = ubprod.id and ubprod.id = ?id";
                }
                if (tipo == 2)
                {
                    comando = "SELECT pdDescProd,ubprod.id,prodquant, cod_prod, pdOriginal, etqvalor, etqCompra,valor lista,ubprod.pdTributo, (etqvalor * (valor  / 100)) valor  FROM ubloja,ubestoque,ubprod,ubestoq_lj,ublistacomissao where ubestoq_lj.cod_lj = ?loja and ubloja.id = ubestoq_lj.cod_lj  and  ubestoq_lj.cod_prod = ubprod.id and ublistacomissao.id = ubloja.ljLista  and etqcod_prod = ubprod.id and ubprod.pdOriginal = ?id";
                }
                try
                {
                    dr = objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros);
                }
                catch
                {
                    dr = null;
                }
                return dr;
            }
            else 
            {
                return null;

            }

        }

        public int preencheCamposItem(DataRow dr, vendaOrcamento frmTela,int tipo)
        {
            if (tipo == 1)
            {
                frmTela.txtCodBarras.Text = dr["pdOriginal"].ToString();
            }
            if (tipo == 2)
            {
                frmTela.txtCodProduto.Text = dr["id"].ToString();
            }
            
            frmTela.txtAliquota.Text = dr["pdTributo"].ToString();
            frmTela.txtValor.Text = string.Format("{0:n2}", dr["valor"]);
            frmTela.txtDicas.Text = dr["pdDescProd"].ToString();
            decimal qtd = frmTela.numQtd.Value;
            decimal valor = Convert.ToDecimal(dr["valor"]);
            decimal subtotal = qtd * valor;
            frmTela.txtSubTotal.Text = string.Format("{0:n2}",subtotal);
            return 1;

        }

        public void resetCamposItem(vendaOrcamento frmTela,int tipo) 
        {
            if (tipo == 1)
            {
                frmTela.txtCodBarras.ResetText();
            }
            if (tipo == 2)
            {
                frmTela.txtCodProduto.ResetText();
            }
            
            frmTela.txtDicas.ResetText();
            frmTela.txtAliquota.ResetText();
            frmTela.txtValor.ResetText();
            frmTela.txtSubTotal.ResetText();
        }

        public void atualizaSubTotal(vendaOrcamento frmTela)
        {
            decimal qtd = frmTela.numQtd.Value;
            decimal valor = Convert.ToDecimal(frmTela.txtValor.Text);
            decimal subtotal = qtd * valor;
            frmTela.txtSubTotal.Text = string.Format("{0:n2}",subtotal);
        }

        public int inserirItem(int codItem, string codBarras, int codVenda, decimal Qtd, decimal valorBase,decimal valorVenda,string nsBateria)
        {
            MySqlParameter[] parametros= new MySqlParameter[7];

            parametros[0] = new MySqlParameter("?codItem",codItem);
            parametros[1] = new MySqlParameter("?codBarras",codBarras);
            parametros[2] = new MySqlParameter("?codVenda",codVenda);
            parametros[3] = new MySqlParameter("?Qtd",Qtd);
            parametros[4] = new MySqlParameter("?valorBase",valorBase);
            parametros[5] = new MySqlParameter("?valorVenda",valorVenda);
            parametros[6] = new MySqlParameter("?nsBateria", nsBateria);

            string comando = "INSERT INTO ubitem SET itemCodigo=?codItem,itemCodBarras=?codBarras,codVenda=?codVenda,itemQuantidade=?Qtd,itemValorBase=?valorBase,itemValorVenda=?valorVenda,itemNsBateria=?nsBateria";

            return objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, parametros);

        }

        public string efetuaPagamento(int codVenda, decimal total, decimal pagamento, int forma)
        {
            if (pagamento == 0)
            {
                return null;
            }
            
            MySqlParameter[] parametros = new MySqlParameter[4];

            parametros[0] = new MySqlParameter("?forma", forma);
            parametros[1] = new MySqlParameter("?pagamento", pagamento);
            parametros[2] = new MySqlParameter("?codVenda", codVenda);
            parametros[3] = new MySqlParameter("?total", total);

            string comando = "INSERT INTO ubvendapagamento SET formaPagamento=?forma,valorPago=?pagamento,idVenda=?codVenda;";

            objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, parametros);

            comando = "SELECT sum(valorPago) FROM ubvendapagamento where idVenda = ?codVenda";
            
            decimal somaValorPago =  Convert.ToDecimal(objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros)[0]);

            if (somaValorPago >= total)
            {
                comando = "SELECT sum(valorPago) - ?total troco FROM ubvendapagamento where idVenda = ?codVenda";
                string troco = string.Format("{0:d2}",objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros)[0].ToString());
                return "+" + troco;
            }
            else
            {
                comando = "SELECT ?total - sum(valorPago)   troco FROM ubvendapagamento where idVenda = ?codVenda";
                string faltam = string.Format("{0:d2}", objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros)[0].ToString());
                return "-" + faltam;
            }

        }

        public DataTable retornaPagamentosEfetuados(int codVenda)
        {
            MySqlParameter[] parametros = new MySqlParameter[1];

            parametros[0] = new MySqlParameter("?codVenda", codVenda);

            string comando = "SELECT valorPago,nome FROM ubvendapagamento, ubformapag where idVenda = ?codVenda and ubvendapagamento.formaPagamento = ubformapag.id order by ubvendapagamento.id desc";


            return objBanco.RetornaDataTable(_conn, CommandType.Text, comando, parametros);
        }

        public void trataRetornoPagamento(string retornoPag, vendaOrcamento frmTela, int codVenda) 
        {
            string flag = retornoPag.Substring(0, 1);
            string valor = retornoPag.Substring(1);


            if (flag == "+") 
            {
                //montaOrcamento(int cupom, int loja, int caixa, int vendedor) 
                
                montaOrcamento(codVenda, login.idLoja, frmTela.caixa, login.idUsuario);

                frmTela.txtTroco.Text = valor;

                frmTela.txtDicas.Text = "OBRIGADO, VOLTE SEMPRE!";

                MySqlParameter[] parametros = new MySqlParameter[1];

                parametros[0] = new MySqlParameter("?codVenda", codVenda);

                string comando = "UPDATE ubvenda SET  vendaFinalizada = 1 WHERE vendaCodigo = ?codVenda;";

                objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, parametros);

                frmTela.numVenda = 0;

            }

            if (flag == "-")
            { 
                frmTela.txtDicas.Text = "Faltam: R$" + valor;
                frmTela.txtTroco.Text = "0,00";

            }


        }
    
    }
}





