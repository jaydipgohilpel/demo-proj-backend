using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using WebGrease;
using log4net;
using LogManager = log4net.LogManager;

namespace demo_proj_backend.DataObject
{
    /// <summary>
    /// Class For Database Operation
    /// </summary>
    public class Connection
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Global SQL Connection
        /// </summary>
        public SqlConnection GblSqlConnection;
        /// <summary>
        /// Global Data Adapter
        /// </summary>
        public SqlDataAdapter GblDataAdapter = new SqlDataAdapter();
        /// <summary>
        /// Global Data Reader
        /// </summary>
        public SqlDataReader GblDataReader;
        /// <summary>
        /// Global Data Command
        /// </summary>
        public SqlCommand GblCommand = new SqlCommand();
        /// <summary>
        /// Hash Table For Assigning Parameter Names And Values
        /// </summary>
        public Hashtable HTParam = new Hashtable();
        /// <summary>
        ///Variable Use For Hash Table Parameter Count
        /// </summary>
        public Int32 IntParamCount = 0;

        /// <summary>
        /// Current Culture Info (US)
        /// </summary>
        public System.Globalization.CultureInfo CultureInfoUS = new System.Globalization.CultureInfo("en-US", false);

        public Connection()
        {
        }

        #region Password Encryption Utility

        /// <summary>
        /// Method for Encoding Or Decoding Given Password
        /// </summary>
        /// <param name="pStr">PassWord String </param>
        /// <param name="pStrToEncodeOrDecode"> String For Encode Or Decode [E-Encode,D-Decode]</param>
        /// <returns>String</returns>
        public string ENCODE_DECODE(string pStr, string pStrToEncodeOrDecode)
        {
            int IntPos;
            string StrPass;
            string StrECode;
            string StrDCode;
            char ChrSingle;

            StrECode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StrDCode = ")(*&^%$#@!";

            for (int IntLen = 1; IntLen <= 52; IntLen++)
            {
                StrDCode = StrDCode + (Char)(IntLen + 160);
            }

            StrPass = "";
            for (int IntCnt = 0; IntCnt <= pStr.Trim().Length - 1; IntCnt++)
            {
                ChrSingle = char.Parse(pStr.Substring(IntCnt, 1));
                if (pStrToEncodeOrDecode == "E")
                {
                    IntPos = StrECode.IndexOf(ChrSingle, 1);
                }
                else
                {
                    IntPos = StrDCode.IndexOf(ChrSingle, 1);
                }
                if (pStrToEncodeOrDecode == "E")
                {
                    StrPass = StrPass + StrDCode.Substring(IntPos, 1);
                }
                else
                {
                    StrPass = StrPass + StrECode.Substring(IntPos, 1);
                }
            }
            return StrPass;
        }

        internal void FillDataTable(string diamondConnectionString, string v1, SqlParameter[] sqlParameter, bool v2)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Connection Manipulation

        /// <summary>
        /// Checks if server connection is valid or not.
        /// </summary>
        /// <param name="pStrConnectionString"></param>
        /// <returns>True if connection is available</returns>
        public bool CheckConnection(string pStrConnectionString)
        {
            SqlConnection NewCon;
            bool Result = false;
            NewCon = new SqlConnection(pStrConnectionString);
            try
            {
                OpenConnection(NewCon);
                Result = true;
                CloseConnection(NewCon);
            }
            catch (Exception ex)
            {
                log.Error($"Exception in CheckConnection.\n pStrConnectionString: {pStrConnectionString}\n", ex);
                CloseConnection(NewCon);
                Result = false;
            }
            return Result;
        }

        /// <summary>
        /// Checks whether connection state is open.
        /// </summary>
        /// <param name="pConn">SqlConnection</param>
        /// <returns>True if open</returns>
        public bool IsOpen(SqlConnection pConn)
        {
            return pConn.State == ConnectionState.Open;
        }

        #region Open Connection

        /// <summary>
        /// Opens a connection of SQL server.
        /// </summary>
        /// <param name="pObjConnection">SqlConnection object</param>
        public void OpenConnection(SqlConnection pObjConnection)
        {
            if (pObjConnection.State == ConnectionState.Closed)
            {
                pObjConnection.Open();

            }
        }

