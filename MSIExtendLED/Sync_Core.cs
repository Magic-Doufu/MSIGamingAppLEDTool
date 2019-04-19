using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public class Sync_Core
{
    public string[] words;
    public List<LEDObject> LEDobjs = new List<LEDObject> { };
    public Sync_Core()
    {
        words = ReadRegistryKey("SOFTWARE\\WOW6432Node\\MSI\\GamingApp\\LED").Split('|');
    }
    public void syncReg(String settype)
    {
        LEDObject[] objs = LEDobjs.ToArray();
        if (objs.Length > 0 && objs.Length == words.Length - 1)
        {
            words[0] = settype;
            int i = 1;
            try
            {
                foreach (LEDObject item in objs)
                {
                    words[i] = item.Serial();
                    i++;
                }
                WriteRegistryKey("SOFTWARE\\WOW6432Node\\MSI\\GamingApp\\LED", "LEDSettings", string.Join("|", words));
            }
            catch (Exception)
            {
                return;
            }
        }
        else
        {
            return;
        }
    }
    public string ReadRegistryKey(string RegKey)
    {
        //讀取Registry Key位置
        RegistryKey RegK = Registry.LocalMachine.OpenSubKey(RegKey);
        //讀取Registry Key String"test"裡面的值
        string RegT = (string)RegK.GetValue("LEDSettings");
        //Show Registry Key值，檢查讀取的值是否正確
        return RegT;
    }
    private void WriteRegistryKey(string RegKey, string ValueName, string data)
    {
        //For Debug
        //MessageBox.Show(string.Join("|", words));
        
        //讀取Registry Key位置
        RegistryKey RegK = Registry.LocalMachine.OpenSubKey(RegKey, true);
        RegK.CreateSubKey(ValueName);
        //讀取Registry Key String"test"裡面的值
        RegK.SetValue(ValueName, data);
    }
}
