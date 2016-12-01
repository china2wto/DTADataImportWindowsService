using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Threading;

namespace DTADataImport
{
    public class ProcessImport  
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ProcessImport));
        private bool isError = false;
        private DateTime startProcessDataTime = DateTime.Now;
        public void process()
        {
            LOGGER.Info("--------------------DTADataImport start!-----------------------");
            isError = false;
            //String processType = ConfigurationManager.AppSettings.Get("process_type");
            //if ("2".Equals(processType))
            //{
            //    process2();//rongtiliusu_result,lashen_report,lashen_data
            //}
            //else
            //{
            //    process1();
            //}
            String project_name = ConfigurationManager.AppSettings.Get("project_name");
            if ("rongtiliusu_result".Equals(project_name) || "lashen_report".Equals(project_name) || "lashen_data".Equals(project_name))
            {
                process2();
            }
            else
            {
                process1();
            }


            LOGGER.Info("--------------------DTADataImport end!-----------------------");
        }
        public void process1()
        {
            try
            {
                //Random ran = new Random();
                //int RandKey = ran.Next(1, 10);
                //LOGGER.Info("---will sleep :" + RandKey+ " seconds");
                //Thread.Sleep(RandKey * 1000);
                DataView accessDataView = this.getAccessDataView();
                this.insertIntoMysql(accessDataView);
                //写最后一次读取的时间
                writeLastDateTime(this.startProcessDataTime);
                
            }
            catch (Exception e)
            {
                LOGGER.Error(e.ToString());
            }
        }



        
        public void process2()
        {
            try
            {
                //从access中获取所有数据， 
                DataView accessDataView = this.getAccessDataView();
                //LOGGER.Info("start to getMysqlDataView()");
                //从mysql中获取数据
                DataSet mysqlDataSet = this.getMysqlDataView();
                //LOGGER.Info("start to compareTwoDataview()");
                //比对留下需要插入的数据
                DataView compareDateView = this.compareTwoDataview(accessDataView, mysqlDataSet);
                //插入数据
                this.insertIntoMysql(compareDateView);
                //写最后一次读取的时间
                writeLastDateTime(this.startProcessDataTime);
            }
            catch (Exception e) {
                LOGGER.Error(e.ToString());
            }
        }


        private DataView getAccessDataView()
        {
            DataView dataview = new DataView();
            DataSet mydataset = new DataSet();
            OleDbConnection conn = null;
            try
            {
                String[] WhereConditionName = Configuration.accessWhereConditionName.Replace(" ", "").Split(',');
                String[] WhereConditionValue = Configuration.accessWhereConditionValue.Split(',');// 不能去除日期和时间之间的空格
                //LOGGER.Info("WhereConditionValue=" + string.Join(",", WhereConditionValue));

                string accessFile = Loader.accessFile;
                conn = OleDBService.getAccessConn(accessFile);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                string sql = Configuration.accessSelectGrammar;
                for (int i = 0; i < WhereConditionValue.Length; i++)
                {
                    sql = sql.Replace("${" + WhereConditionName[i] + "}", WhereConditionValue[i]);
                }
                //sql = sql.Replace("${0}", Configuration.lastDate).Replace("${1}", Configuration.lastTime);
                LOGGER.Info(sql);
                this.startProcessDataTime = DateTime.Now;
                LOGGER.Info(startProcessDataTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                mydataset = new System.Data.DataSet();
                adapter.SelectCommand = new OleDbCommand(sql, conn);
                adapter.Fill(mydataset, "accesstable");

                dataview = new DataView(mydataset.Tables["accesstable"]);
                LOGGER.Info("---从access中获取到" + dataview.Count + "行数据----");
            }
            catch (Exception e)
            {
                isError = true;
                LOGGER.Error("数据库出错:" + e.Message);
            }
            finally
            {
                if(conn!=null) conn.Close();
            }

            //for (int i = 0; i < dataview.Count; i++)
            //{
            //    DataRowView r = dataview[i];//这是一个行对象

            //    for (int colIndex = 0; colIndex < dataview.Table.Columns.Count; colIndex++)
            //    {
            //LOGGER.Debug(r[colIndex].ToString());
            //    }
            //}
            return (dataview);
        }
        private DataSet getMysqlDataView()
        {
            try
            {
                string sql = Configuration.mysqlSelectAll;
                DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sql, null);
                return ds;
            }
            catch (Exception ex)
            {
                isError = true;
                LOGGER.Error(ex.ToString());
                return null;
            }
            
        }
        private DataView compareTwoDataview(DataView accessDataView, DataSet mysqlDataSet)
        {
            if (accessDataView == null || accessDataView.Table == null || mysqlDataSet == null || mysqlDataSet.Tables == null )
            {
                return null;
            }
            String [] accessCompareColumn = Configuration.accessCompareColumn.Replace(" ", "").Split(',');
            String[] accessCompareColumnType = Configuration.accessCompareColumnType.Replace(" ", "").Split(',');
            String [] mysqlCompareColumn = Configuration.mysqlCompareColumn.Replace(" ", "").Split(',');
            String[] mysqlCompareColumnType = Configuration.mysqlCompareColumnType.Replace(" ", "").Split(',');
            String accessCompareDateFormat = Configuration.accessCompareDateFormat;
            String mysqlCompareDateFormat = Configuration.mysqlCompareDateFormat;


            if (accessCompareColumn == null || accessCompareColumn.Length == 0)
            {
                LOGGER.Warn("配置信息中 access CompareColumn is null");
            }
            if (accessCompareColumnType == null || accessCompareColumnType.Length == 0)
            {
                LOGGER.Warn("配置信息中 access CompareColumnType is null");
            }
            if (mysqlCompareColumn == null || mysqlCompareColumn.Length == 0)
            {
                LOGGER.Warn("配置信息中 mysql CompareColumn is null");
            }
            if (mysqlCompareColumnType == null || mysqlCompareColumnType.Length == 0)
            {
                LOGGER.Warn("配置信息中 mysql CompareColumnType is null");
            }



            if (accessCompareDateFormat == null || "".Equals(accessCompareDateFormat))
            {
                LOGGER.Warn("配置信息中 access CompareDateFormat is null");
            }
            if (mysqlCompareDateFormat == null || "".Equals(mysqlCompareDateFormat))
            {
                LOGGER.Warn("配置信息中 mysql CompareDateFormat is null");
            }

            for (int i = accessDataView.Table.Rows.Count-1; i >=0; i--)
            {
                bool same = false;
                DataRow rowAccess = accessDataView.Table.Rows[i];
                //double mfrAccess = double.Parse(rowAccess["mfr"].ToString());
                //string timeAccess = rowAccess["time"].ToString();

                List<Object> accessCompareColumnValueList = new List<Object>();
                for (int k = 0; k < accessCompareColumn.Length; k++)
                {
                    string value = rowAccess[accessCompareColumn[k]].ToString();
                    LOGGER.Debug("rowAccess[accessCompareColumn[k]= " + value);
                    if (value == null) break;
                    if ("double".Equals(accessCompareColumnType[k]))
                    {
                        accessCompareColumnValueList.Add(double.Parse(value));
                    }
                    else if ("int".Equals(accessCompareColumnType[k]))
                    {
                        accessCompareColumnValueList.Add(int.Parse(value));
                    }
                    else if ("string".Equals(accessCompareColumnType[k]))
                    {
                        accessCompareColumnValueList.Add(value);
                    }
                    else if ("datetime".Equals(accessCompareColumnType[k]))
                    {
                        accessCompareColumnValueList.Add(DateTime.Parse(value).ToString(accessCompareDateFormat));
                    }
                    else
                    {
                        accessCompareColumnValueList.Add(value);
                    }
                }

                for (int j = 0; j < mysqlDataSet.Tables[0].Rows.Count; j++)
                {
                    DataRow rowMysql = mysqlDataSet.Tables[0].Rows[j];
                    List<Object> mysqlCompareColumnValueList = new List<Object>();
                    for (int m = 0; m < mysqlCompareColumn.Length; m++)
                    {
                        string value = rowMysql[mysqlCompareColumn[m]].ToString();
                        if (value == null) break;
                        //LOGGER.Debug("rowMysql[mysqlCompareColumn[m]= " + value);

                        if ("Double".Equals(mysqlCompareColumnType[m], StringComparison.InvariantCultureIgnoreCase))
                        {
                            try{
                                mysqlCompareColumnValueList.Add(double.Parse(value));
                            }
                            catch (Exception e)
                            {
                                LOGGER.Warn(e.ToString());
                            }
                        }
                        else if ("String".Equals(mysqlCompareColumnType[m], StringComparison.InvariantCultureIgnoreCase))
                        {
                                mysqlCompareColumnValueList.Add(value);
                        }
                        else if ("Int".Equals(mysqlCompareColumnType[m], StringComparison.InvariantCultureIgnoreCase))
                        {
                            try{
                                mysqlCompareColumnValueList.Add(int.Parse(value));
                            }
                            catch (Exception e)
                            {
                                LOGGER.Warn(e.ToString());
                            }
                        }
                        else if ("DateTime".Equals(mysqlCompareColumnType[m], StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                mysqlCompareColumnValueList.Add(DateTime.Parse(value).ToString(mysqlCompareDateFormat));
                            }
                            catch (Exception e) {
                                LOGGER.Warn(e.ToString());
                            }
                        }
                        else
                        {
                            mysqlCompareColumnValueList.Add(value);
                        }
                    }
                    bool flag = true;
                    for (int a = 0; a < accessCompareColumnValueList.Count; a++)
                    {

                        LOGGER.Debug("accessCompareColumnValueList[a]  :  " + accessCompareColumnValueList[a]);
                        LOGGER.Debug("mysqlCompareColumnValueList[a]  :  " + mysqlCompareColumnValueList[a]);
                        if (accessCompareColumnValueList[a] != null 
                            && mysqlCompareColumnValueList[a] != null 
                            && !accessCompareColumnValueList[a].Equals(mysqlCompareColumnValueList[a]))
                        {
                            flag = false;//有一个字段两边对不上，就跳过。
                            break;
                        }
                        
                    }
                    if (flag)//所有字段都对上
                    {
                        same = true;//整条记录一致，
                        //LOGGER.Info("same-----------------"+ "mfrAccess= " + mfrAccess + "     timeAccess= " + timeAccess);
                        LOGGER.Debug("same");
                        break;//已经找到相同的，跳过和其他记录的比较
                    }
                    
                }
                if (same == true)
                {//相同则删除，保留不同的，后续进行保存。
                    
                    accessDataView.Table.Rows[i].Delete();
                    accessDataView.Table.AcceptChanges();
                }
            }
            LOGGER.Info("比较后，发现的新数据条数："+ accessDataView.Table.Rows.Count);
            return accessDataView;

        }

        private void insertIntoMysql(DataView accessDataView)
        {
            if (accessDataView == null || accessDataView.Table == null)
            {
                return;
            }
            using (MySqlConnection conn = new MySqlConnection(MySqlHelper.Conn))
            {
                conn.Open();
                MySqlTransaction tx = conn.BeginTransaction();
                try
                {
                    for (int i = 0; i < accessDataView.Table.Rows.Count; i++)
                    {
                        DataRow row = accessDataView.Table.Rows[i];
                        String[] columnsStr = Configuration.accessColumns.Replace(" ", "").Split(',');
                        String[] mysqlParameters = Configuration.mysqlParameters.Replace(" ", "").Split(',');
                        String[] mysqlParametersTypeStr = Configuration.mysqlParametersType.Replace(" ", "").Split(',');
                        String[] mysqlColumnsTypeLengthStr = Configuration.mysqlColumnsTypeLength.Replace(" ", "").Split(',');
                        //LOGGER.Info(string.Join(",",mysqlColumnsTypeLengthStr));
                        MySqlParameter[] parameters = new MySqlParameter[accessDataView.Table.Columns.Count];
                        int k = 0;
                        foreach (String parameterStr in mysqlParameters)
                        {
                            try
                            {
                                //LOGGER.Info("k="+k);
                                int columnsTypeLength = Int32.Parse(mysqlColumnsTypeLengthStr[k]);
                                MySqlParameter parameter = null;
                                if (columnsTypeLength == 0)
                                {
                                    parameter = new MySqlParameter("@" + parameterStr, getMySqlDbType(mysqlParametersTypeStr[k]));
                                }
                                else
                                {
                                    parameter = new MySqlParameter("@" + parameterStr, getMySqlDbType(mysqlParametersTypeStr[k]), columnsTypeLength);
                                }
                                parameter.Value = row[columnsStr[k]];
                                parameters[k] = parameter;
                            }
                            catch (Exception e)
                            {
                                LOGGER.Error(e.ToString());
                            }
                            LOGGER.Debug(parameterStr + "   : " + row[columnsStr[k]]);
                            k++;
                        }//end foreach
                        string sql = Configuration.mysqlSelectGrammar;
                        LOGGER.Info(sql);
                        //MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sql, parameters);
                        MySqlHelper.ExecuteNonQuery(tx, CommandType.Text, sql, parameters);
                    }//end for
                    tx.Commit();
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    LOGGER.Error(e.ToString());
                    isError =true;
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        private static MySqlDbType getMySqlDbType(String columnType)
        {
            if (columnType == null) return MySqlDbType.String;

            if ("String".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.String;
            }
            else if ("DateTime".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.DateTime;
            }
            else if ("Date".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Date;
            }
            else if ("Time".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Time;
            }
            else if ("Int32".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Int32;
            }
            else if ("Int".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Int32;
            }
            else if ("Double".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Double;
            }
            else if ("Text".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySqlDbType.Text;
            }
            else
            {
                return MySqlDbType.String;
            }

        }

        private void writeLastDateTime(DateTime dateTime)
        {
            if (!isError)
            {
                String[] LastDateFormatArray = Configuration.lastDateFormat.Split(',');//不能去除日期和时间之间的空格
                LOGGER.Info("LastDateFormatArray=" + string.Join(",", LastDateFormatArray));
                String value = "";
                for (int i = 0; i < LastDateFormatArray.Length; i++)
                {
                    value += dateTime.ToString(LastDateFormatArray[i], System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    if (i < LastDateFormatArray.Length - 1) value += ",";
                }
                Configuration.accessWhereConditionValue = value;
                LOGGER.Info("value=" + value);
            }
        }

        //public static void getMysqlList(DataView accessDataView)
        //{
        //    string sql = Configuration.mysqlSelectGrammar;
        //    LOGGER.Info(sql);
        //    DataSet mysqldataset = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sql, null);
        //    mysqldataset.Tables[0].Merge(accessDataView.Table);
        //    for (int i = 0; i < mysqldataset.Tables[0].Rows.Count; i++)
        //    {
        //        for (int j = 0; j < mysqldataset.Tables[0].Columns.Count; j++)
        //        {
        //            LOGGER.Info(mysqldataset.Tables[0].Rows[i]["name"]);
        //        }
        //    }

        //}


        //private DataView compareTwoDataview(DataView accessDataView, DataSet mysqlDataSet)
        //{
        //    if (accessDataView == null || accessDataView.Table == null || mysqlDataSet == null || mysqlDataSet.Tables == null)
        //    {
        //        return null;
        //    }
        //    String[] accessCompareColumn = Configuration.accessCompareColumn.Replace(" ", "").Split(',');
        //    String[] mysqlCompareColumn = Configuration.mysqlCompareColumn.Replace(" ", "").Split(',');
        //    for (int i = accessDataView.Table.Rows.Count - 1; i >= 0; i--)
        //    {
        //        bool same = false;
        //        DataRow rowAccess = accessDataView.Table.Rows[i];
        //        double mfrAccess = double.Parse(rowAccess["mfr"].ToString());
        //        string timeAccess = rowAccess["time"].ToString();
        //        LOGGER.Info("mfrAccess= " + mfrAccess + "     timeAccess= " + timeAccess);
        //        for (int j = 0; j < mysqlDataSet.Tables[0].Rows.Count; j++)
        //        {
        //            DataRow rowMysql = mysqlDataSet.Tables[0].Rows[j];
        //            double mfrMysql = double.Parse(rowMysql["mfr"].ToString());
        //            string timeMysql = rowMysql["time"].ToString();
        //            if (mfrAccess.Equals(mfrMysql) && timeAccess != null && timeAccess.Equals(timeMysql))
        //            {
        //                same = true;
        //                LOGGER.Info("same-----------------" + "mfrAccess= " + mfrAccess + "     timeAccess= " + timeAccess);
        //                break;
        //            }

        //        }
        //        if (same == true)
        //        {//相同则删除，保留不同的，后续进行保存。

        //            accessDataView.Table.Rows[i].Delete();
        //            accessDataView.Table.AcceptChanges();
        //        }
        //    }
        //    return accessDataView;

        //}
    }
}