        /// <summary>
        /// Opens a connection Of SQL server using given Connection string.
        /// </summary>
        /// <param name="pObjConnection">SqlConnection object</param>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pBlnForceOpen">Force connection to open</param>
        public void OpenConnection(SqlConnection pObjConnection, string pStrConnectionString, bool pBlnForceOpen)
        {
            CloseConnection(pObjConnection);
            if (pBlnForceOpen == true)
            {
                pObjConnection.Open();
            }
            if (pObjConnection.State == ConnectionState.Closed)
            {
                pObjConnection.Open();
            }
        }

        /// <summary>
        /// Opens a connection Of SQL server using credentials.
        /// </summary>
        /// <param name="pObjConnection">SqlConnection object</param>
        /// <param name="pStrServer">Server Name</param>
        /// <param name="pStrDBName">DataBase Name</param>
        /// <param name="pStrDBUser">DataBase User Name</param>
        /// <param name="pStrDBPass">DataBase Password</param>
        public void OCon(SqlConnection pObjConnection, string pStrServer, string pStrDBName, string pStrDBUser, string pStrDBPass)
        {
            CloseConnection(pObjConnection);
            pObjConnection.ConnectionString = $"Data Source={pStrServer}; Initial Catalog={pStrDBName}; User Id={pStrDBUser}; Password={pStrDBPass}; Persist Security Info=True;";
            pObjConnection.Open();
        }

        /// <summary>
        /// Opens a connection of SQL server using connection string.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <returns></returns>
        public SqlConnection OpenConnection(String pStrConnectionString)
        {
            SqlConnection NewCon;
            NewCon = new SqlConnection(pStrConnectionString);
            try
            {
                OpenConnection(NewCon);
                GblCommand.Connection = NewCon;
                return NewCon;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in OpenConnection.\n pStrConnectionString: {pStrConnectionString}\n", ex);
                CloseConnection(NewCon);
                throw new Exception("System is not able to connect the server. Kindly please check your connection string and the network connection.");
            }
        }

        #endregion Open Connection

        #region Close Connection

        /// <summary>
        /// Close global connection Of SQL Server.
        /// </summary>
        public void CloseConnection()
        {
            if (GblSqlConnection != null)
            {
                if (GblSqlConnection.State == ConnectionState.Open)
                {
                    GblSqlConnection.Close();
                    GblSqlConnection.Dispose();
                    GblSqlConnection = null;
                }
            }
            else
            {
                GblSqlConnection = null;
            }
        }

        /// <summary>
        /// Close Connection Of SQL Server.
        /// </summary>
        /// <param name="pObjConnection">SQLConnection object</param>
        public void CloseConnection(SqlConnection pObjConnection)
        {
            if (pObjConnection != null)
            {
                if (pObjConnection.State == ConnectionState.Open)
                {
                    pObjConnection.Close();
                    pObjConnection.Dispose();
                    pObjConnection = null;
                }
            }
            else
            {
                pObjConnection = null;
            }
        }

        #endregion Close Connection

        #endregion Connection Manipulation

        #region Parameters Manipulation

