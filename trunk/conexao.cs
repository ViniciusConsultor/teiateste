using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace uniBaterFrenteLoja
{
    public class conexao
    {
        
        public DataRow executarComando(String comando,MySqlParameter[] par,string con)
        {
            MySqlConnection Conexao = new MySqlConnection(con);

        Conexao.Open();
        MySqlCommand Comando = new MySqlCommand(comando, Conexao);

        foreach (MySqlParameter parametroRecebido in par) {
            Comando.Parameters.Add(parametroRecebido);
        }

        MySqlDataAdapter da = new MySqlDataAdapter(Comando);
        DataTable Tabela = new DataTable();
        da.Fill(Tabela);  
        Conexao.Close();
        return Tabela.Rows[0];
        }

        public void preencheCB(ComboBox cbox, string query,string cone)
        {
            MySqlConnection con = new MySqlConnection(cone);
            con.Open();
            MySqlCommand comando = new MySqlCommand(query);
            comando.Connection = con;
            MySqlDataAdapter da = new MySqlDataAdapter(comando);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cbox.DisplayMember = dt.Columns[1].ToString();
            cbox.ValueMember = dt.Columns[0].ToString();
            cbox.DataSource = dt;
            con.Close();
        }

       
    }
}
