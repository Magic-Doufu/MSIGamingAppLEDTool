using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Status_Function
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public string GetValue()
        {
            return Id;
        }
    }
    public partial class MainWindow : Window
    {
        public List<Status_Function> mode_list = new List<Status_Function> { };
        public Sync_Core sc;

        public MainWindow()
        {
            InitializeComponent();
            Process[] myProcesses = Process.GetProcessesByName("MSI_LED");
            if (myProcesses.Length < 1)
            {
                try
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "MSI_LED.exe");
                }
                catch (Exception)
                {
                    try
                    {
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI\\Gaming App\\MSI_LED.exe");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("MSI_LED Service can't not work! Please put the prorgram to \"Gaming App\" folder!");
                        this.Close();
                        return;
                    }
                }
            }
            mode_list.Add(new Status_Function { Name = "無效果", Id = "0" });
            mode_list.Add(new Status_Function { Name = "呼吸", Id = "1" });
            mode_list.Add(new Status_Function { Name = "閃爍", Id = "2" });
            mode_list.Add(new Status_Function { Name = "雙重閃爍", Id = "3" });
            mode_list.Add(new Status_Function { Name = "彩虹", Id = "7" });
            mode_list.Add(new Status_Function { Name = "閃電", Id = "8" });
            //mode_list.Add(new Status_Function { Name = "CPU溫度", Id = "10" });
            try
            {
                sc = new Sync_Core();
                //extend led
                LEDObject extLED = new LEDObject(sup_check("Extend LED,"), chb_extend, status_LED_extend);
                if (extLED.enable)
                {
                    extLED.sl_dark = ext_dark;
                    extLED.tb_dark = tb_ext_dark;
                    extLED.sl_hue = ext_hue;
                    extLED.tb_hue = tb_ext_hue;
                    extLED.wpc = color_ext_LED;
                    extLED.tab = tab_ExtLED;
                    extLED.cmb = ext_Mode;
                    extLED.handle_init();
                    sc.LEDobjs.Add(extLED);
                }
                //mystic led

                LEDObject mysticLED = new LEDObject(sup_check("Mystic Light,"), chb_mystic, status_LED_mystic);
                if (mysticLED.enable)
                {
                    mysticLED.sl_dark = mystic_dark;
                    mysticLED.tb_dark = tb_mystic_dark;
                    mysticLED.sl_hue = mystic_hue;
                    mysticLED.tb_hue = tb_mystic_hue;
                    mysticLED.wpc = color_mystic_LED;
                    mysticLED.tab = tab_MysticLED;
                    mysticLED.cmb = mystic_Mode;
                    mysticLED.handle_init();
                    sc.LEDobjs.Add(mysticLED);
                }

                LEDObject functionLED = new LEDObject(sup_check("Function LED,"), chb_function, status_LED_function);
                if (functionLED.enable)
                {
                    sc.LEDobjs.Add(functionLED);
                }
                /*
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
                }*/
            }
            catch (Exception)
            {
                return;
            }
        }
        private string[] sup_check(string pattern)
        {
            //偵測輸入類型
            foreach (string spec in sc.words)
                if (Regex.IsMatch(spec, pattern))
                {
                    string[] space = spec.Split(','); //拆字串
                    name_MB.Text = space[1];//根據cell位置分析
                    return space;
                }
            return null;
        }
    }
}
