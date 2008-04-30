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
    public partial class frmCancItem : Form
    {
        public int codVenda;
        string itemSel;
        MySql objCon = new MySql();
        Form1 frm1 = new Form1();


        
        public frmCancItem()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }
        public void atualizarItensCancelar(int cod) 
        {
            codVenda = cod;
            
            MySqlParameter[] parametros = new MySqlParameter[1];
            parametros[0] = new MySqlParameter("?codVenda", cod);

            string comando = "select itemNumero NUMERO, itemCodigo CÓDIGO, itemQuantidade QTD, itemValorVenda VALOR from ubitem where codVenda = ?codVenda and cancelado <> 1";
            DataTable dt = objCon.RetornaDataTable(frm1.conexao, CommandType.Text, comando, parametros);
            dgvItensCancelar.DataSource = dt;
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dgvItensCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                itemSel = dgvItensCancelar.SelectedRows[0].Cells[0].Value.ToString();
            }
            catch
            {
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MySqlParameter[] parametros = new MySqlParameter[2];
            parametros[0] = new MySqlParameter("?codVenda", codVenda);
            parametros[1] = new MySqlParameter("?Item", itemSel);

            string comando = "UPDATE ubitem SET cancelado = 1 where codVenda = ?codVenda and itemNumero = ?Item";
            objCon.ExecuteNonQuery(frm1.conexao, CommandType.Text, comando, parametros);

            atualizarItensCancelar(codVenda);

            ECFSWEDA.ECF_CancelaItemGenerico(itemSel);
        }

    }
}