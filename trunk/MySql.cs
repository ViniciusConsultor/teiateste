using System;
using System.Collections.Generic;
using System.Text;
using MySql;
using MySql.Data.MySqlClient;
using System.Data;

namespace uniBaterFrenteLoja
{
    class MySql
    {
        MySqlConnection objConn;
        MySqlCommand objCmd;
        MySqlDataAdapter objDA;
        DataSet objDS;

        #region "Métodos Públicos"
        /// <summary>
        /// Executa o Método ExecuteNonQuery e retorna o numero de linhas afetadas
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parametros"></param>
        /// <returns>Numero de linhas afetadas</returns>
        public int ExecuteNonQuery(string caminhoConexao, CommandType commandType, string commandText, MySqlParameter[] parametros)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText, parametros);

                objConn.Open();
                objCmd.Connection = objConn;

                return objCmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Executa o Método ExecuteNonQuery e retorna o numero de linhas afetadas
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns>Numero de linhas afetadas</returns>
        public int ExecuteNonQuery(string caminhoConexao, CommandType commandType, string commandText)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText);

                objConn.Open();
                objCmd.Connection = objConn;

                return objCmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataSet
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parametros"></param>
        /// <returns>DataSet com resultado da consulta</returns>
        public DataSet RetornaDataSet(string caminhoConexao, CommandType commandType, string commandText, MySqlParameter[] parametros)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText, parametros);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                objDS = new DataSet();
                objDA.Fill(objDS);

                return objDS;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataSet
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns>DataSet com resultado da consulta</returns>
        public DataSet RetornaDataSet(string caminhoConexao, CommandType commandType, string commandText)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                objDS = new DataSet();
                objDA.Fill(objDS);

                return objDS;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataTable
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parametros"></param>
        /// <returns>DataTable com resultado da consulta</returns>
        public DataTable RetornaDataTable(string caminhoConexao, CommandType commandType, string commandText, MySqlParameter[] parametros)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText, parametros);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                DataTable objDT = new DataTable();
                objDA.Fill(objDT);

                return objDT;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataTable
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns>DataTable com resultado da consulta</returns>
        public DataTable RetornaDataTable(string caminhoConexao, CommandType commandType, string commandText)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                DataTable objDT = new DataTable();
                objDA.Fill(objDT);

                return objDT;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataRow
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parametros"></param>
        /// <returns>DataRow com resultado da consulta</returns>
        public DataRow RetornaDataRow(string caminhoConexao, CommandType commandType, string commandText, MySqlParameter[] parametros)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText, parametros);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                DataTable objDT = new DataTable();
                objDA.Fill(objDT);

                return objDT.Rows[0];
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        /// <summary>
        /// Efetua uma consulta no banco de dados e retorna 1 DataRow
        /// </summary>
        /// <param name="caminhoConexao"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns>DataRow com resultado da consulta</returns>
        public DataRow RetornaDataRow(string caminhoConexao, CommandType commandType, string commandText)
        {
            try
            {
                preparaConexao(caminhoConexao, commandType, commandText);

                objConn.Open();
                objCmd.Connection = objConn;
                objDA = new MySqlDataAdapter();
                objDA.SelectCommand = objCmd;
                DataTable objDT = new DataTable();
                objDA.Fill(objDT);

                return objDT.Rows[0];
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

            finally
            {
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
            }
        }

        #endregion

        #region "Métodos Privados"
        private void preparaConexao(string caminhoConexao, CommandType commandType, string commandText, MySqlParameter[] parametros)
        {
            try
            {
                objConn = new MySqlConnection(caminhoConexao);
                objCmd = new MySqlCommand(commandText);
                objCmd.CommandType = commandType;
                objCmd.Parameters.AddRange(parametros);
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                objConn.Close();
            }
        }

        private void preparaConexao(string caminhoConexao, CommandType commandType, string commandText)
        {
            try
            {
                objConn = new MySqlConnection(caminhoConexao);
                objCmd = new MySqlCommand(commandText);
                objCmd.CommandType = commandType;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion
    }
}
