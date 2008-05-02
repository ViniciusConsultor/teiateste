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


namespace uniBaterFrenteLoja
{
    public partial class Form1 : Form
    {
        public string conexao = "Server=colocation1.teiapp.com.br;Database=unibater;Uid=root;Pwd=bd@teia;";
        public string lista;
        public string clienteEncontrado = "0";
        public string cupomAberto = "0";
        public int status;
        public StringBuilder coo = new StringBuilder(6);
        decimal valorBase;
        public int codVenda;
        string quantidadeEmEstoque;
        int qtdAtual;
        MySql objBanco = new MySql();
        string sucataSelecionada = "";
        StringBuilder caixa = new StringBuilder(4);
        string itemSel;



        //retornoEcf
        int ack, st1, st2, st3;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            int abrePorta = ECFSWEDA.ECF_AbrePortaSerial();
            if (abrePorta == 1) {
                ECFSWEDA.ECF_ZAUTO("1");
                MessageBox.Show("Porta Aberta!");
            }

            ECFSWEDA.ECF_NumeroCaixa(caixa);

            try
            {
                MySqlParameter[] parametros1 = new MySqlParameter[2];

                parametros1[0] = new MySqlParameter("?caixa", caixa.ToString());
                parametros1[1] = new MySqlParameter("?loja", login.idLoja);

                string comandoT = "select vendaCodigo, vendaCoo,VendaCodCli from ubvenda where caixa = ?caixa and vendaLoja = ?loja and vendaFinalizada <> 1 and vendaFinalizada <> 2 and vendaCupom ='C' order by vendaCodigo desc limit 1";
                DataRow drCupom =  objBanco.RetornaDataRow(conexao, CommandType.Text, comandoT, parametros1);

                codVenda = Convert.ToInt32(drCupom["vendaCodigo"]);
                stCodVenda.Text = "CÓDIGO DA VENDA: " + codVenda.ToString();
                string cooCupom = drCupom["vendaCoo"].ToString();
                lblCoo.Text = "COO: " + cooCupom;



                atualizarItensCancelar(codVenda);

                MySqlParameter[] parametros2 = new MySqlParameter[1];

                parametros2[0] = new MySqlParameter("?Cliente", drCupom[2].ToString());


                string comandoC = "select id,cfNome, cfDTel, cfTel, cfCadastroPJ, cftipoCli,cfStatus  from ubclifor where id = ?Cliente";
                DataRow drCliente = objBanco.RetornaDataRow(conexao, CommandType.Text, comandoC, parametros2);
                txtCodigoCliente.Text = drCliente["id"].ToString();
                txtNomeCliente.Text = drCliente["cfNome"].ToString().ToUpper();
                txtTelefone.Text = drCliente["cfDTel"].ToString().ToUpper() + drCliente["cfTel"].ToString().ToUpper();
                txtCpfCnpj.Text = drCliente["cfCadastroPJ"].ToString().ToUpper();
                if (drCliente[4].ToString().ToUpper() == "V")
                {
                    cbTipoCliente.SelectedIndex = 1;
                }
                if (drCliente["cftipoCli"].ToString().ToUpper() == "A")
                {
                    cbTipoCliente.SelectedIndex = 0;
                }

                if (drCliente["cfStatus"].ToString().ToUpper() == "B")
                {
                    cbBloquear.SelectedIndex = 0;
                }
                if (drCliente["cfStatus"].ToString().ToUpper() == "D")
                {
                    cbBloquear.SelectedIndex = 1;
                }

                
                
                
             
            }
            catch
            {

            }



            ArrayList ar = new ArrayList();
            ar.Add(new combo("Bloqueado","B"));
            ar.Add(new combo("Desbloqueado", "D"));

            cbBloquear.DisplayMember = "Nome";
            cbBloquear.ValueMember = "Valor";
            cbBloquear.DataSource = ar;

            cbBloquear.SelectedIndex = 1;

            ar = new ArrayList();
            ar.Add(new combo("Atacadista", "A"));
            ar.Add(new combo("Varejista", "V"));

            cbTipoCliente.DisplayMember = "Nome";
            cbTipoCliente.ValueMember = "Valor";
            cbTipoCliente.DataSource = ar;

            cbTipoCliente.SelectedIndex = 1;

            cbTroca.SelectedIndex = 1;


        
            conexao con = new conexao();

            MySqlParameter[] parametros = new MySqlParameter[2];

            parametros[0] = new MySqlParameter("?idl",login.idLoja);
            parametros[1] = new MySqlParameter("?idf", login.idUsuario);

            //DataRow dr = con.executarComando("Select ubloja.ljNome,ubfuncionario.fcNome from ubloja,ubfuncionario where ubloja.id = ?idl and ubfuncionario.id =?idf", parametros);


            DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "Select ubloja.ljNome,ubfuncionario.fcNome,ubloja.ljLista from ubloja,ubfuncionario where ubloja.id = ?idl and ubfuncionario.id =?idf", parametros);

            stLoja.Text = "LOJA: " + dr[0].ToString().ToUpper();
            stOperador.Text = "OPERADOR: " + dr[1].ToString().ToUpper();

