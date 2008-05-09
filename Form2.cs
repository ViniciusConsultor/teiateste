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
    public partial class frmCadastroCheques : Form
    {
        public int codVenda;
        MySql objCon = new MySql();
        Form1 frm1 = new Form1();

        
        public frmCadastroCheques()
        {
            InitializeComponent();
        }

        public frmCadastroCheques(string idCliente)
        {
            InitializeComponent();
            txtIdCliente.Text = idCliente;


        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }
        public void atualizarItensCancelar(int cod) 
        {
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dgvItensCancelar_Click(object sender, EventArgs e)
        {
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

    }
}