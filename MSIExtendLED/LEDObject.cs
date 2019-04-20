using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public class LEDObject
{
    private string[] cells;
    public bool enable = true;
    private CheckBox chb;
    public TextBlock stat;
    public TextBlock tb_hue, tb_dark;
    public Slider sl_hue, sl_dark;
    public WrapPanel wpc;
    public TabItem tab;
    public ComboBox cmb;
    MSIExtendLED.MainWindow win = (MSIExtendLED.MainWindow)Application.Current.MainWindow;
    public LEDObject(string[] setting, CheckBox checkBox, TextBlock status)
    {
        if (setting != null)
        {
            cells = setting;
            stat = status;
            chb = checkBox;
            stat.Text = " Hue: " + cells[8] +
                " Darkness: " + cells[9] +
                " Effect: " + cells[4];
            chb.IsChecked = (cells[3] == "T") ? true : false;
            chb.Checked += Chb_Checked;
            chb.Unchecked += Chb_Checked;
        }
        else
        {
            enable = false;
            tab.Visibility = Visibility.Hidden;
            stat.Visibility = Visibility.Hidden;
            chb.Visibility = Visibility.Hidden;
        }
    }
    public void handle_init()
    {
        try
        {
            sl_dark.Value = this.Dark();
            sl_hue.Value = this.Hue();
            tb_dark.Text = cells[9];
            tb_hue.Text = cells[8];
            wpc.Background = new SolidColorBrush(ColorScale.ColorFromHSL(this.Hue(), 1, (1 - this.Dark()) > 0.5 ? 1 - this.Dark() : 0.5));
            sl_hue.ValueChanged += SLVC;
            sl_dark.ValueChanged += SLVC;
            sl_dark.PreviewMouseLeftButtonUp += MLBU_Sync;
            sl_hue.PreviewMouseLeftButtonUp += MLBU_Sync;
            cmb.ItemsSource = win.mode_list;
            cmb.SelectedIndex = int.Parse(cells[4]);
            cmb.SelectionChanged += ModeSC;
        }
        catch (Exception)
        {

            return;
        }
    }
    public Double Dark()
    {
        return enable ? Double.Parse(cells[9]) : 1 ;
    }
    public int Hue()
    {
        return enable ? int.Parse(cells[8]) : 0;
    }
    public void Dark(string input)
    {
        cells[9] = enable ? input : "";
    }
    public void Hue(string input)
    {
        cells[8] = enable ? input : "";
    }
    public string Serial()
    {
        return enable ? string.Join(",", cells) : "";
    }

    private void SLVC(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        tb_hue.Text = sl_hue.Value.ToString();
        tb_dark.Text = sl_dark.Value.ToString();
        cells[8] = tb_hue.Text;
        cells[9] = tb_dark.Text;
        wpc.Background = new SolidColorBrush(ColorScale.ColorFromHSL(this.Hue(), 1, (1 - this.Dark()) > 0.5 ? 1 - this.Dark() : 0.5));
    }
    private void Chb_Checked(object sender, RoutedEventArgs e)
    {
        cells[3] = ((bool)((CheckBox)sender).IsChecked) ? "T" : "F";
        win.sc.syncReg(((CheckBox)sender).Tag.ToString());
    }
    private void MLBU_Sync(object sender, MouseButtonEventArgs e)
    {
        win.sc.syncReg(((Slider)sender).Tag.ToString());
    }
    private void ModeSC(object sender, SelectionChangedEventArgs e)
    {
        cells[4] = (((ComboBox)sender).SelectedItem as MSIExtendLED.Status_Function).GetValue();
        win.sc.syncReg(((ComboBox)sender).Tag.ToString());
        //MessageBox.Show();

    }
}
