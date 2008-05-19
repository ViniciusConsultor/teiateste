using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;
using System.Collections;


namespace uniBaterFrenteLoja
{
    public partial class vendaOrcamento : Form
    {

        Form1 frm1 =new Form1();
        venda objVenda = new venda(Form1.conexao);
        string codCliente;
        public int caixa;
        public int numVenda = 0;
        int itemEncontrado = 0;
        DataRow drItem;


        public vendaOrcamento()
        {
            InitializeComponent();
        }

        

        private void txtCodigoCliente_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;

            int tipo = 0;
            
            if (txtbox.Name == "txtCodigoCliente")
            { 
                tipo = 1;
            }

            if (txtbox.Name == "txtCpfCnpj")
            { 
                tipo = 2;
            }

            DataRow dr = objVenda.consultarCliente(txtbox.Text.ToString(), tipo);
            if (dr != null)
            {
                
                if (txtbox.Name == "txtCodigoCliente")
                {
                    txtCpfCnpj.Text = dr["cfCadastroPJ"].ToString();
                    txtTelefone.Text = dr["cfDtel"].ToString() + dr["cfTel"].ToString();
                    txtNomeCliente.Text = dr["cfNome"].ToString();
                }
                if (txtbox.Name == "txtCpfCnpj")
                {
                    txtCodigoCliente.Text = dr["id"].ToString();
                    txtTelefone.Text = dr["cfDtel"].ToString() + dr["cfTel"].ToString();
                    txtNomeCliente.Text = dr["cfNome"].ToString();
                }

                if(dr["cfStatus"].ToString() == "D") 
                {    
                    cbBloquear.SelectedIndex = 0;
                }
                if (dr["cfStatus"].ToString() == "B")
                {                  
                    cbBloquear.SelectedIndex = 1;

                }
                if (dr["cftipoCli"].ToString() == "V")
                {
                    cbTipoCliente.SelectedIndex = 0;
                }
                if (dr["cftipoCli"].ToString() == "A")
                {
                    cbTipoCliente.SelectedIndex = 1;

                }
                codCliente = dr["id"].ToString();
            }
            else
            {
                if (txtbox.Name == "txtCodigoCliente")
                {
                    txtCpfCnpj.ResetText();
                    txtTelefone.ResetText();
                    txtNomeCliente.ResetText();
                }
                if (txtbox.Name == "txtCpfCnpj")
                {
                    txtCodigoCliente.ResetText();
                    txtTelefone.ResetText();
                    txtNomeCliente.ResetText();
                }
                codCliente = null;
            }
        }

        private void a(object sender, KeyEventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

            if (numVenda == 0)
            {
                txtDicas.Text = "NÃO HÁ VENDA ABERTA!";
            }
            else
            {
               int formaPagamento = cbFormaPagamento.SelectedIndex;
               formaPagamento = ++formaPagamento;
               try
                  {
                   string retorno = objVenda.efetuaPagamento(numVenda, Convert.ToDecimal(txtTotal.Text), Convert.ToDecimal(txtValorPago.Text), formaPagamento);
                   dgvPagamentos.DataSource = objVenda.retornaPagamentosEfetuados(numVenda);
                   objVenda.trataRetornoPagamento(retorno, this, numVenda);
                   txtValorPago.ResetText();
               }
               catch { }
                
             }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (codCliente != null)
            {
                if (numVenda == 0)
                {
                    if (cbBloquear.SelectedIndex != 1 && codCliente != "")
                    {
                        //Abre Venda/Orçamento
                        decimal listaComissao = objVenda.retornaLista(login.idLoja);
                        numVenda = objVenda.abreVenda('O', listaComissao, login.idLoja, Convert.ToInt32(codCliente), login.idUsuario, txtVeiculo.Text, txtPlaca.Text, caixa);
                        stCodVenda.Text = "CÓDIGO DA VENDA: " + numVenda.ToString();
                        txtDicas.Text = "CÓDIGO DA VENDA: " + numVenda.ToString();
                        dgvItens.DataSource = objVenda.retornaItens(numVenda);
                    }
                    else
                    {
                        txtDicas.Text = "CLIENTE BLOQUEADO!";
                    }
                }
            }
            else
            {
                if (txtNomeCliente.Text != "" || txtTelefone.Text != "")
                {
                    //Cadastrar Cliente
                    int retorno = objVenda.cadastraCliente(txtNomeCliente.Text, txtTelefone.Text, txtCpfCnpj.Text, cbTipoCliente.SelectedIndex, login.idLoja);
                    txtCodigoCliente.Text = retorno.ToString();
                    txtDicas.Text = "CLIENTE CADASTRADO, CÓDIGO: " + retorno;
                    codCliente = retorno.ToString();

                    decimal listaComissao = objVenda.retornaLista(login.idLoja);
                    numVenda = objVenda.abreVenda('O', listaComissao, login.idLoja, Convert.ToInt32(codCliente), login.idUsuario, txtVeiculo.Text, txtPlaca.Text, caixa);
                    stCodVenda.Text = "CÓDIGO DA VENDA: " + numVenda.ToString();
                    txtDicas.Text = "CÓDIGO DA VENDA: " + numVenda.ToString();
                }
            }
        }