            //Atualiza hora na barra de status
            timerHora.Tick += new EventHandler(timerHora_Tick);

            con.preencheCB(cbTipoBateria, "select sukgunid,suprod from ubsucataprod;",conexao);
            con.preencheCB(cbFormaPagamento, "select nome,nome from ubformapag;",conexao);

            getListaComissao();

        }

        public void getListaComissao() {
            MySqlParameter[] lojaParametros = new MySqlParameter[1];
            lojaParametros[0] = new MySqlParameter("?lojaId", login.idLoja);
            lista = objBanco.RetornaDataRow(conexao, CommandType.Text, "select valor from ubloja,ubListaComissao where ubloja.id = ?lojaId and ubListaComissao.id = ubloja.ljLista", lojaParametros).ItemArray[0].ToString();
        }

        void timerHora_Tick(object sender, EventArgs e)
        {
            bsHorario.Text = DateTime.Now.ToString("'DATA': dd/MM/yy  'HORA': HH:mm:ss");
        }




  
        private void txtCodigoCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            int abrePorta = ECFSWEDA.ECF_FechaPortaSerial();
            Application.Exit();

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtValorCompra_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            comprarBateria();
        }

        public bool compraSucata = false;
        string comando;
        int result;
        int codCompraBateria;

        public void comprarBateria(){
            //Gera numero compra
            

            if (compraSucata == false)
            {
                result = Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select id from ubvencomsucata order by id desc").ItemArray[0]);
                codCompraBateria = ++result;

                tsCodigoCompra.Text = "CÓDIGO DA COMPRA: " + codCompraBateria;

                ///compra sucata
                MySqlParameter[] parametros = new MySqlParameter[6];
                parametros[0] = new MySqlParameter("?id", codCompraBateria);
                parametros[1] = new MySqlParameter("?nome", txtNomeCliente.Text);
                parametros[2] = new MySqlParameter("?doc", txtCpfCnpj.Text);
                parametros[3] = new MySqlParameter("?data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                parametros[4] = new MySqlParameter("?loja", login.idLoja);
                parametros[5] = new MySqlParameter("?funcionario", login.idUsuario);


                comando = "INSERT INTO ubvencomsucata SET id=?id, sunome=?nome, suPj=?doc, suDia=?data, suLoja=?loja, suFunc=?funcionario,sutipo='1'";
                objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametros);
                compraSucata = true;
            }

            int codProduto = cbTipoBateria.SelectedIndex;
            codProduto = ++codProduto;
            //item sucata
            MySqlParameter[] parametrosItem = new MySqlParameter[5];
            parametrosItem[0] = new MySqlParameter("?itprod", codProduto);
            parametrosItem[1] = new MySqlParameter("?itquant", numQtdCompra.Value);
            parametrosItem[2] = new MySqlParameter("?itvalor", txtValorCompra.Text.Replace(",","."));
            parametrosItem[3] = new MySqlParameter("?itvencomsucata", codCompraBateria);
            parametrosItem[4] = new MySqlParameter("?baseTroca", cbTroca.SelectedIndex.ToString());


            comando = "INSERT INTO ubitemsucata SET itprod=?itprod, itquant=?itquant, itvalor=?itvalor, itvencomsucata=?itvencomsucata, baseTroca = ?baseTroca";
            objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametrosItem);

            atualizarSucata();


            MySqlParameter[] parEstoq = new MySqlParameter[2];
            parEstoq[0] = new MySqlParameter("?loja", login.idLoja);
            parEstoq[1] = new MySqlParameter("?produto", codProduto);

            comando = "select count(id) from  ubestoq_sucata where esljsu = ?loja and esprodsu = ?produto";
            int numLinhas = Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, comando, parEstoq)[0]);

            MySqlParameter[] parEstoqIns = new MySqlParameter[3];
            parEstoqIns[0] = new MySqlParameter("?loja", login.idLoja);
            parEstoqIns[1] = new MySqlParameter("?produto", codProduto);
            parEstoqIns[2] = new MySqlParameter("?qtd", numQtdCompra.Value);

            if (numLinhas < 1)
            {
                comando = "Insert into ubestoq_sucata set esprodsu = ?produto, esljsu = ?loja, esquantsu = ?qtd";
                objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parEstoqIns);
            }
            else 
            {
                comando = "Update ubestoq_sucata set   esquantsu = esquantsu + ?qtd where esprodsu = ?produto and  esljsu = ?loja";
                objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parEstoqIns);
            }


        
        }

        private void atualizarSucata ()
        {
            MySqlParameter[] parametrosDGV= new MySqlParameter[1];
            parametrosDGV[0] = new MySqlParameter("?venda", codCompraBateria);

            comando = "SELECT  ubitemsucata.id 'ITEM',itvencomsucata 'COMPRA',suprod 'PRODUTO',itprod 'COD. PROD.',itquant 'QUANTIDADE', itvalor 'VALOR',itvalor * itquant  SUBTOTAL  FROM ubitemsucata,ubsucataprod WHERE itvencomsucata = ?venda and ubitemsucata.itprod = ubsucataprod.id order by ubitemsucata.id desc  ";
            DataTable tabelaSucata = new DataTable();
            tabelaSucata = objBanco.RetornaDataTable(conexao, CommandType.Text, comando, parametrosDGV);
            dgvBateriasCompra.DataSource = tabelaSucata;

            comando = "select sum(itquant * itvalor) SUBTOTAL from ubitemsucata where itvencomsucata = ?venda";
            string SubTotalCompraSucata = objBanco.RetornaDataRow(conexao, CommandType.Text, comando, parametrosDGV)[0].ToString();
            txtSubTotalCompraSucata.Text = SubTotalCompraSucata;
        }

        private void txtValorCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ao apertar BackSpace limpar textbox
            if (e.KeyChar == (Char)Keys.Back)
            {
                txtValorCompra.Text = "";
            }
            //Caso seja digitado um ponto coloca-se uma vírgula
            if (e.KeyChar == ((char)(46)))
            {
                e.KeyChar = (char)(44);
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)Keys.Back && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            //pega a posição da virgula, caso ela exista:
            int posSeparator = txtValorCompra.Text.IndexOf(',');

            //se a tecla digitada for virgula e ela já existir, barra:
            if (e.KeyChar == ',' && posSeparator > -1)
            {
                e.Handled = true;
                return;
            }

            //verifica quantos digitos há após a vírgula
            if (txtValorCompra.Text.Contains(","))
            {
                string selecao = txtValorCompra.Text.Substring(txtValorCompra.SelectionStart);
                if (selecao.Contains(","))
                {
                }
                else
                {
                    int tamanho = txtValorCompra.Text.Length;
                    posSeparator = ++posSeparator;
                    string depoisVirgula = txtValorCompra.Text.Substring(posSeparator);

                    if (depoisVirgula.Length > 1)
                    {

                        e.Handled = true;

                        return;
                    }
                }
            }

       
        }

        private void txtValorCompra_Click(object sender, EventArgs e)
        {
            txtValorCompra.SelectAll();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtCodigoCliente_KeyPress(object sender, KeyEventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[1];

            parametros[0] = new MySqlParameter("?id", txtCodigoCliente.Text);

            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT cfnome,cfdtel,cftel,cfCadastroPJ,cftipocli,cfstatus from ubclifor where id = ?id;", parametros);
                txtNomeCliente.Text = dr[0].ToString().ToUpper();
                txtTelefone.Text = dr[1].ToString().ToUpper() + dr[2].ToString().ToUpper();
                txtCpfCnpj.Text = dr[3].ToString().ToUpper();
                if (dr[4].ToString().ToUpper() == "V")
                {
                    cbTipoCliente.SelectedIndex = 1;
                }
                if (dr[4].ToString().ToUpper() == "A")
                {
                    cbTipoCliente.SelectedIndex = 0;
                }

                if (dr[5].ToString().ToUpper() == "B")
                {
                    cbBloquear.SelectedIndex = 0;
                }
                if (dr[5].ToString().ToUpper() == "D")
                {
                    cbBloquear.SelectedIndex = 1;
                }
                txtDicas.Text = "";

                clienteEncontrado = "1";

            }
            catch
            {
                txtDicas.Text = "Cliente não cadastrado.";
                txtNomeCliente.Text = "";
                txtTelefone.Text = "";
                txtCpfCnpj.Text = "";
            }

        }

        private void maskedTextBox1_Enter(object sender, EventArgs e)
        {
            txtTelefone.SelectAll();

        }

        private void txtTelefone_Click(object sender, EventArgs e)
        {
            txtTelefone.SelectAll();
        }

        private void cbTipoCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCpfCnpj_KeyUp(object sender, KeyEventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[1];

            parametros[0] = new MySqlParameter("?id", txtCpfCnpj.Text);

            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT cfnome,cfdtel,cftel,cfCadastroPJ,cftipocli,id,cfstatus from ubclifor where cfCadastroPJ = ?id;", parametros);
                txtNomeCliente.Text = dr[0].ToString().ToUpper();
                txtTelefone.Text = dr[1].ToString().ToUpper() + dr[2].ToString().ToUpper();
                txtCodigoCliente.Text = dr[5].ToString().ToUpper();
                lblStatusBusca.Text = "";
                if (dr[4].ToString().ToUpper() == "V")
                {
                    cbTipoCliente.SelectedIndex = 1;
                }
                if (dr[4].ToString().ToUpper() == "A")
                {
                    cbTipoCliente.SelectedIndex = 0;
                }

                if (dr[6].ToString().ToUpper() == "B")
                {
                    cbBloquear.SelectedIndex = 0;
                }
                if (dr[6].ToString().ToUpper() == "D")
                {
                    cbBloquear.SelectedIndex = 1;
                }
                txtDicas.Text = "";
                clienteEncontrado = "0";

            }
            catch
            {
                txtDicas.Text = "Cliente não cadastrado.";
                txtNomeCliente.Text = "";
                txtTelefone.Text = "";
                txtCodigoCliente.Text = "";
            }

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[2];

            parametros[0] = new MySqlParameter("?id", txtCodProduto.Text);
            parametros[1] = new MySqlParameter("?loja", login.idLoja);


            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT etqvalor,pdDescProd,pdOriginal,pdTributo,etqde,prodquant from ubestoque,ubprod,ubestoq_lj where ubestoq_lj.cod_lj = ?loja and  ubestoq_lj.cod_prod = ubprod.id  and etqcod_prod = ubprod.id and ubprod.id = ?id", parametros);

               
                decimal valor = decimal.Parse(dr[0].ToString());
                valorBase = valor;
                valor = valor * (decimal.Parse(lista) / 100);

                decimal subTotal = valor * decimal.Parse(numQtd.Value.ToString());
               
                txtValor.Text = string.Format("{0:n2}", valor);
                txtSubTotal.Text = string.Format("{0:n2}", subTotal);
                txtDescProduto.Text = dr["pdDescProd"].ToString();
                txtCodBarras.Text = dr["pdOriginal"].ToString();
                txtAliquota.Text = dr["pdTributo"].ToString();
                quantidadeEmEstoque = dr["prodquant"].ToString();
                txtDicas.Text = "Quantidade em estoque: " + quantidadeEmEstoque;


          }
            catch
            {
                txtDicas.Text = "Produto não encontrado!";
                txtValor.Text = "";
                txtSubTotal.Text = "";
                txtDescProduto.Text = "";
                txtCodBarras.Text = "";
                txtAliquota.Text = "";
            }

        }

        private void numQtd_ValueChanged(object sender, EventArgs e)
        {
            atualizaPreco(e);
        }
        public void atualizaPreco(EventArgs e) 
        {
            decimal subTotal = decimal.Parse(numQtd.Value.ToString()) * decimal.Parse(txtValor.Text);
            txtSubTotal.Text = string.Format("{0:n2}", subTotal);
        }

        private void numQtd_KeyUp(object sender, KeyEventArgs e)
        {
            atualizaPreco(e);

        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ao apertar BackSpace limpar textbox
            if (e.KeyChar == (Char)Keys.Back)
            {
                txtValorPago.Text = "";
            }
            //Caso seja digitado um ponto coloca-se uma vírgula
            if (e.KeyChar == ((char)(46)))
            {
                e.KeyChar = (char)(44);
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)Keys.Back && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            //pega a posição da virgula, caso ela exista:
            int posSeparator = txtValorPago.Text.IndexOf(',');

            //se a tecla digitada for virgula e ela já existir, barra:
            if (e.KeyChar == ',' && posSeparator > -1)
            {
                e.Handled = true;
                return;
            }

            //verifica quantos digitos há após a vírgula
            if (txtValorPago.Text.Contains(","))
            {
                string selecao = txtValorPago.Text.Substring(txtValorPago.SelectionStart);
                if (selecao.Contains(","))
                {
                }
                else
                {
                    int tamanho = txtValorPago.Text.Length;
                    posSeparator = ++posSeparator;
                    string depoisVirgula = txtValorPago.Text.Substring(posSeparator);

                    if (depoisVirgula.Length > 1)
                    {

                        e.Handled = true;

                        return;
                    }
                }
            }
        }

        private void txtCodBarras_KeyUp(object sender, KeyEventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[2];

            parametros[0] = new MySqlParameter("?id", txtCodBarras.Text);
            parametros[1] = new MySqlParameter("?loja", login.idLoja);


            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT etqvalor,pdDescProd,pdOriginal,pdTributo,etqde,prodquant,ubprod.id from ubestoque,ubprod,ubestoq_lj where ubestoq_lj.cod_lj = ?loja and  ubestoq_lj.cod_prod = ubprod.id  and etqcod_prod = ubprod.id and ubprod.pdOriginal = ?id", parametros);

               
                decimal valor = decimal.Parse(dr["etqvalor"].ToString());
                decimal valorBase = valor;
                valor = valor * (decimal.Parse(lista) / 100);
                decimal subTotal = valor * decimal.Parse(numQtd.Value.ToString());

                txtValor.Text = string.Format("{0:n2}", valor);
                txtSubTotal.Text = string.Format("{0:n2}", subTotal);
                txtDescProduto.Text = dr["pdDescProd"].ToString();
                txtCodProduto.Text = dr["id"].ToString();
                txtAliquota.Text = dr["pdTributo"].ToString();
                quantidadeEmEstoque = dr["prodquant"].ToString();
                txtDicas.Text = "Quantidade em estoque: " + quantidadeEmEstoque;


            }
            catch
            {
                txtDicas.Text = "Produto não encontrado!";
                txtValor.Text = "";
                txtSubTotal.Text = "";
                txtDescProduto.Text = "";
                txtCodProduto.Text = "";
                txtAliquota.Text = "";
            }
        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {
            if (clienteEncontrado == "1")
            {
                abreCupom();
            }
            else
            {
                if (clienteEncontrado == "0")
                {
                    insereCliente();   
                }
            }
        }
        public void abreCupom() 
        {
            
            status = ECFSWEDA.ECF_AbreCupom(txtCpfCnpj.Text);
            // Verifica se há algum cupom Aberto 

            if (status == 1) {
            ECFSWEDA.ECF_RetornaCOO(coo);
            txtTotal.Text = "0,00";
            txtTroco.Text = "0,00";
            }

            if (status != 1)
            {
                StringBuilder statusCupom = new StringBuilder(2);
                status = ECFSWEDA.ECF_StatusCupomFiscal(statusCupom);
                if (statusCupom.ToString() != "0")
                {
                    MessageBox.Show("Existe cupom aberto atualmente!");
                    return;
                }
            }
            

            //Gera numero da venda
            int result= Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select count(*) from ubvenda").ItemArray[0]);
            codVenda = ++result;
            //Grava dados da Venda
            atualizarItensCancelar(codVenda);
            int meioPag = cbFormaPagamento.SelectedIndex;

            ECFSWEDA.ECF_NumeroCaixa(caixa);           
            
            MySqlParameter[] par = new MySqlParameter[9];
            par[0] = new MySqlParameter("?vendaCod",codVenda);
            par[1] = new MySqlParameter("?vendaData",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            par[2] = new MySqlParameter("?vendaLoja",Convert.ToInt32(login.idLoja));
            par[3] = new MySqlParameter("?vendaOperador",Convert.ToInt32(login.idUsuario));
            par[4] = new MySqlParameter("?vendaLista",lista.Replace(",","."));
            par[5] = new MySqlParameter("?vendaCategoria", ++meioPag);
            par[6] = new MySqlParameter("?vendaCodCli", txtCodigoCliente.Text);
            par[7] = new MySqlParameter("?vendaCOO", coo.ToString());
            par[8] = new MySqlParameter("?caixa", caixa.ToString());
            

            stCodVenda.Text = "CÓDIGO DA VENDA: " + codVenda.ToString();
            
            lblCoo.Text = "COO: " + coo.ToString();

            objBanco.ExecuteNonQuery(conexao, CommandType.Text, "INSERT INTO ubvenda (vendaCodigo,vendaCoo,vendaCupom,vendaLoja,vendaCodCli,vendaData,vendaOperador,vendaCategoria,vendaLista,caixa) values(?vendaCod,?vendaCOO,'C',?vendaLoja,?vendaCodCli,?vendaData,?vendaOperador,?vendaCategoria,?vendaLista,?caixa)", par);
            

        }

        public void insereCliente() 
        {
            if ((txtNomeCliente.Text != "") && (txtTelefone.Text != ""))
            {
                string bloqueia;
                if (cbTipoCliente.SelectedIndex == 1)
                {
                    bloqueia = "D";
                }
                else
                {
                    bloqueia = "B";

                }

                string tipoCliente;
                if (cbTipoCliente.SelectedIndex == 1)
                {
                    tipoCliente = "V";
                }
                else
                {
                    tipoCliente = "A";

                }
                string ddd = txtTelefone.Text.Substring(1, 2);
                string tel = txtTelefone.Text.Substring(5);
                MySqlParameter[] par = new MySqlParameter[7];
                par[0] = new MySqlParameter("?cfNome", txtNomeCliente.Text);
                par[1] = new MySqlParameter("?cfCadastroPJ", txtCpfCnpj.Text);
                par[2] = new MySqlParameter("?cfDTel", ddd);
                par[3] = new MySqlParameter("?cfTel", tel);
                par[4] = new MySqlParameter("?cftipoCli", tipoCliente);
                par[5] = new MySqlParameter("?cfLoja", login.idLoja);
                par[6] = new MySqlParameter("?cfStatus", bloqueia);
                objBanco.ExecuteNonQuery(conexao, CommandType.Text, "INSERT INTO ubclifor (cfNome,cfCadastroPJ,cfDTel,cfTel,cftipoCli,cfLoja,cfStatus) VALUES(?cfNome,?cfCadastroPJ,?cfDTel,?cfTel,?cftipoCli,?cfLoja,?cfStatus);", par);

                try
                {
                    MySqlParameter[] parametros = new MySqlParameter[1];
                    parametros[0] = new MySqlParameter("?id", txtCpfCnpj.Text);
                    DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT cfnome,cfdtel,cftel,cfCadastroPJ,cftipocli,id,cfstatus from ubclifor where cfCadastroPJ = ?id;", parametros);
                    txtNomeCliente.Text = dr[0].ToString().ToUpper();
                    txtTelefone.Text = dr[1].ToString().ToUpper() + dr[2].ToString().ToUpper();
                    txtCodigoCliente.Text = dr[5].ToString().ToUpper();
                    lblStatusBusca.Text = "";
                    if (dr[4].ToString().ToUpper() == "V")
                    {
                        cbTipoCliente.SelectedIndex = 1;
                    }
                    if (dr[4].ToString().ToUpper() == "A")
                    {
                        cbTipoCliente.SelectedIndex = 0;
                    }

                    if (dr[6].ToString().ToUpper() == "B")
                    {
                        cbBloquear.SelectedIndex = 0;
                    }
                    if (dr[6].ToString().ToUpper() == "D")
                    {
                        cbBloquear.SelectedIndex = 1;
                    }
                    lblStatusBusca.Text = "";
                    txtDicas.Text = "Cliente cadastrado. Código: " + dr[5].ToString().ToUpper();
                    abreCupom();
                }
                catch
                {
                    MessageBox.Show("Houve um erro tente novamente!");
                }
            }
            else {
                MessageBox.Show("Preencha os dados do cliente corretamente!");
            }
        }

        private void txtValor_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                decimal subTotalVenda = Convert.ToDecimal(txtValor.Text) * Convert.ToDecimal(numQtd.Value);
                txtSubTotal.Text = subTotalVenda.ToString();
            }
            catch { }

        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ao apertar BackSpace limpar textbox
            if (e.KeyChar == (Char)Keys.Back)
            {
                txtValor.Text = "";
            }
            //Caso seja digitado um ponto coloca-se uma vírgula
            if (e.KeyChar == ((char)(46)))
            {
                e.KeyChar = (char)(44);
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)Keys.Back && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            //pega a posição da virgula, caso ela exista:
            int posSeparator = txtValor.Text.IndexOf(',');

            //se a tecla digitada for virgula e ela já existir, barra:
            if (e.KeyChar == ',' && posSeparator > -1)
            {
                e.Handled = true;
                return;
            }

            //verifica quantos digitos há após a vírgula
            if (txtValor.Text.Contains(","))
            {
                string selecao = txtValor.Text.Substring(txtValor.SelectionStart);
                if (selecao.Contains(","))
                {
                }
                else
                {
                    int tamanho = txtValor.Text.Length;
                    posSeparator = ++posSeparator;
                    string depoisVirgula = txtValor.Text.Substring(posSeparator);

                    if (depoisVirgula.Length > 1)
                    {

                        e.Handled = true;

                        return;
                    }
                }
            }
        }

        public void atualizarItensCancelar(int cod)
        {
            codVenda = cod;

            MySqlParameter[] parametros = new MySqlParameter[1];
            parametros[0] = new MySqlParameter("?codVenda", cod);

            string comando = "select itemNumero NUMERO, itemCodigo CODIGO, itemQuantidade QTD, itemValorVenda VALOR from ubitem where codVenda = ?codVenda and cancelado <> 1";
            DataTable dt = objBanco.RetornaDataTable(conexao, CommandType.Text, comando, parametros);
            dgvItens.DataSource = dt;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            try
            {
                inserirItem();
            }
            catch { }
        }

        private void inserirItem() {



               int result = Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select count(*) from ubvenda").ItemArray[0]);

               MySqlParameter[] par0 = new MySqlParameter[2];
               par0[0] = new MySqlParameter("?itemCodigo", txtCodProduto.Text);
               par0[1] = new MySqlParameter("?codVenda", result);
               
            string qtdItem = objBanco.RetornaDataRow(conexao, CommandType.Text, "select  count(distinct itemCodigo) * sum(itemQuantidade) itens from ubitem where itemCodigo = ?itemCodigo and codVenda= ?codVenda and cancelado = 0", par0)[0].ToString();

            if (qtdItem == ""){
                qtdItem = "0";            
            }

            int semEstoque = 0;

            if (Int32.Parse(quantidadeEmEstoque) < (Int32.Parse(qtdItem) + numQtd.Value))
            {
                semEstoque = 1;

                txtDicas.Text = "Estoque Insuficiente!";
                return;
            }

            status = ECFSWEDA.ECF_VendeItem(txtCodProduto.Text, txtDescProduto.Text, txtAliquota.Text, "I", numQtd.Value.ToString(), 2, txtValor.Text, "$", "");


            if (status == 1 && semEstoque != 1)
           {
               StringBuilder numeroItem = new StringBuilder(4);
               status = ECFSWEDA.ECF_UltimoItemVendido(numeroItem);


               MySqlParameter[] par = new MySqlParameter[7];
               par[0] = new MySqlParameter("?itemNumero", numeroItem.ToString());
               par[1] = new MySqlParameter("?itemCodigo",txtCodProduto.Text );
               par[2] = new MySqlParameter("?itemCodBarras", txtCodBarras.Text);
               par[3] = new MySqlParameter("?CodVenda", result);
               par[4] = new MySqlParameter("?itemQuantidade",numQtd.Value);
               par[5] = new MySqlParameter("?itemValorBase",valorBase);
               par[6] = new MySqlParameter("?itemValorVenda",txtValor.Text.Replace(",","."));
               objBanco.ExecuteNonQuery(conexao, CommandType.Text,"INSERT INTO ubitem SET itemNumero=?itemNumero,itemCodigo=?itemCodigo,itemCodBarras=?itemCodBarras,CodVenda=?CodVenda,itemQuantidade=?itemQuantidade,itemValorBase=?itemValorBase,itemValorVenda=?itemValorVenda;", par);

               atualizarItensCancelar(codVenda);
            
            
            }  

           if (status != 1 ) 
           {
               ECFSWEDA.ECF_RetornoImpressoraMFD(ref ack, ref st1, ref st2, ref  st3);
               MessageBox.Show("ERRO: " + st3.ToString());

          }
           else
           {

               if (semEstoque != 1)
               {

                   qtdAtual = Int32.Parse(quantidadeEmEstoque) - (Int32.Parse(qtdItem) + Int32.Parse(numQtd.Value.ToString()));
                   txtDicas.Text = "Estoque Atual: " + qtdAtual.ToString();

               StringBuilder item = new StringBuilder(4);
               ECFSWEDA.ECF_UltimoItemVendido(item);
               int count = txtDescProduto.Text.Length;
               string espacos = new string(' ', 29 - count);
               string descricao = txtDescProduto.Text + espacos;
               string cod = txtCodProduto.Text;
               count = cod.Length;
               espacos = new string(' ', 10 - count);
               cod = cod + espacos;

               string qtd = numQtd.Value.ToString();
               count = qtd.Length;
               espacos = new string(' ', 8 - count);
               qtd = qtd + espacos;

               string valor = txtValor.Text.ToString();
               count = valor.Length;
               espacos = new string(' ', 18 - count);
               valor = valor + espacos;

               string subtotal = txtSubTotal.Text.ToString();

               subTotal();
              }
           }
        }

        private void subTotal() 
        {
            StringBuilder subTotalB = new StringBuilder(14);
            ECFSWEDA.ECF_SubTotal(subTotalB);

            string subTotal = subTotalB.ToString();
            string subTotalP1 = subTotal.Substring(0,12);
            string subTotalP2 = subTotal.Substring(12,2);
            subTotal = subTotalP1 + "," + subTotalP2;
            decimal subTotalDec = Convert.ToDecimal(subTotal);
            txtTotal.Text =subTotalDec.ToString();

        }

        private void txtSubTotalCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ao apertar BackSpace limpar textbox
            if (e.KeyChar == (Char)Keys.Back)
            {
                txtSubTotalCompra.Text = "";
            }
            //Caso seja digitado um ponto coloca-se uma vírgula
            if (e.KeyChar == ((char)(46)))
            {
                e.KeyChar = (char)(44);
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)Keys.Back && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            //pega a posição da virgula, caso ela exista:
            int posSeparator = txtSubTotalCompra.Text.IndexOf(',');

            //se a tecla digitada for virgula e ela já existir, barra:
            if (e.KeyChar == ',' && posSeparator > -1)
            {
                e.Handled = true;
                return;
            }

            //verifica quantos digitos há após a vírgula
            if (txtSubTotalCompra.Text.Contains(","))
            {
                string selecao = txtSubTotalCompra.Text.Substring(txtSubTotalCompra.SelectionStart);
                if (selecao.Contains(","))
                {
                }
                else
                {
                    int tamanho = txtValor.Text.Length;
                    posSeparator = ++posSeparator;
                    string depoisVirgula = txtSubTotalCompra.Text.Substring(posSeparator);

                    if (depoisVirgula.Length > 1)
                    {

                        e.Handled = true;

                        return;
                    }
                }
            }
        }

        private void txtValorCompra_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                decimal subtotalCompraSucata = Convert.ToDecimal(txtValorCompra.Text) * Convert.ToDecimal(numQtdCompra.Value);
                txtSubTotalCompra.Text = subtotalCompraSucata.ToString();
            }
            catch {
            }
        }

        private void numQtdCompra_ValueChanged(object sender, EventArgs e)
        {
            decimal subtotalCompraSucata = Convert.ToDecimal(txtValorCompra.Text) * Convert.ToDecimal(numQtdCompra.Value);
            txtSubTotalCompra.Text = subtotalCompraSucata.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            StringBuilder statusCupom = new StringBuilder(2);
            ECFSWEDA.ECF_StatusCupomFiscal(statusCupom);
            ECFSWEDA.ECF_CancelaCupom();

            MySqlParameter[] parCanCup = new MySqlParameter[1];
            parCanCup[0] = new MySqlParameter("?codVenda",codVenda);
            
            comando = "select vendaFinalizada from ubvenda WHERE vendaCodigo = ?codVenda";
            DataRow drCanCup =  objBanco.RetornaDataRow(conexao, CommandType.Text, comando, parCanCup);
            if (drCanCup["vendaFinalizada"].ToString() == "0") {
                MessageBox.Show("0");
            }
            if (drCanCup["vendaFinalizada"].ToString() == "1")
            {
                MessageBox.Show("1");
            }
            if (drCanCup["vendaFinalizada"].ToString() == "2")
            {
                MessageBox.Show("2");
            }


           
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
           fechaCupom();
        }

        public string tipo = "$";
        public string valor = "00000000000000";

        private void fechaCupom(){
            string meioPagamento = cbFormaPagamento.SelectedValue.ToString();
            //ECFSWEDA.ECF_FechaCupom(meioPagamento, "A", "$", "0000", txtValorPago.Text, "A UNIBATER AGRADECE A PREFERÊNCIA - VOLTE SEMPRE");
            ECFSWEDA.ECF_IniciaFechamentoCupom("A", tipo, valor);
            ECFSWEDA.ECF_EfetuaFormaPagamento(cbFormaPagamento.SelectedValue.ToString(), txtValorPago.Text);
            status = ECFSWEDA.ECF_TerminaFechamentoCupom("Volte Sempre!");

            MessageBox.Show(status.ToString());

            StringBuilder valorPago = new StringBuilder(14);
            ECFSWEDA.ECF_ValorPagoUltimoCupom(valorPago);
            string valorPagoA = valorPago.ToString();
            string valorPagoP = valorPagoA.Substring(0, 12);
            string valorPagoS = valorPagoA.Substring(12, 2);
            valorPagoA = valorPagoP + "," + valorPagoS;
            decimal valorPagoD = Convert.ToDecimal(valorPagoA);
            txtValorPago.ResetText();
           
            StringBuilder subTotal = new StringBuilder(14);
            ECFSWEDA.ECF_SubTotal(subTotal);

            string subTotalA = subTotal.ToString();
 
            subTotalA = subTotalA.Insert(12,",");
            decimal subTotalD = Convert.ToDecimal(subTotalA);

            if (valorPagoD < subTotalD)
            {
                decimal valorFaltante = subTotalD - valorPagoD;
                txtDicas.Text = "Faltam: R$" + valorFaltante.ToString();
            }
            else 
            {
                if (valorPagoD > subTotalD)
                { 
                decimal troco = valorPagoD - subTotalD;
                txtTroco.Text = troco.ToString();
                }
                txtDicas.Text = "Obrigado! Volte Sempre!";
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter("?vendaCodigo", codVenda);
                objBanco.ExecuteNonQuery(conexao, CommandType.Text, "UPDATE ubvenda set vendaFinalizada = 1 where vendaCodigo=?vendaCodigo", param);
                DataTable dt = objBanco.RetornaDataTable(conexao, CommandType.Text, "SELECT itemCodigo,sum(itemQuantidade) from ubitem where codvenda = ?vendaCodigo and cancelado <> 1 GROUP by itemCodigo", param);
                int numeroLinhas = dt.Rows.Count;
                DataRow dr;

                MySqlParameter[] parametros = new MySqlParameter[3];

                for (int i = 0; i < numeroLinhas; i++)
                {
                    dr = dt.Rows[i];
                    parametros[0] = new MySqlParameter("?cod_prod", dr[0].ToString());
                    parametros[1] = new MySqlParameter("?qtd", dr[1].ToString());
                    parametros[2] = new MySqlParameter("?cod_lj", login.idLoja);
                    objBanco.ExecuteNonQuery(conexao, CommandType.Text, "update ubestoq_lj set prodquant = prodquant - ?qtd where cod_lj = ?cod_lj and cod_prod = ?cod_prod", parametros);
                }

            }

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            frmCancItem frmCI = new frmCancItem();
            frmCI.atualizarItensCancelar(codVenda);
            frmCI.ShowDialog();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            status = ECFSWEDA.ECF_CancelaItemAnterior();
            if (status == 1)
            {
                try
                {
                    DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "select  id from ubitem order by id desc limit 1");
                    MySqlParameter[] par = new MySqlParameter[1];
                    par[0] = new MySqlParameter("?id", dr["id"]);
                    objBanco.ExecuteNonQuery(conexao, CommandType.Text, "UPDATE ubitem set cancelado = 1 where id = ?id", par);
                    atualizarItensCancelar(codVenda);
                
                }
                catch
                {
                }
            }
        }

        private void dgvBateriasCompra_Click(object sender, EventArgs e)
        {
            try
            {
                sucataSelecionada = dgvBateriasCompra.SelectedRows[0].Cells[0].Value.ToString();
            }
            catch 
            {
            }
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[2];
            parametros[0] = new MySqlParameter("?codVenda", codVenda);
            parametros[1] = new MySqlParameter("?Item", itemSel);

            string comando = "UPDATE ubitem SET cancelado = 1 where codVenda = ?codVenda and itemNumero = ?Item";
            objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametros);

            atualizarItensCancelar(codVenda);

            ECFSWEDA.ECF_CancelaItemGenerico(itemSel);
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            compraSucata = false;
            dgvBateriasCompra.DataSource = null;
        }

        private void dgvItens_Click(object sender, EventArgs e)
        {
            try
            {
                itemSel = dgvItens.SelectedRows[0].Cells[0].Value.ToString();
            }
            catch
            {
            }
        }

        private void pictureBox12_Click_1(object sender, EventArgs e)
        {
            try
            {
                string idItemSucata = dgvBateriasCompra.SelectedRows[0].Cells[0].Value.ToString();
                string idProdSucata = dgvBateriasCompra.SelectedRows[0].Cells[3].Value.ToString();

                excluirItem(idItemSucata, idProdSucata);
            }
            catch { }
        }

        private void excluirItem(string itemSucata, string codProdSucata)
        {
            MySqlParameter[] parametros = new MySqlParameter[1];
            parametros[0] = new MySqlParameter("?id", itemSucata);

            string comando = "delete from ubitemsucata where id  = ?id";
            objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametros);

            MySqlParameter[] parametrosEst = new MySqlParameter[2];
            parametrosEst[0] = new MySqlParameter("?produto",codProdSucata);
            parametrosEst[1] = new MySqlParameter("?loja",login.idLoja);

            comando = "update ubestoq_sucata set esquantsu = esquantsu - 1  where esprodsu = ?produto and esljsu = ?loja";
            objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametrosEst);

            atualizarSucata();
        }
    }

}