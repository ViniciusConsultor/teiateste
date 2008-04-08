using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace uniBaterFrenteLoja
{
    public partial class login : Form
    {

        Form1 frm1 = new Form1();
        public static int idUsuario;
        public static int idLoja;


        public login()
        {
            InitializeComponent();
        }

        private void imgCupomFiscal_MouseHover(object sender, EventArgs e)
        {
            imgCupomFiscal.ImageLocation = "img/btnCupomFiscalHover.png";

        }

        private void imgCupomFiscal_MouseLeave(object sender, EventArgs e)
        {
            imgCupomFiscal.ImageLocation = "img/btnCupomFiscal.png";

        }

        private void login_Load(object sender, EventArgs e)
        {
            this.Width = 231;
            conexao objConexao = new conexao();

            objConexao.preencheCB(cbLojas, "Select * from ubloja",frm1.conexao);
            txtUsuario.Focus();
        }

     

        private void imgCupomFiscal_Click(object sender, EventArgs e)
        {
            this.Hide(); 
            Form1 vendaCupom = new Form1();
            vendaCupom.Show();

        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            efetuarLogin();
        }

        public void efetuarLogin() {

            MySqlConnection con = new MySqlConnection(frm1.conexao);
            con.Open();
            MySqlCommand comando = new MySqlCommand("Select id,fcNomeAcesso,fcSenha,fcLoja from ubfuncionario where fcNomeAcesso =?Nome and fcSenha =?Senha and fcLoja=?Loja");
            comando.Connection = con;

            MySqlParameter Parametros = new MySqlParameter();

            comando.Parameters.AddWithValue("?Senha", txtSenha.Text);
            comando.Parameters.AddWithValue("?Loja", cbLojas.SelectedValue);
            comando.Parameters.AddWithValue("?Nome", txtUsuario.Text);

            MySqlDataAdapter daLogin = new MySqlDataAdapter(comando);
            DataTable dtLogin = new DataTable();
            daLogin.Fill(dtLogin);
            try
            {
                idLoja = Int32.Parse(dtLogin.Rows[0][3].ToString());
                idUsuario = Int32.Parse(dtLogin.Rows[0][0].ToString());
                lblStatus.Text = "LOGIN EFETUADO!";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                for (int i = 231; i <= 483; i += 50)
                {
                    this.Width = i;
                }
            }
            catch
            {

                lblStatus.Text ="USUÁRIO INEXISTENTE!";
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

         
           
        }

        private void txtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) {
                efetuarLogin();
            
            }
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }
    }
}