        private void vendaOrcamento_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bsHorario.Text = DateTime.Now.ToString("'DATA': dd/MM/yy  'HORA': HH:mm:ss");
        }

        private void vendaOrcamento_Load(object sender, EventArgs e)
        {
            cbFormaPagamento.DataSource = objVenda.retornaFomasPagamentos();
            cbFormaPagamento.DisplayMember = objVenda.retornaFomasPagamentos().Columns[0].ToString();

            timer1.Tick +=new EventHandler(timer1_Tick);
            stLoja.Text = "LOJA: " + objVenda.retornaNomeLoja(login.idLoja).ToUpper();
            stOperador.Text = "OPERADOR: " + objVenda.retornaNomeFuncionario(login.idUsuario).ToUpper();
            caixa = objVenda.retornaNumeroCaixa();
            numVenda = objVenda.verificaVendaAberta('O', login.idLoja, login.idUsuario, caixa);
            
            if (numVenda != 0) 
            {
                DataRow dr = objVenda.retornaDadosCliente(numVenda);
                txtCodigoCliente.Text = dr["id"].ToString();
                codCliente = dr["id"].ToString();
                txtNomeCliente.Text = dr["cfNome"].ToString();
                txtCpfCnpj.Text = dr["cfCadastroPj"].ToString();
                txtTelefone.Text = dr["cfDTel"].ToString() + dr["cfTel"].ToString();
                txtVeiculo.Text = dr["vendaVeiculo"].ToString();
                txtPlaca.Text = dr["vendaPlaca"].ToString();
                
                if (dr["cfStatus"].ToString() == "D")
                {
                    cbBloquear.SelectedIndex = 0;
                }
                if (dr["cfStatus"].ToString() == "B")
                {
                    cbBloquear.SelectedIndex = 1;
                }
                if (dr["cftipoCli"].ToString() == "V")
                {
                    cbTipoCliente.SelectedIndex = 0;
                }
                if (dr["cftipoCli"].ToString() == "A")
                {
                    cbTipoCliente.SelectedIndex = 1;

                }
                stCodVenda.Text = "CÓDIGO DA VENDA: " + numVenda.ToString().ToUpper();
                dgvItens.DataSource = objVenda.retornaItens(numVenda);
                txtTotal.Text = objVenda.retornaSubTotal(numVenda).ToString();
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            int[] result = objVenda.cancelaVenda(numVenda);
            vendaOrcamento frmVenda = this;
            objVenda.resetVenda(frmVenda);
            codCliente = "";
        }

        private void txtCodProduto_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
               DataRow dr = objVenda.consultarItem(txtCodProduto.Text,1);
               objVenda.preencheCamposItem(dr, this,1);
               itemEncontrado = 1;
               drItem = dr;
            }
            catch
            {
                objVenda.resetCamposItem(this, 1);
                itemEncontrado = 0;
            }
        }

        private void txtCodBarras_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                DataRow dr = objVenda.consultarItem(txtCodBarras.Text,2);
                objVenda.preencheCamposItem(dr, this, 2);
                itemEncontrado = 1;
                drItem = dr;

            }
            catch
            {
                objVenda.resetCamposItem(this,2);
                itemEncontrado = 0;
            }
        }

        private void numQtd_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                objVenda.atualizaSubTotal(this);
            }
            catch
            {

            }
        }

        private void numQtd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                objVenda.atualizaSubTotal(this);
            }
            catch
            {

            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (itemEncontrado == 0) 
            {
                txtDicas.Text = "NÃO HÁ VENDA ABERTA!";

            }

            if (itemEncontrado == 1 && numVenda != 0) 
            {

                try
                {
                    objVenda.inserirItem(Convert.ToInt32(drItem["cod_prod"]), drItem["pdOriginal"].ToString(), numVenda, numQtd.Value, Convert.ToDecimal(drItem["etqvalor"]), Convert.ToDecimal(txtValor.Text), txtNSBateria.Text);
                    dgvItens.DataSource = objVenda.retornaItens(numVenda);
                    txtTotal.Text = objVenda.retornaSubTotal(numVenda).ToString();
                }
                catch {
                    return;
                }
            }
        }

        private void txtValorPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Caso seja digitado um ponto coloca-se uma vírgula
            if (e.KeyChar == ((char)(46)))
            {
                e.KeyChar = (char)(44);
            }
        }

    

        }
    }
