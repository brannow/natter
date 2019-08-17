using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FBO.Classes
{
    public class IniFile
    {
        protected static string path;
        protected static string EXE = Assembly.GetExecutingAssembly().GetName().Name;
        protected static Dictionary<String, Dictionary<String, String>> sectionCache;

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        public static void Init(string INIPath)
        {
            path = new FileInfo(INIPath ?? EXE + ".ini").FullName.ToString();
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>bool</returns>
        public static bool ExistsIniFile()
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public static String IniReadValue(string Section, string Key, String fallback = "")
        {
            Dictionary<String, String> keyVal = ReadSection(Section);
            if (keyVal.TryGetValue(Key, out String s))
            {
                return s;
            }

            return fallback;

        }

        public static Dictionary<String, String> ReadSection(String section)
        {
            if (sectionCache == null)
            {
                sectionCache = new Dictionary<String, Dictionary<String, String>>();
            }

            if (sectionCache.TryGetValue(section, out Dictionary<String, String> s))
            {
                return s;
            }

            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(section, buffer, 2048, path);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            Dictionary<String, String> result = new Dictionary<String, String>();
            foreach (String entry in tmp)
            {
                String[] row = entry.Trim().Split('=');
                if (row.Length == 2)
                {
                    result.Add(row[0].Trim(), row[1].Trim());
                }
            }
            sectionCache.Add(section, result);
            return result;
        }
    }
}
