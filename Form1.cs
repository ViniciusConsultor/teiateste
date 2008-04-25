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

            ar = new ArrayList();
            ar.Add(new combo("SIM", "T"));
            ar.Add(new combo("NÃO", "C"));

            cbTroca.DisplayMember = "Nome";
            cbTroca.ValueMember = "Valor";
            cbTroca.DataSource = ar;

            cbTroca.SelectedIndex = 0;


        
            conexao con = new conexao();

            MySqlParameter[] parametros = new MySqlParameter[2];

            parametros[0] = new MySqlParameter("?idl",login.idLoja);
            parametros[1] = new MySqlParameter("?idf", login.idUsuario);

            //DataRow dr = con.executarComando("Select ubloja.ljNome,ubfuncionario.fcNome from ubloja,ubfuncionario where ubloja.id = ?idl and ubfuncionario.id =?idf", parametros);

            MySql objBanco = new MySql();

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
            MySql objBanco = new MySql();
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
            MySql objBanco = new MySql();
            

            if (compraSucata == false)
            {
                result = Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select id from ubvencomsucata order by id desc").ItemArray[0]);
                codCompraBateria = ++result;

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

            result = Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select id from ubitemsucata order by id desc").ItemArray[0]);
            int codCompraItemSucata = ++result;
            int codProduto = cbTipoBateria.SelectedIndex; 
            //item sucata
            MySqlParameter[] parametrosItem = new MySqlParameter[6];
            parametrosItem[0] = new MySqlParameter("?id", codCompraItemSucata);
            parametrosItem[1] = new MySqlParameter("?itprod", ++codProduto);
            parametrosItem[2] = new MySqlParameter("?itquant", numQtdCompra.Value);
            parametrosItem[3] = new MySqlParameter("?itvalor", txtValorCompra.Text.Replace(",","."));
            parametrosItem[4] = new MySqlParameter("?itsubtotal", txtSubTotalCompra.Text.Replace(",", "."));
            parametrosItem[5] = new MySqlParameter("?itvencomsucata", codCompraBateria);

            comando = "INSERT INTO ubitemsucata SET id=?id, itprod=?itprod, itquant=?itquant, itvalor=?itvalor, itsubtotal=?itsubtotal, itvencomsucata=?itvencomsucata";
            objBanco.ExecuteNonQuery(conexao, CommandType.Text, comando, parametrosItem);

            MySqlParameter[] parametrosDGV= new MySqlParameter[1];
            parametrosDGV[0] = new MySqlParameter("?venda", codCompraBateria);

            comando = "SELECT itvencomsucata 'COD. VENDAS',suprod 'PRODUTO',itquant 'QUANTIDADE', itvalor 'VALOR',itsubtotal 'SUBTOTAL'  FROM ubitemsucata,ubsucataprod WHERE itvencomsucata = ?venda and ubitemsucata.itprod = ubsucataprod.id order by ubitemsucata.id desc  ";
            DataTable tabelaSucata = new DataTable();
            tabelaSucata = objBanco.RetornaDataTable(conexao, CommandType.Text, comando, parametrosDGV);
            dgvBateriasCompra.DataSource = tabelaSucata;
        
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

            MySql objBanco = new MySql();
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

            MySql objBanco = new MySql();
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
            MySqlParameter[] parametros = new MySqlParameter[1];

            parametros[0] = new MySqlParameter("?id", txtCodProduto.Text);

            MySql objBanco = new MySql();
            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT etqvalor,pdDescProd,pdOriginal,pdTributo,etqde from ubestoque,ubprod where etqcod_prod = ubprod.id and ubprod.id = ?id;", parametros);

               
                float valor = float.Parse(dr[0].ToString());
                valor = valor * (float.Parse(lista) / 100);

                float subTotal = valor * float.Parse(numQtd.Value.ToString());
               
                txtValor.Text = string.Format("{0:n2}", valor);
                txtSubTotal.Text = string.Format("{0:n2}", subTotal);
                txtDescProduto.Text = dr["pdDescProd"].ToString();
                txtCodBarras.Text = dr["pdOriginal"].ToString();
                txtAliquota.Text = dr["pdTributo"].ToString();
                txtDicas.Text = "Quantidade em estoque: " + dr["etqde"].ToString();


          }
            catch
            {
                lblStatusBusca.Text = "Não encontrado";
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
            float subTotal = float.Parse(numQtd.Value.ToString()) * float.Parse(txtValor.Text);
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
            MySqlParameter[] parametros = new MySqlParameter[1];

            parametros[0] = new MySqlParameter("?id", txtCodBarras.Text);

            MySql objBanco = new MySql();
            try
            {
                DataRow dr = objBanco.RetornaDataRow(conexao, CommandType.Text, "SELECT etqvalor,pdDescProd,pdOriginal,pdTributo,ubprod.id,etqde from ubestoque,ubprod where etqcod_prod = ubprod.id and pdOriginal = ?id;", parametros);

                float valor = float.Parse(dr["etqvalor"].ToString());
                valor = valor * (float.Parse(lista) / 100);
                float subTotal = valor * float.Parse(numQtd.Value.ToString());

                txtValor.Text = string.Format("{0:n2}", valor);
                txtSubTotal.Text = string.Format("{0:n2}", subTotal);
                txtDescProduto.Text = dr["pdDescProd"].ToString();
                txtCodProduto.Text = dr["id"].ToString();
                txtAliquota.Text = dr["pdTributo"].ToString();
                txtDicas.Text = "Quantidade em estoque: " + dr["etqde"].ToString();


            }
            catch
            {
                lblStatusBusca.Text = "Não encontrado";
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
            MySql objBanco = new MySql();            
            int result= Convert.ToInt32(objBanco.RetornaDataRow(conexao, CommandType.Text, "select count(*) from ubvenda").ItemArray[0]);
            int codVenda = ++result;

            //Grava dados da Venda
            MySqlParameter[] par = new MySqlParameter[8];
            par[0] = new MySqlParameter("?vendaCod",codVenda);
            par[1] = new MySqlParameter("?vendaData",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            par[2] = new MySqlParameter("?vendaLoja",Convert.ToInt32(login.idLoja));
            par[3] = new MySqlParameter("?vendaOperador",Convert.ToInt32(login.idUsuario));
            par[4] = new MySqlParameter("?vendaLista",lista.Replace(",","."));
            par[5] = new MySqlParameter("?vendaCategoria", cbFormaPagamento.SelectedValue);
            par[6] = new MySqlParameter("?vendaCodCli", txtCodigoCliente.Text);
            par[7] = new MySqlParameter("?vendaCOO", coo.ToString());

            lblCoo.Text = "COO: " + coo.ToString();

            objBanco.ExecuteNonQuery(conexao, CommandType.Text, "INSERT INTO ubvenda (vendaCodigo,vendaCoo,vendaCupom,vendaLoja,vendaCodCli,vendaData,vendaOperador,vendaCategoria,vendaLista) values(?vendaCod,?vendaCOO,'C',?vendaLoja,?vendaCodCli,?vendaData,?vendaOperador,?vendaCategoria,?vendaLista)", par);
            rtbCupom.Text = rtbCupom.Text + "============================================\n";
            rtbCupom.Text = rtbCupom.Text + "    DAVID MARCOS RODRIGUES BATERIAS EPP.\n";
            rtbCupom.Text = rtbCupom.Text + "        AV. DR. JOÃO GUIMARÃES, 735\n";
            rtbCupom.Text = rtbCupom.Text + "               SÃO PAULO - SP\n";
            rtbCupom.Text = rtbCupom.Text + "CNPJ:68.955.459/0005-06   IE:149.831.779.113\n";
            rtbCupom.Text = rtbCupom.Text + "============================================\n";
            rtbCupom.Text = rtbCupom.Text + "ITEM CÓDIGO    DESCRIÇÃO                    \n";
            rtbCupom.Text = rtbCupom.Text + "QTD.           VL UNIT(R$)       VL ITEM(R$)\n";
            rtbCupom.Focus();
            rtbCupom.Select(rtbCupom.Text.Length, 0);
            

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
                MySql objBanco = new MySql();
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

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            inserirItem();
        }

        private void inserirItem() {

           status =  ECFSWEDA.ECF_VendeItem(txtCodProduto.Text, txtDescProduto.Text, txtAliquota.Text, "I", numQtd.Value.ToString(), 2, txtValor.Text, "$", "");
          // status = ;
          // MessageBox.Show(st3.ToString());
           if (status != 1) 
           {
               ECFSWEDA.ECF_RetornoImpressoraMFD(ref ack, ref st1, ref st2, ref  st3);
               MessageBox.Show("ERRO: " + st3.ToString());
           }
           else
           {
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


              
             //44
             //rtbCupom.Text = rtbCupom.Text + "ITEM CÓDIGO    DESCRIÇÃO                    \n";
             //rtbCupom.Text = rtbCupom.Text + "QTD.           VL UNIT(R$)       VL ITEM(R$)\n";
               rtbCupom.Text = rtbCupom.Text + item + " " +cod+ descricao + "\n";
               rtbCupom.Text = rtbCupom.Text + qtd + " x     " + valor + subtotal + "\n";
               rtbCupom.Focus();
               rtbCupom.Select(rtbCupom.Text.Length, 0);

               subTotal();
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
            txtTotal.Text = string.Format("{0:c}",subTotalDec);

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
            if (statusCupom.ToString() == "1")
            {
                ECFSWEDA.ECF_CancelaCupom();
                rtbCupom.Text = rtbCupom.Text + "============================================\n";
                rtbCupom.Text = rtbCupom.Text + "              CUPOM CANCELADO               \n";
                rtbCupom.Text = rtbCupom.Text + "============================================\n";
                rtbCupom.Focus();
                rtbCupom.Select(rtbCupom.Text.Length, 0);

            }
            else
            {
                MessageBox.Show("Não há cupom a cancelar.");
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
            MessageBox.Show(valorPago.ToString());
            txtValorPago.ResetText();
        }
    }

}