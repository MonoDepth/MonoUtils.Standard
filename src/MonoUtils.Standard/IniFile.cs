using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace MonoUtilities.Files
{
    public class SectionAttribute : Attribute
    {
        public string Name { get; private set; }
        public SectionAttribute(string sectionName)
        {
            Name = sectionName;
        }
    }

    public class IniFile
    {
        /***
         * Ini read
        ***/

        public string FilePath { get; set; }
        private string lastSection = "General";

        public IniFile(string aFilePath)
        {
            FilePath = aFilePath;
        }

        public void LoadConfig()
        {
            LoadFromIni(FilePath);
        }

        public void SaveConfig()
        {
            SaveToIni(FilePath);
        }

        public void ResetToDefaults()
        {
            string[] constructorParams = { FilePath };
            Type[] typeParams = { typeof(string) };
            IniFile ini = GetType().GetConstructor(typeParams).Invoke(constructorParams) as IniFile;
            PropertyInfo[] settings = GetType().GetProperties().Where(prop => prop.PropertyType.IsPublic && prop.DeclaringType != typeof(IniFile)).ToArray();
            foreach (PropertyInfo setting in settings)
            {
                setting.SetValue(this, setting.GetValue(ini));
            }
        }

        protected void LoadFromIni(string iniPath)
        {
            PropertyInfo[] settings = GetType().GetProperties().Where(prop => prop.PropertyType.IsPublic && prop.DeclaringType != typeof(IniFile)).ToArray();
            foreach (PropertyInfo setting in settings)
            {
                string section = lastSection;
                var attributes = setting.GetCustomAttributes(false);
                var sectionMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(SectionAttribute));
                if (sectionMapping != null)
                {
                    var mapsto = sectionMapping as SectionAttribute;
                    section = mapsto.Name;
                    lastSection = section;
                }

                MethodInfo method = typeof(IniFile).GetMethod("IniReadGeneric").MakeGenericMethod(new[] { setting.PropertyType });
                object[] param = { iniPath, section, setting.Name, setting.GetValue(this) };
                object iniValue = method.Invoke(null, param);
                setting.SetValue(this, iniValue);
            }

        }

        protected void SaveToIni(string iniPath)
        {
            PropertyInfo[] settings = GetType().GetProperties().Where(prop => prop.PropertyType.IsPublic && prop.DeclaringType != typeof(IniFile)).ToArray();

            foreach (PropertyInfo setting in settings)
            {
                string section = lastSection;
                var attributes = setting.GetCustomAttributes(false);
                var sectionMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(SectionAttribute));
                if (sectionMapping != null)
                {
                    var mapsto = sectionMapping as SectionAttribute;
                    section = mapsto.Name;
                    lastSection = section;
                }

                MethodInfo method = typeof(IniFile).GetMethod("IniWriteGeneric").MakeGenericMethod(new[] { setting.PropertyType });
                object[] param = { iniPath, section, setting.Name, setting.GetValue(this) };
                method.Invoke(null, param);

            }
        }

        //Static methods

        public static string IniReadString(string path, string sectionName, string settingName, string defaultValue)
        {
            try
            {
                using (StreamReader rdr = new StreamReader(path, Encoding.UTF8))
                {
                    string _section = "";
                    while ((_section = rdr.ReadLine()) != null)
                    {
                        if (_section.Length <= 0 || _section[0] == '#')
                            continue;
                        if (_section.Trim().ToUpper() == $"[{sectionName.ToUpper()}]")
                        {
                            break;
                        }
                    }

                    if (_section == "")
                        return defaultValue;

                    string _setting = "";
                    while ((_setting = rdr.ReadLine()) != null)
                    {
                        if (_setting.Length <= 0 || _setting[0] == '#')
                            continue;
                        if (_setting.Split('=')[0].Trim().ToUpper() == settingName.ToUpper())
                        {
                            _setting = _setting.Substring(_setting.IndexOf('=') + 1).Trim();
                            return _setting;
                        }


                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T IniReadGeneric<T>(string path, string sectionName, string settingName, T defaultValue = default(T))
        {
            CultureInfo orgCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                string _s = IniReadString(path, sectionName, settingName, "");
                T _t;
                if (_s == "")
                    return defaultValue;
                try
                {
                    _t = (T)Convert.ChangeType(_s, typeof(T));
                }
                catch
                {
                    _t = defaultValue;
                }
                return _t;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = orgCultureInfo;
            }
        }


        /***
         * Ini write
        ***/

        public static bool IniWriteString(string path, string sectionName, string settingName, string settingValue)
        {
            List<string> ini;
            if (File.Exists(path))
                ini = File.ReadAllLines(path).ToList();
            else
                ini = new List<string>();
            int currentLine = 0;

            string _section = "";
            for (int i = 0; i < ini.Count; i++)// ((_section = rdr.ReadLine()) != null)
            {
                if (ini[i].Trim().ToUpper() == $"[{sectionName.ToUpper()}]")
                {
                    if (ini[i].Length <= 0 || ini[i][0] == '#')
                        continue;
                    _section = ini[i];
                    currentLine = i + 1;
                    break;
                }
            }

            if (_section == "")
            {
                ini.Add($"[{sectionName}]");
                ini.Add($"{settingName}={settingValue}");
                ini.Add(Environment.NewLine);
                using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                {
                    foreach (string s in ini)
                        wrr.WriteLine(s);
                }

                return true;
            }
            else
            {
                string _setting = "";
                for (int i = currentLine; i < ini.Count; i++)
                {
                    _setting = ini[i];
                    if (_setting.Length <= 0 || _setting[0] == '#')
                        continue;
                    if (!_setting.Contains('='))
                        continue;
                    _setting = _setting.Substring(0, _setting.IndexOf('='));
                    if (_setting.ToUpper() == settingName.ToUpper())
                    {
                        using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                        {
                            //ini.RemoveAt(i);
                            ini[i] = $"{settingName}={settingValue}";
                            foreach (string s in ini)
                                wrr.WriteLine(s);
                        }
                        return true;
                    }
                    _setting = "";
                }
                if (_setting == "")
                {
                    ini.Insert(currentLine, $"{settingName}={settingValue}");
                    using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        foreach (string s in ini)
                            wrr.WriteLine(s);
                    }
                    return true;
                }
            }

            return false;
        }

        public static bool IniWriteGeneric<T>(string path, string sectionName, string settingName, T settingValue)
        {
            CultureInfo orgCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                bool success = IniWriteString(path, sectionName, settingName, settingValue.ToString());
                return success;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = orgCultureInfo;
            }
        }

        public static string GetOrCreateProgramAppdataFolder(string programName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/" + @programName;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