        /// <summary>
        /// Generate condition string of primary key columns of DataTable
        /// </summary>
        /// <param name="pdtTable">Data Table</param>
        /// <returns>string of condition</returns>
        private string GeneratePrimaryKeys(DataTable pdtTable)
        {
            string strCondition = "";
            foreach (DataColumn DataColumnPrimaryKey in pdtTable.PrimaryKey)
            {
                switch (DataColumnPrimaryKey.DataType.Name.ToLower())
                {
                    case "string":
                        strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = '" + pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original] + "' ";
                        break;
                    case "double":
                    case "decimal":
                    case "integer":
                    case "int32":
                    case "int64":
                    case "int16":
                        strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = " + pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original];
                        break;
                    case "datetime":
                        if (pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original].GetType().ToString() == "System.DBNull")
                        {
                            strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = Null ";
                        }
                        else
                        {
                            strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = '" + SqlDate(pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original].ToString()) + "'";
                        }
                        break;
                    case "boolean":

                        if (pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original].GetType().ToString() == "System.DBNull")
                        {
                            strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = Null ";
                        }
                        else
                        {
                            strCondition = strCondition + " And [" + DataColumnPrimaryKey.ColumnName + "] = " + Convert.ToInt32(pdtTable.Rows[0][DataColumnPrimaryKey.ColumnName, DataRowVersion.Original]);
                        }
                        break;
                }
            }
            return strCondition;
        }

        internal IDisposable ExecuteReader(object diamondConnectionString, string v1, SqlParameter[] sqlParameter, bool v2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and string value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pStrVal">Parameter Value</param>
        public void AddParameter(String pStrKey, String pStrVal)
        {
            HTParam.Add(pStrKey, pStrVal);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and image
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pObjImage">Parameter Image</param>
        public void AddParameter(String pStrKey, byte[] pObjImage)
        {
            HTParam.Add(pStrKey, pObjImage);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and DataTable
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pDtCustomTable">Parameter DataTable</param>
        public void AddParameter(String pStrKey, DataTable pDtCustomTable)
        {
            HTParam.Add(pStrKey, pDtCustomTable);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and short integer value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pInt16Val">Parameter Value</param>
        public void AddParameter(String pStrKey, Int16 pInt16Val)
        {
            HTParam.Add(pStrKey, pInt16Val);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and integer value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pIntVal">Parameter Value</param>
        public void AddParameter(String pStrKey, int? pIntVal)
        {
            if (pIntVal.HasValue)
            {
                AddParameter(pStrKey, pIntVal.Value);
            }
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and integer value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pIntVal">Parameter Value</param>
        public void AddParameter(String pStrKey, int pIntVal)
        {
            HTParam.Add(pStrKey, pIntVal);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and long integer value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pInt64Val">Parameter Value</param>
        public void AddParameter(String pStrKey, Int64 pInt64Val)
        {
            HTParam.Add(pStrKey, pInt64Val);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and double value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pDoubleVal">Parameter Value</param>
        public void AddParameter(String pStrKey, Double pDoubleVal)
        {
            HTParam.Add(pStrKey, pDoubleVal);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and decimal value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pDcmVal">Parameter Value</param>
        public void AddParameter(String pStrKey, Decimal pDcmVal)
        {
            HTParam.Add(pStrKey, pDcmVal);
            IntParamCount++;
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and boolean value
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pBoolVal">Parameter Value</param>
        public void AddParameter(String pStrKey, bool? pBoolVal)
        {
            if (pBoolVal.HasValue)
            {
                HTParam.Add(pStrKey, pBoolVal);
                IntParamCount++;
            }
        }

        /// <summary>
        /// Add Parameter To HashTable using Key and DateTime
        /// </summary>
        /// <param name="pStrKey">Parameter Name</param>
        /// <param name="pdtDateTime">Parameter DateTime</param>
        public void AddParameter(String pStrKey, DateTime? pdtDateTime)
        {
            if (pdtDateTime == null || (pdtDateTime.Value.Year > 1 && pdtDateTime.Value.Year < 1900))
            {
                HTParam.Add(pStrKey, DBNull.Value);
            }
            else
            {
                if (pdtDateTime.Value.ToShortDateString() == "01/01/0001")
                {
                    HTParam.Add(pStrKey, pdtDateTime.Value.ToString("hh:mm tt"));
                }
                else
                {
                    HTParam.Add(pStrKey, pdtDateTime.Value.ToString("MM/dd/yyyy hh:mm tt"));
                }
            }
            IntParamCount++;
        }

        /// <summary>
        /// Get array of parameters from HashTable
        /// </summary>
        /// <returns>Array of SqlParameter object</returns>
        public SqlParameter[] GetParameters()
        {
            Int16 IntI = 0;

            SqlParameter[] GetPara = new SqlParameter[HTParam.Count];
            foreach (DictionaryEntry DE in HTParam)
            {
                if (DE.Value == null || DE.Value == DBNull.Value)
                {
                    GetPara[IntI] = new SqlParameter("@" + DE.Key.ToString(), SqlDbType.DateTime);
                    GetPara[IntI].Value = DBNull.Value;
                }
                else if (DE.Value.GetType().Name.ToUpper() == "BYTE[]")
                {
                    GetPara[IntI] = new SqlParameter("@" + DE.Key.ToString(), SqlDbType.Image);
                    GetPara[IntI].Value = DE.Value;
                }
                else if (DE.Value.GetType().Name == "DBNull")
                {
                    GetPara[IntI] = new SqlParameter("@" + DE.Key.ToString(), SqlDbType.DateTime);
                    GetPara[IntI].Value = DBNull.Value;
                }
                else
                {
                    GetPara[IntI] = new SqlParameter("@" + DE.Key.ToString(), DE.Value.ToString());
                }

                IntI++;
            }

            Clear();
            return GetPara;
        }

        /// <summary>
        /// Clear All parameters from HashTable 
        /// </summary>
        public void Clear()
        {
            if (HTParam == null)
                HTParam = new Hashtable();
            HTParam.Clear();
        }

        #endregion Parameters Manipulation

        #region Fill Dataset using connection string

        /// <summary>
        /// Fill DataSet using connection string and stored procedure with DataTable name, parameters and primary keys.
        /// </summary>
        /// <param name="pStrConnectionString">Server Connection String </param>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pStrTableName">Name of DataTable</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">[Optional] SqlParameter Collection</param>
        /// <param name="pStrPrimaryKeys">[Optional] Comma separated Primary Keys</param>
        /// <param name="IsStoredProcedure">[Default is True] True if command text is stored procedure name</param>
        public void FillDataSet(String pStrConnectionString, DataSet pDataSet, string pStrTableName, string pStrCommandText, SqlParameter[] pParamList = null, string pStrPrimaryKeys = null, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);
            DataColumn[] DataColumnPrimaryKey;

            string strParameterString = string.Empty;
            int CountParam = pParamList == null ? 0 : pParamList.Length;
            GblCommand.Parameters.Clear();
            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataSet.Tables[pStrTableName] != null)
                {
                    pDataSet.Tables[pStrTableName].Rows.Clear();
                }
                GblDataAdapter.Fill(pDataSet, pStrTableName);
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataSet 1.\n pStrTableName: {pStrTableName}\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }
            if (!string.IsNullOrWhiteSpace(pStrPrimaryKeys))
            {
                string[] StrArray;
                StrArray = pStrPrimaryKeys.Split(',');
                DataColumnPrimaryKey = new DataColumn[StrArray.GetUpperBound(0) + 1];
                for (int IntCount = 0; IntCount <= StrArray.GetUpperBound(0); IntCount++)
                {
                    DataColumnPrimaryKey[IntCount] = pDataSet.Tables[pStrTableName].Columns[IntCount];
                }
                pDataSet.Tables[pStrTableName].PrimaryKey = DataColumnPrimaryKey;
            }
        }

        /// <summary> Fill Of DataSet with Stored Procedure(Dataset ,Procedure Name)
        /// Fill DataSet using connection string and stored procedure with parameters
        /// </summary>
        /// <param name="pStrConnectionString">Server Connection String </param>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">[Optional] SqlParameter Collection</param>
        /// <param name="IsStoredProcedure">[Default is True] True if command text is stored procedure name</param>
        public void FillDataSet(String pStrConnectionString, DataSet pDataSet, string pStrCommandText, SqlParameter[] pParamList = null, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);

            string strParameterString = string.Empty;
            int CountParam = pParamList == null ? 0 : pParamList.Length;
            GblCommand.Parameters.Clear();
            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataSet != null)
                {
                    pDataSet.Clear();
                }
                GblDataAdapter.Fill(pDataSet);
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataSet 2.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }
        }

        #endregion Fill Dataset

        #region Fill Dataset without server connection i.e. using global connection

        /// <summary>
        /// Fill DataSet using stored procedure with DataTable name, parameters and primary keys.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pStrTableName">Name of DataTable</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">[Optional] SqlParameter Collection</param>
        /// <param name="pStrPrimaryKeys">[Optional] Comma separated Primary Keys</param>
        /// <param name="IsStoredProcedure">[Default is True] True if command text is stored procedure name</param>
        public void FillDataSet(DataSet pDataSet, string pStrTableName, string pStrCommandText, SqlParameter[] pParamList = null, string pStrPrimaryKeys = null, bool IsStoredProcedure = true)
        {
            DataColumn[] DataColumnPrimaryKey;

            string strParameterString = string.Empty;
            int CountParam = pParamList == null ? 0 : pParamList.Length;
            GblCommand.Parameters.Clear();
            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = GblSqlConnection;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataSet.Tables[pStrTableName] != null)
                {
                    pDataSet.Tables[pStrTableName].Rows.Clear();
                }
                GblDataAdapter.Fill(pDataSet, pStrTableName);
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataSet 4.\n pStrTableName: {pStrTableName}\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
            }
            if (!string.IsNullOrWhiteSpace(pStrPrimaryKeys))
            {
                string[] StrArray;
                StrArray = pStrPrimaryKeys.Split(',');
                DataColumnPrimaryKey = new DataColumn[StrArray.GetUpperBound(0) + 1];
                for (int IntCount = 0; IntCount <= StrArray.GetUpperBound(0); IntCount++)
                {
                    DataColumnPrimaryKey[IntCount] = pDataSet.Tables[pStrTableName].Columns[IntCount];
                }
                pDataSet.Tables[pStrTableName].PrimaryKey = DataColumnPrimaryKey;
            }
        }

        /// <summary>
        /// Fill DataSet using stored procedure with DataTable name, parameters and primary keys.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">[Optional] SqlParameter Collection</param>
        /// <param name="pStrPrimaryKeys">[Optional] Comma separated Primary Keys</param>
        /// <param name="IsStoredProcedure">[Default is True] True if command text is stored procedure name</param>
        public void FillDataSet(DataSet pDataSet, string pStrCommandText, SqlParameter[] pParamList = null, bool IsStoredProcedure = true)
        {
            string strParameterString = string.Empty;
            int CountParam = pParamList == null ? 0 : pParamList.Length;
            GblCommand.Parameters.Clear();
            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = GblSqlConnection;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataSet != null)
                {
                    pDataSet.Clear();
                }
                GblDataAdapter.Fill(pDataSet);
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataSet 4.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
            }
        }

        #endregion

        #region Fill DataReader

        /// <summary>
        /// Fill DataTable using connection string and stored procedure with parameters.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pDataTable">DataTable</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="pStrPrimaryKeys">[Optional] Comma separated Primary Keys</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        public void FillDataTable(String pStrConnectionString, DataTable pDataTable, String pStrCommandText, SqlParameter[] pParamList, string pStrPrimaryKeys = null, bool IsStoredProcedure = true)
        {
            int CountParam = pParamList.Length;
            string strParameterString = string.Empty;

            SqlConnection pCon = OpenConnection(pStrConnectionString);
            DataColumn[] DataColumnPrimaryKey;

            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }
            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataTable != null)
                {
                    pDataTable.Rows.Clear();
                }
                GblDataAdapter.Fill(pDataTable);
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataTable 1.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }

            if (!string.IsNullOrWhiteSpace(pStrPrimaryKeys))
            {
                string[] StrArray;
                StrArray = pStrPrimaryKeys.Split(',');
                DataColumnPrimaryKey = new DataColumn[StrArray.GetUpperBound(0) + 1];
                for (int IntCount = 0; IntCount <= StrArray.GetUpperBound(0); IntCount++)
                {
                    DataColumnPrimaryKey[IntCount] = pDataTable.Columns[IntCount];
                }
                pDataTable.PrimaryKey = DataColumnPrimaryKey;
            }
        }

        /// <summary>
        /// Fill DataTable using connection string and stored procedure with primary keys.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pDataTable">DataTable</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pStrPrimaryKeys">[Optional] Comma separated Primary Keys</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        public void FillDataTable(String pStrConnectionString, DataTable pDataTable, string pStrCommandText, string pStrPrimaryKeys = null, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);
            DataColumn[] DataColumnPrimaryKey;

            GblCommand.CommandText = pStrCommandText;
            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.Connection = pCon;
            GblDataAdapter.SelectCommand = GblCommand;
            try
            {
                if (pDataTable != null)
                {
                    pDataTable.Rows.Clear();
                }
                GblDataAdapter.Fill(pDataTable);

                if (!string.IsNullOrWhiteSpace(pStrPrimaryKeys))
                {
                    string[] StrArray;
                    StrArray = pStrPrimaryKeys.Split(',');
                    DataColumnPrimaryKey = new DataColumn[StrArray.GetUpperBound(0) + 1];
                    for (int IntCount = 0; IntCount <= StrArray.GetUpperBound(0); IntCount++)
                    {
                        DataColumnPrimaryKey[IntCount] = pDataTable.Columns[IntCount];
                    }
                    pDataTable.PrimaryKey = DataColumnPrimaryKey;
                }
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in FillDataTable 2.\n pStrCommandText: {pStrCommandText}\n", ex);
                throw ex;
            }
            finally
            {
                CloseConnection(pCon);
            }
        }

        #endregion Fill DataReader

        #region Execute Reader

        /// <summary>
        /// Executer Store Procedure With Parameter List.
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>SqlDataReader With Record</returns>
        public SqlDataReader ExecuteReader(String pStrConnectionString, String pStrCommandText, SqlParameter[] pParamList = null, bool IsStoredProcedure = true)
        {
            GblSqlConnection = OpenConnection(pStrConnectionString);

            int CountParam = pParamList == null ? 0 : pParamList.Length;
            string strParameterString = string.Empty;

            GblCommand.Parameters.Clear();
            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }
            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = GblSqlConnection;
            try
            {
                return GblCommand.ExecuteReader();
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in ExecuteReader 1.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                return null;
            }
            finally
            {
                GblCommand.Parameters.Clear();
            }
        }

        /// <summary>
        /// Executer Store Procedure With Parameter List using global connection.
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>SqlDataReader With Record</returns>
        public SqlDataReader ExecuteReader(String pStrCommandText, SqlParameter[] pParamList, bool IsStoredProcedure = true)
        {
            int CountParam = pParamList.Length;
            string strParameterString = string.Empty;

            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }
            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = GblSqlConnection;
            try
            {
                return GblCommand.ExecuteReader();
            }
            catch (SqlException ex)
            {
                log.Error($"Exception in ExecuteReader 2.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                return null;
            }
            finally
            {
                GblCommand.Parameters.Clear();
            }
        }

        #endregion Execute Reader

        #region Execute Scaler

        /// <summary> Give String With (Store Procedure)
        /// Execute Store Procedure With Parameter List which returns the scaler string value.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>Number of Affected Record Or If error raise then return -1</returns>
        public string ExecuteScalar(String pStrConnectionString, String pStrCommandText, SqlParameter[] pParamList, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);

            String strReturn = "";
            int CountParam = pParamList.Length;
            string strParameterString = string.Empty;

            for (int i = 0; i < CountParam; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }
            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;
            try
            {
                strReturn = Convert.ToString(GblCommand.ExecuteScalar());
                return strReturn;
            }
            catch (SqlException ex)
            {
                if (ex.Number != 50000)
                    log.Error($"Exception in ExecuteScalar 1.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }
        }

        #endregion Execute Scaler

        #region Execute Non Query

        /// <summary> Execute NonQuery With No of Affected Record With(Store Procedure)
        /// Executes stored procedure with parameter list and return non query.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>Number of affected records</returns>
        public int ExecuteNonQuery(String pStrConnectionString, String pStrCommandText, SqlParameter[] pParamList, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);

            Int32 IntParams = pParamList.Length;
            string strParameterString = string.Empty;
            GblCommand.Parameters.Clear();

            for (int i = 0; i < pParamList.Length; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;

            try
            {
                return GblCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number != 50000)
                    log.Error($"Exception in ExecuteNonQuery 1.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }
        }

        /// <summary> Execute NonQuery With No of Affected Record With(Store Procedure)
        /// Executes stored procedure with parameter list with custom table type and return non query.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="pDtCustomTypeData">DataTable of data of customer type table</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>Number of affected records</returns>
        public int ExecuteNonQuery(String pStrConnectionString, String pStrCommandText, SqlParameter[] pParamList, DataTable pDtCustomTypeData, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);

            Int32 IntParams = pParamList.Length;
            string strParameterString = string.Empty;
            GblCommand.Parameters.Clear();

            for (int i = 0; i < pParamList.Length; i++)
            {
                if (pParamList[i] != null)
                {
                    if (pParamList[i].ToString() == "@TableVar" || pParamList[i].ToString() == "@SaleTable")
                    {
                        strParameterString += $"\n {pParamList[i].ToString()} = '{pDtCustomTypeData}'";
                        GblCommand.Parameters.AddWithValue(pParamList[i].ToString(), pDtCustomTypeData);
                    }
                    else
                    {
                        GblCommand.Parameters.Add(pParamList[i]);
                        strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                    }
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;

            try
            {
                return GblCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number != 50000)
                    log.Error($"Exception in ExecuteNonQuery 2.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
                CloseConnection(pCon);
            }
        }

        /// <summary> Execute NonQuery With No of Affected Record With(Store Procedure)
        /// Executes stored procedure with parameter list with custom table type and return non query.
        /// </summary>
        /// <param name="pStrConnectionString">Connection string</param>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>Number of affected records</returns>
        public int ExecuteNonQuery(String pStrConnectionString, String pStrCommandText, bool IsStoredProcedure = true)
        {
            SqlConnection pCon = OpenConnection(pStrConnectionString);
            GblCommand.CommandType = CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = pCon;
            try
            {
                return GblCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number != 50000)
                    log.Error($"Exception in ExecuteNonQuery 3.\n pStrCommandText: {pStrCommandText}\n", ex);
                throw ex;
            }
            finally
            {
                CloseConnection(pCon);
            }
        }

        /// <summary> Execute NonQuery With No of Affected Record With(Store Procedure)
        /// Executes stored procedure with parameter list using global connection and return non query.
        /// </summary>
        /// <param name="pStrCommandText">Query or Stored Procedure Name</param>
        /// <param name="pParamList">Array of parameters</param>
        /// <param name="IsStoredProcedure">True if command text is stored procedure name</param>
        /// <returns>Number of affected records</returns>
        public int ExecuteNonQuery(String pStrCommandText, SqlParameter[] pParamList, bool IsStoredProcedure = true)
        {
            Int32 IntParams = pParamList.Length;
            string strParameterString = string.Empty;
            GblCommand.Parameters.Clear();

            for (int i = 0; i < pParamList.Length; i++)
            {
                if (pParamList[i] != null)
                {
                    GblCommand.Parameters.Add(pParamList[i]);
                    strParameterString += $"\n {pParamList[i].ParameterName} = '{pParamList[i].SqlValue}'";
                }
            }

            GblCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            GblCommand.CommandText = pStrCommandText;
            GblCommand.Connection = GblSqlConnection;

            try
            {
                return GblCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number != 50000)
                    log.Error($"Exception in ExecuteNonQuery 4.\n pStrCommandText: {pStrCommandText}\n pParamList: {strParameterString}\n", ex);
                throw ex;
            }
            finally
            {
                GblCommand.Parameters.Clear();
            }
        }

        #endregion Execute Non Query

        #region DataReader methods

        /// <summary>
        /// Closes Open Data Reader
        /// </summary>
        /// <param name="pObjReader">SqlDataReader</param>
        public void CloseReader(SqlDataReader pObjReader)
        {
            if (pObjReader != null)
            {
                if (pObjReader.IsClosed == false)
                {
                    pObjReader.Close();
                }
            }
        }

        /// <summary>
        /// Checks whether SqlDataReader object has the rows.
        /// </summary>
        /// <param name="pObjReader">SqlDataReader object</param>
        /// <returns></returns>
        public bool HasRows(SqlDataReader pObjReader)
        {
            return pObjReader != null && pObjReader.HasRows;
        }

        #endregion Misc methods

        #region Utility

        /// <summary>
        /// Converts Date string to SQL "yyyy/MM/dd" date Format
        /// </summary>
        /// <param name="pStrDate">Date String</param>
        /// <returns>String</returns>
        private string SqlDate(string pStrDate)
        {
            if (pStrDate.Length == 0)
            {
                return "null";
            }
            else
            {
                return DateTime.Parse(pStrDate).ToString("yyyy/MM/dd");
            }
        }

        /// <summary>
        /// Converts DateTime string to SQL "hh:mm tt" time Format
        /// </summary>
        /// <param name="pStrTime">Time String</param>
        /// <returns>String</returns>
        private string SqlTime(string pStrTime)
        {
            if (pStrTime.Length == 0)
            {
                return "null";
            }
            else
            {
                return Convert.ToDateTime(pStrTime).ToString("hh:mm tt");
            }
        }

        #endregion
    }
}