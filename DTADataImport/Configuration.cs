using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using DTADataImport;
using log4net;

namespace DTADataImport
{
    class ServiceInstallLoad
    {
        static string szCurrent = new FileInfo(typeof(ServiceInstallLoad).Assembly.Location).DirectoryName;//��ȡ��ǰ��Ŀ¼
        static string loader_ini = "/installservice.ini" ;

        public static string ServiceName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue("install", "ServiceName");
            }

        }
        public static string DisplayName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue("install", "DisplayName");
            }

        }
        public static string Description
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue("install", "Description");
            }

        }

    }
    class Loader
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ProcessImport));
        static string szCurrent = new FileInfo(typeof(Loader).Assembly.Location).DirectoryName;//��ȡ��ǰ��Ŀ¼
        static string loader_ini = "/" + ConfigurationManager.AppSettings.Get("loader_ini");
        public static string accessFile
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                String loader = getLoaderName();
                return Ini.Instance.ReadValue(loader, "access_file");
               
            }

        }
        public static string config_ini
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                String loader = getLoaderName();
                return Ini.Instance.ReadValue(loader, "config_ini");
            }

        }

        public static String getLoaderName()
        {
            String deployment = ConfigurationManager.AppSettings.Get("deployment");
            if ("product".Equals(deployment))
            {
                deployment = "";
            }
            String project_name = ConfigurationManager.AppSettings.Get("project_name");
            if (!"".Equals(project_name)) project_name = "_" + project_name;
            if (!"".Equals(deployment)) deployment = "_" + deployment;
            String loader = "loader" + project_name  + deployment;
            LOGGER.Info("loader name : " + loader);
            return loader;
        }
    }
    class Configuration
    {
        static string szCurrent = new FileInfo(typeof(Configuration).Assembly.Location).DirectoryName;//��ȡ��ǰ��Ŀ¼
        //static string config_ini_filename = @"/config_local_jingyeya_information.ini";
        //static string config_ini_filename = "/"+System.Configuration.ConfigurationSettings.AppSettings["config_ini"];
        static string config_ini_filename = "/" + Loader.config_ini;
        public static string accessColumns
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "columns");
            }
        }

        public static string accessWhereConditionName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "WhereConditionName");
            }
        }

        public static string accessWhereConditionValue
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "WhereConditionValue");
            }
            set
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                Ini.Instance.Write("access", "WhereConditionValue", value);
            }
        }

        public static string mysqlParametersType
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "ParametersType");
            }
        }

        public static string mysqlColumnsTypeLength
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "ParametersTypeLength");
            }
        }

        public static string lastDateColumnName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "LastDateColumnName");
            }
        }
        public static string lastDateFormat
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "LastDateFormat");
            }
        }
        public static string accessSelectGrammar 
        {
            get 
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "SelectGrammar");
            }
        }

        public static string accessCompareColumn
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "CompareColumn");
            }
        }

        public static string accessCompareDateFormat
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "CompareDateFormat");
            }
        }
        public static string accessCompareColumnType
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("access", "CompareColumnType");
            }
        }

        public static string mysqlSelectGrammar
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "SelectGrammar");
            }
        }

        public static string mysqlSelectAll
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "SelectAll");
            }
        }

        public static string mysqlParameters
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "Parameters");
            }
        }

        public static string mysqlCompareColumn
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "CompareColumn");
            }
        }
        public static string mysqlCompareColumnType
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "CompareColumnType");
            }
        }
        public static string mysqlCompareDateFormat
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + config_ini_filename;
                return Ini.Instance.ReadValue("mysql", "CompareDateFormat");
            }
        }
    }

    public class Ini
    {
        // ����INI�ļ���д�������� WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // ����INI�ļ��Ķ��������� GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        //ini�����ļ�·��
        private static string m_szPath = null;
        public string FilePath
        {
            get
            {
                return m_szPath;
            }
            set
            {
                m_szPath = value;
            }
        }

        private static Ini m_instance = null;
        public static Ini Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Ini();

                return m_instance;
            }
        }

        public Ini()
        {
        }

        public void Write(string section, string key, string value)
        {
            // section=���ýڣ�key=������value=��ֵ��path=·��
            WritePrivateProfileString(section, key, value, FilePath);

        }
        public string ReadValue(string section, string key)
        {

            // ÿ�δ�ini�ж�ȡ�����ֽ�
            System.Text.StringBuilder temp = new System.Text.StringBuilder(1024);

            // section=���ýڣ�key=������temp=���棬path=·��
            GetPrivateProfileString(section, key, "", temp, 1024, FilePath);

            return temp.ToString();

        }
    }
}
