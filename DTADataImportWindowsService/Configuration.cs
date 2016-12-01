using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
//using DTADataImport;

namespace DTADataImportWindowsService
{
    public class ServiceInstallLoad  
    {
        static string szCurrent = new FileInfo(typeof(ServiceInstallLoad).Assembly.Location).DirectoryName;//��ȡ��ǰ��Ŀ¼
        static string loader_ini = "/installservice.ini" ;

        public static string ServiceName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue(getInstallName(), "ServiceName");
            }

        }
        public static string DisplayName
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue(getInstallName(), "DisplayName");
            }

        }
        public static string Description
        {
            get
            {
                Ini.Instance.FilePath = szCurrent + loader_ini;
                return Ini.Instance.ReadValue(getInstallName(), "Description");
            }

        }

        public static String getInstallName()
        {
            Ini.Instance.FilePath = szCurrent + loader_ini;
            String project_name = Ini.Instance.ReadValue("common", "project_name");
            if (!"".Equals(project_name)) project_name = "_" + project_name;
            String install = "install" + project_name;
            Console.WriteLine("install :" + install);
            return install;
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
