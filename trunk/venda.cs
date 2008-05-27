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

        public int retornaPortaNaoFiscal()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return Int32.Parse(doc.GetElementsByTagName("portaNaoFiscal")[0].InnerText);
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

        public DataTable retornaMensagem() 
        {
            string comando = "SELECT * FROM ubmensagem_rodape";
            return objBanco.RetornaDataTable(_conn, CommandType.Text, comando);
        }
        
        public string montaOrcamento(int cupom, int loja, int caixa, int vendedor) 
        {
            string crlf = ((char)13).ToString() + ((char)10).ToString();

            SerialPort porta = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

            StringBuilder topoCupom = new StringBuilder();

            string comando = "SELECT ljNome,ljEndereco,ljCidade,ljUf,ljCgc,ljInsEstadual from ubLoja WHERE id = " + loja;
            DataRow drLoja = objBanco.RetornaDataRow(_conn, CommandType.Text, comando);


            topoCupom.Append("LOJA: "+ drLoja["ljNome"] + crlf);
            topoCupom.Append(drLoja["ljEndereco"] + crlf);
            topoCupom.Append(drLoja["ljCidade"]+ "-"+ drLoja["ljUf"] + crlf);
            topoCupom.Append("CNPJ: " + drLoja["ljCgc"] + crlf);
            topoCupom.Append("IE: " + drLoja["ljInsEstadual"] + crlf);
            topoCupom.Append("+----------------------------------------------+" + crlf);
            topoCupom.Append("|  CUPOM:cccccc  VEND.:vvv  LOJA:lll CAIXA:xxx |" + crlf);
            topoCupom.Append("|  DATA:dd/mm/aaaa              HORA:hh:mm:ss  |" + crlf);
            topoCupom.Append("+----------------------------------------------+" + crlf);

            string cabecalho = topoCupom.ToString();

            string numCupom = cupom.ToString("000000");
            string numLoja = loja.ToString("000");
            string numVendedor = vendedor.ToString("000");
            string numCaixa = caixa.ToString("000");

            DateTime data = new DateTime();
            data = DateTime.Now;

            string dataOrcamento = data.ToString("dd/MM/yyyy");
            string horaOrcamento = data.ToString("hh:mm:ss");

            cabecalho = cabecalho.Replace("cccccc", numCupom);
            cabecalho = cabecalho.Replace("xxx", numCaixa);
            cabecalho = cabecalho.Replace("dd/mm/aaaa", dataOrcamento);
            cabecalho = cabecalho.Replace("hh:mm:ss", horaOrcamento);
            cabecalho = cabecalho.Replace("vvv", numVendedor);
            cabecalho = cabecalho.Replace("lll", numLoja);

            StringBuilder corpoCupom = new StringBuilder();
            corpoCupom.Append("DESCRICAO             QTD   VL.UN.    SUBTOTAL  ");
                        //"+----------------------------------------------+"
            string item = "DDDDDDDDDDDDDDDDDDDDD QQQ x VVVVVVVVV SSSSSSSSSS" + crlf;
            DataTable dt = retornaItens(cupom);

            //campos retornados pelo "retornaItens(cupom)"
            //SELECT ubitem.id ID,itemCodigo COD ,itemQuantidade QTDE,itemValorVenda UNIDADE, (itemQuantidade * itemValorVenda) SUBTOTAL,pdDescProd DESC  from ubitem,ubprod where codVenda =27 and itemCodigo = ubprod.id

            decimal TotalCupom = 0;
            
            foreach (DataRow dr in dt.Rows)
            {
                string descricao = string.Format("{0,-21}",dr["DESCRICAO"]);
                string qtd = Convert.ToInt32(dr["QTDE"]).ToString("000");
                string vlUnitario = string.Format("{0,9}", dr["UNIDADE"]);
                string vlSub = string.Format("{0,10}", dr["SUBTOTAL"]);

                descricao = descricao.Replace(' ', '.');
                string novoItem = item.Replace("DDDDDDDDDDDDDDDDDDDDD", descricao);
                novoItem = novoItem.Replace("QQQ", qtd);
                novoItem = novoItem.Replace("VVVVVVVVV", vlUnitario);
                novoItem = novoItem.Replace("SSSSSSSSSS", vlSub);

                corpoCupom.Append(novoItem);
                TotalCupom = TotalCupom + Convert.ToDecimal(dr["UNIDADE"]) * Convert.ToDecimal(dr["QTDE"]);
            }

            string corpo = corpoCupom.ToString();
            


            StringBuilder rodapeCupom = new StringBuilder();
            rodapeCupom.Append("------------------------------------------------" + crlf);
            
            dt = retornaPagamentosEfetuados(cupom);

            decimal TotalPago = 0;

            foreach (DataRow dr in dt.Rows)
            {
                string forma = string.Format("{0,16}", dr[1].ToString());
                string valor = string.Format("{0,12}", dr[0].ToString());
                string novaForma = "                  FFFFFFFFFFFFFFF: TTTTTTTTTTTT" + crlf;
                novaForma = novaForma.Replace("FFFFFFFFFFFFFFF", forma);
                novaForma = novaForma.Replace("TTTTTTTTTTTT", valor);
                rodapeCupom.Append(novaForma);
                TotalPago = TotalPago + Convert.ToDecimal(dr[0]);
            }
            rodapeCupom.Append("------------------------------------------------" + crlf);
            string LinhaTotal = "                             TOTAL: TTTTTTTTTTTT" + crlf;
            string valorTotal = string.Format("{0,12}",TotalCupom);
            LinhaTotal = LinhaTotal.Replace("TTTTTTTTTTTT", valorTotal);

            string rodape = "------------------------------------------------";
            rodape = rodape + LinhaTotal;
            string LinhaPAGO = "                        TOTAL PAGO: TTTTTTTTTTTT" + crlf;
            string valorTotalPago = string.Format("{0,12}",TotalPago);
            LinhaPAGO = LinhaPAGO.Replace("TTTTTTTTTTTT", valorTotalPago);
            rodapeCupom.Append(LinhaPAGO);

            decimal Troco = TotalPago - TotalCupom;
            string valorTroco = string.Format("{0,12}", Troco);
            string LinhaTroco = "                             TROCO: TTTTTTTTTTTT" + crlf;
            LinhaTroco = LinhaTroco.Replace("TTTTTTTTTTTT", valorTroco);
            rodapeCupom.Append(LinhaTroco);
            rodapeCupom.Append("------------------------------------------------" + crlf);


            //DADOS DO COMPRADOR

            DataRow drCliente = retornaDadosCliente(cupom);
            rodapeCupom.Append("CLIENTE: "+drCliente["cfNome"] + crlf);
            rodapeCupom.Append("DOC.: " + drCliente["cfCadastroPJ"] + crlf);
            rodapeCupom.Append("ASSINATURA:_____________________________________"+ crlf);
            rodapeCupom.Append("------------------------------------------------" + crlf);


            dt = retornaMensagem();
            foreach (DataRow dr in dt.Rows)
            {
                string mensagem = string.Format("{0,-48}", dr["mensagem"]);
                rodapeCupom.Append(mensagem);
            }
            rodapeCupom.Append("------------------------------------------------"+ crlf);

            for (int i=0; i <= 5; i++)
            {
                rodapeCupom.Append("                                                ");
            }

            rodape = rodape + rodapeCupom.ToString();

            string cupomCompleto = cabecalho + corpo + rodape ;
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
            string comando = "SELECT ubitem.id ID,itemCodigo COD ,itemQuantidade QTDE,itemValorVenda UNIDADE, (itemQuantidade * itemValorVenda) SUBTOTAL,pdDescProd DESCRICAO  from ubitem,ubprod where codVenda = ?codVenda and itemCodigo = ubprod.id";
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

        public int verificaEstoque(int loja,int venda,int produto,int qtd)
        {
            MySqlParameter[] parametros = new MySqlParameter[4];

            parametros[0] = new MySqlParameter("?codLoja", loja);
            parametros[1] = new MySqlParameter("?codVenda", venda);
            parametros[2] = new MySqlParameter("?codProd", produto);
            parametros[3] = new MySqlParameter("?qtd", qtd);

            string comando = "SELECT prodquant - (SUM(itemQuantidade) + ?qtd) qtd FROM ubestoq_lj,ubitem WHERE cod_lj = ?codLoja AND cod_prod = ?codProd AND ubitem.codVenda = ?codVenda GROUP by codvenda;";
            return Convert.ToInt32(objBanco.RetornaDataRow(_conn, CommandType.Text, comando, parametros)[0]);
        }

        public int inserirItem(int codItem, string codBarras, int codVenda, decimal Qtd, decimal valorBase,decimal valorVenda,string nsBateria)
        {
            int estoque = verificaEstoque(login.idLoja, codVenda, codItem, Convert.ToInt32(Qtd));

            if (estoque <= 0)
            {
                MessageBox.Show(estoque.ToString());
            }
          
            
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

        public void trataRetornoPagamento(string retornoPag, vendaOrcamento frmTela, int codVenda,int nota) 
        {
            string flag = retornoPag.Substring(0, 1);
            string valor = retornoPag.Substring(1);


            if (flag == "+") 
            {

                if (nota == 0)
                {
                    montaOrcamento(codVenda, login.idLoja, frmTela.caixa, login.idUsuario);
                }

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

        public int novaCompra(string nome,string doc,int loja,int func) 
        {
            //comando = "INSERT INTO ubvencomsucata SET id=?id, sunome=?nome, suPj=?doc, suDia=?data, suLoja=?loja, suFunc=?funcionario,sutipo='1'";
            //comando = "SELECT  ubitemsucata.id 'ITEM',itvencomsucata 'COMPRA',suprod 'PRODUTO',itprod 'COD. PROD.',itquant 'QUANTIDADE', itvalor 'VALOR',itvalor * itquant  SUBTOTAL  FROM ubitemsucata,ubsucataprod WHERE itvencomsucata = ?venda and ubitemsucata.itprod = ubsucataprod.id order by ubitemsucata.id desc  ";
            //comando = "SELECT sum(itquant * itvalor) SUBTOTAL from ubitemsucata where itvencomsucata = ?venda";
            string comando = "SELECT id FROM ubvencomsucata ORDER BY id DESC";
            int codComVen;
                MySqlParameter[] par = new MySqlParameter[6];
            try
            {
                DataRow drIdVenda = objBanco.RetornaDataRow(_conn, CommandType.Text, comando);
                codComVen = Convert.ToInt32(drIdVenda["id"]);
                codComVen = ++codComVen;
                par[0] = new MySqlParameter("?id",codComVen);
                par[1] = new MySqlParameter("?nome",nome);
                par[2] = new MySqlParameter("?doc",doc);
                par[3] = new MySqlParameter("?data",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                par[4] = new MySqlParameter("?loja",loja);
                par[5] = new MySqlParameter("?funcionario", func);

                comando = "INSERT INTO ubvencomsucata SET id=?id, sunome=?nome, suPj=?doc, suDia=?data, suLoja=?loja, suFunc=?funcionario, sutipo='1'";
                objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando,par);
            }
            catch
            {
                codComVen = 1;
                par[0] = new MySqlParameter("?id", codComVen);
                par[1] = new MySqlParameter("?nome",nome);
                par[2] = new MySqlParameter("?doc",doc);
                par[3] = new MySqlParameter("?data",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                par[4] = new MySqlParameter("?loja",loja);
                par[5] = new MySqlParameter("?funcionario", func);

                comando = "INSERT INTO ubvencomsucata SET id=?id, sunome=?nome, suPj=?doc, suDia=?data, suLoja=?loja, suFunc=?funcionario, sutipo='1'";
                objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando,par);
            }

            listaCompraSucata(codComVen);
            return codComVen;
                      

        }

        public void retornaModelosSucata(ComboBox combo) 
        {
            string comando = "SELECT id,suprod FROM ubsucataprod";
            DataTable  dtLista   = objBanco.RetornaDataTable(_conn,CommandType.Text,comando);
            combo.ValueMember = dtLista.Columns[0].ToString();
            combo.DisplayMember = dtLista.Columns[1].ToString();
            combo.DataSource = dtLista;
        }

       public DataTable listaCompraSucata(int codComVen)
       {
            string  comando = "SELECT  ubitemsucata.id 'ITEM',itvencomsucata 'COMPRA',suprod 'PRODUTO',itprod 'COD. PROD.',itquant 'QUANTIDADE', itvalor 'VALOR',itvalor * itquant  SUBTOTAL  FROM ubitemsucata,ubsucataprod WHERE itvencomsucata = '" + codComVen + "' and ubitemsucata.itprod = ubsucataprod.id order by ubitemsucata.id desc  ";
            return objBanco.RetornaDataTable(_conn, CommandType.Text, comando);
       }

        public DataTable insereSucata(int tipoBateria, decimal quantidade, decimal valor, int baseTroca,int compra)
        {
            MySqlParameter[] par = new MySqlParameter[5];
            par[0] = new MySqlParameter("?itprod", tipoBateria);
            par[1] = new MySqlParameter("?itquant", quantidade);
            par[2] = new MySqlParameter("?itvalor", valor);
            par[3] = new MySqlParameter("?itvencomsucata", compra);
            par[4] = new MySqlParameter("?baseTroca", baseTroca);

            string comando = "INSERT INTO ubitemsucata SET itprod=?itprod, itquant=?itquant, itvalor=?itvalor, itvencomsucata=?itvencomsucata, baseTroca = ?baseTroca";
            objBanco.ExecuteNonQuery(_conn, CommandType.Text, comando, par);
            return listaCompraSucata(compra);
        }
    
    }
}





