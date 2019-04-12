using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace MSIExtendLED
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    /// 
    class Status_Function
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
    class ColorScale
    {
        public static Color ColorFromHSL(double h, double s, double l)
        {
            h = (360 - h) / 360;
            double r = 0, g = 0, b = 0;
            if (l != 0)
            {
                if (s == 0)
                    r = g = b = l;
                else
                {
                    double temp2;
                    if (l < 0.5)
                        temp2 = l * (1.0 + s);
                    else
                        temp2 = l + s - (l * s);

                    double temp1 = 2.0 * l - temp2;

                    r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, h);
                    b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                }
            }
            return Color.FromRgb((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));

        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }
    }
    public partial class MainWindow : Window
    {
        string raw_value = "";
        string[] words, ext_cells, mystic_cells, function_cells, gaming_cells;
        public MainWindow()
        {
            InitializeComponent();
            List<Status_Function> status = new List<Status_Function>{ };
            status.Add(new Status_Function { Name = "無效果", Id = "0" });
            status.Add(new Status_Function { Name = "呼吸", Id = "1" });
            status.Add(new Status_Function { Name = "閃爍", Id = "2" });
            status.Add(new Status_Function { Name = "雙重閃爍", Id = "3" });
            status.Add(new Status_Function { Name = "閃電", Id = "7" });
            status.Add(new Status_Function { Name = "彩虹", Id = "8" });
            status.Add(new Status_Function { Name = "CPU溫度", Id = "10" });
            raw_value = ReadRegistryKey("SOFTWARE\\WOW6432Node\\MSI\\GamingApp\\LED"); //直接給string的Registry路徑即可
            words = raw_value.Split('|');
            try
            {
                //extend led
                ext_cells = support_check("Extend LED,", chb_extend, status_LED_extend);
                if (ext_cells != null)
                {
                    ext_hue.Value = Double.Parse(ext_cells[8]);
                    ext_hue.ValueChanged += SLVC;
                    ext_dark.ValueChanged += SLVC;
                    ext_dark.Value = Double.Parse(ext_cells[9]);
                    tb_ext_hue.Text = ext_cells[8].ToString();
                    tb_ext_dark.Text = ext_cells[9].ToString();
                    color_ext_LED.Background = new SolidColorBrush(ColorScale.ColorFromHSL(Double.Parse(tb_ext_hue.Text), Double.Parse("1"), 1 - Double.Parse(tb_ext_dark.Text)));
                    chb_extend.Checked += Chb_extend_Checked;
                    chb_extend.Unchecked += Chb_extend_Checked;
                }
                else
                {
                    chb_extend.Visibility = Visibility.Hidden;
                    tab_ExtLED.Visibility = Visibility.Hidden;
                    status_LED_extend.Visibility = Visibility.Hidden;
                }
                //mystic led
                mystic_cells = support_check("Mystic Light,", chb_mystic, status_LED_mystic);
                if (mystic_cells != null)
                {
                    mystic_hue.Value = Double.Parse(mystic_cells[8]);
                    mystic_hue.ValueChanged += SLVC;
                    mystic_dark.ValueChanged += SLVC;
                    mystic_dark.Value = Double.Parse(mystic_cells[9]);
                    tb_mystic_hue.Text = mystic_cells[8].ToString();
                    tb_mystic_dark.Text = mystic_cells[9].ToString();
                    color_mystic_LED.Background = new SolidColorBrush(ColorScale.ColorFromHSL(Double.Parse(tb_mystic_hue.Text), Double.Parse("1"), 1 - Double.Parse(tb_mystic_dark.Text)));
                    chb_mystic.Checked += Chb_mystic_Checked;
                    chb_mystic.Unchecked += Chb_mystic_Checked;
                }
                else
                {
                    chb_mystic.Visibility = Visibility.Hidden;
                    tab_MysticLED.Visibility = Visibility.Hidden;
                    status_LED_mystic.Visibility = Visibility.Hidden;
                }
                function_cells = support_check("Function LED,", chb_function, status_LED_function);
                if (function_cells != null)
                {
                    chb_function.Checked += Chb_function_Checked;
                    chb_function.Unchecked += Chb_function_Checked;
                }
                else
                {
                    chb_function.Visibility = Visibility.Hidden;
                    tab_FunctionLED.Visibility = Visibility.Hidden;
                    status_LED_function.Visibility = Visibility.Hidden;
                }
                gaming_cells = support_check("Gaming LED,", chb_function, status_LED_function);
                if (gaming_cells != null)
                {
                    //chb_function.Checked += Chb_function_Checked;
                    //chb_function.Unchecked += Chb_function_Checked;
                }
                else
                {
                    chb_gaming.Visibility = Visibility.Hidden;
                    //tab_FunctionLED.Visibility = Visibility.Hidden;
                    status_LED_gaming.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string[] support_check(string pattern, CheckBox checkBox, TextBlock status)
        {
            //偵測輸入類型
            foreach (string spec in words)
                if (Regex.IsMatch(spec, pattern))
                {
                    string[] space = spec.Split(','); //拆字串
                    name_MB.Text = space[1];//根據cell位置分析
                    status.Text = " Hue: " + space[8] +
                        " Darkness: " + space[9] +
                        " Effect: " + space[4];
                    checkBox.IsChecked = (space[3] == "T") ? true : false;
                    Color color = Color.FromRgb(150, 0, 0);
                    return space;
                }
            return null;
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
        public void WriteRegistryKey(string RegKey, string ValueName, string data)
        {
            //讀取Registry Key位置
            RegistryKey RegK = Registry.LocalMachine.OpenSubKey(RegKey,true);
            RegK.CreateSubKey(ValueName);
            //讀取Registry Key String"test"裡面的值
            RegK.SetValue(ValueName, data);
        }

        private void Chb_extend_Checked(object sender, RoutedEventArgs e)
        {
            ext_cells[3] = ((bool)((CheckBox)sender).IsChecked) ? "T" : "F";
            Sync_to_registy("MB Extend LED");
        }
        private void Chb_mystic_Checked(object sender, RoutedEventArgs e)
        {
            mystic_cells[3] = ((bool)((CheckBox)sender).IsChecked) ? "T" : "F";
            Sync_to_registy("MB Mystic Light");
        }
        private void Chb_function_Checked(object sender, RoutedEventArgs e)
        {
            function_cells[3] = ((bool)((CheckBox)sender).IsChecked) ? "T" : "F";
            Sync_to_registy("MB Function LED");
        }

        private void SLVC(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (((Slider)sender).Tag)
            {
                case "ext":
                    tb_ext_hue.Text = ext_hue.Value.ToString();
                    tb_ext_dark.Text = ext_dark.Value.ToString();
                    color_ext_LED.Background = new SolidColorBrush(ColorScale.ColorFromHSL(Double.Parse(tb_ext_hue.Text), Double.Parse("1"), 1 - Double.Parse(tb_ext_dark.Text)));
                    ext_cells[8] = tb_ext_hue.Text;
                    ext_cells[9] = tb_ext_dark.Text;
                    break;
                case "mystic":
                    tb_mystic_hue.Text = mystic_hue.Value.ToString();
                    tb_mystic_dark.Text = mystic_dark.Value.ToString();
                    color_mystic_LED.Background = new SolidColorBrush(ColorScale.ColorFromHSL(Double.Parse(tb_mystic_hue.Text), Double.Parse("1"), 1 - Double.Parse(tb_mystic_dark.Text)));
                    mystic_cells[8] = tb_mystic_hue.Text;
                    mystic_cells[9] = tb_mystic_dark.Text;
                    break;
                default:
                    break;
            }
        }

        private void Sync_to_registy(String settype)
        {
            words[0] = settype;
            for (int i = 0; i < words.Length; i++)
            {
                if (Regex.IsMatch(words[i], "Extend LED,"))
                {
                    words[i] = string.Join(",", ext_cells);
                }
                if (Regex.IsMatch(words[i], "Mystic Light,"))
                {
                    words[i] = string.Join(",", mystic_cells);
                }
                if (Regex.IsMatch(words[i], "Function LED,"))
                {
                    words[i] = string.Join(",", function_cells);
                }
            }
            //MessageBox.Show(string.Join("|", words));
            WriteRegistryKey("SOFTWARE\\WOW6432Node\\MSI\\GamingApp\\LED","LEDSettings",string.Join("|", words)); //直接給string的Registry路徑即可
        }

        private void MLBU_Sync(object sender, MouseButtonEventArgs e)
        {
            switch (((Slider)sender).Tag)
            {
                case "ext":
                    Sync_to_registy("MB Extend LED");
                    break;
                case "mystic":
                    Sync_to_registy("MB Mystic Light");
                    break;
                default:
                    break;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((sender as TabControl).SelectedItem as TabItem).Tag.ToString() == "INFO")
                {
                    raw_value = ReadRegistryKey("SOFTWARE\\WOW6432Node\\MSI\\GamingApp\\LED"); //直接給string的Registry路徑即可
                    words = raw_value.Split('|');
                    ext_cells = support_check("Extend LED,", chb_extend, status_LED_extend);
                    mystic_cells = support_check("Mystic Light,", chb_mystic, status_LED_mystic);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
