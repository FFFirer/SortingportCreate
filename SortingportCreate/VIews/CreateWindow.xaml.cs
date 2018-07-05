using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SortingportCreate.ViewModels;
using System.IO;
using System.Configuration;
using SortingportCreate.Helpers;

namespace SortingportCreate.VIews
{
    /// <summary>
    /// CreateWindows.xaml 的交互逻辑
    /// </summary>
    public partial class CreateWindows : Window
    {
        public PortList ports { get; set; }
        //小车数，间隔光电
        public int CarNum { get; set; }
        public float IntervalGD { get; set; }
        public string MoreHandle { get; set; }
        public List<Setting> Settings { get; set; }
        public float Direction { get; set; }
        Setting DefaultSetting = new Setting()
        {
            VarName = "默认配置",
            VarValue = "1.25",
            VarHandle = "no"
        };
        public CreateWindows()
        {
            InitializeComponent();
            this.Closing += CreateWindows_Closing;
            ports = new PortList();
            this.listPort.ItemsSource = ports;
            //初始化参数
            Settings = new List<Setting>();
            //读取配置文件
            if (ConfigurationManager.AppSettings.AllKeys.Contains("mode"))
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string config = configuration.AppSettings.Settings["mode"].Value.ToString();
                var cfgs = config.Split(';');
                foreach (var cfg in cfgs)
                {
                    var cs = cfg.Split(',');
                    Setting setting = new Setting()
                    {
                        VarName = cs[0],
                        VarValue = cs[1],
                        VarHandle = cs[2]
                    };
                    Settings.Add(setting);
                }
            }
            else
            {
                ParamHandler.Save2Conf("mode", "默认配置,1.25,no");
                Settings.Add(DefaultSetting);
            }
            //下拉框绑定数据源
            cbSet.ItemsSource = Settings;
            cbSet.SelectedItem = Settings.Where(p => p.VarName.Equals("默认配置")).FirstOrDefault();
        }

        private void CreateWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //点击添加
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtCarNum.Text == "")
            {
                MessageBox.Show("请填写小车数！");
            }
            else
            {
                Port port;
                AddWindow aw = new AddWindow()
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    port = new Port(),
                };
                aw.port.ExportSide = "外";
                aw.ShowDialog();
                port = aw.port;
                bool IsAdd = aw.IsAdd;
                if (port != null && IsAdd == true)
                {
                    ports.Add(port);
                }
            }
        }

        //点击编辑
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Port port = (Port)listPort.SelectedItem;
            AddWindow aw = new AddWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                port = port
            };
            aw.ShowDialog();
            Port port2 = aw.port;
            if(port2 != port)
            {
                Port pex = ports.Where(p => p.Equals(port)).FirstOrDefault();
                pex = port2;
            }
        }

        //点击删除
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            Port port = (Port)listPort.SelectedItem;
            ports.Remove(port);
        }

        //将数据写入文件SortingPort.cvs.csv
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.FileName = "SortingPort.cvs";
            saveFileDialog.Filter = "CSV(逗号分隔)(.csv)|*.csv";
            System.Windows.Forms.DialogResult result = saveFileDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string content = "出口编号,光电位置,出口方向,缷载速度,状态,分拣码,IO点编号";
                    using(StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(content);
                        int num = 1;
                        foreach(var p in ports)
                        {
                            float NowPoint = p.StartPoint;
                            for(int i=0; i < p.PortCount; i++)
                            {
                                NowPoint = NowPoint + i * IntervalGD * Direction;
                                switch(MoreHandle)
                                {
                                    case "no":
                                        break;
                                    case "满六进一":
                                        //百世700mm下料口，600mm光电
                                        if (NowPoint % 1 == float.Parse("0.6"))
                                        {
                                            NowPoint = (float)((int)NowPoint + 1);
                                        }
                                        else if (NowPoint-float.Parse(((int)NowPoint).ToString()) == float.Parse("0.9"))
                                        {
                                            NowPoint -= float.Parse("0.4");
                                        }
                                        break;
                                }
                                if (NowPoint > CarNum && Direction == 1) NowPoint -= (CarNum * Direction);
                                else if (NowPoint < 0 && Direction == -1) NowPoint += (CarNum * Direction);
                                //sw.WriteLine(string.Format("{0},{1},{2},120,打开,{3},{4}", num, p.StartPoint + i * 1.25, p.ExportSide, num.ToString().PadLeft(3, '0'), num));
                                sw.WriteLine(string.Format("{0},{1},{2},120,打开,{3},{4}", num, NowPoint, p.ExportSide, num.ToString().PadLeft(3, '0'), num));
                                num++;
                            }
                        }
                    }
                    MessageBox.Show("分拣口配置已导出!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ports = new PortList();
        }

        private void cbSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setting setting = (Setting)cbSet.SelectedItem;
            MoreHandle = setting.VarHandle;
            IntervalGD = float.Parse(setting.VarValue);
            MessageBox.Show(string.Format("当前选择参数：\n参数方案：{0}\n间隔光电：{1}\n附加方法：{2}", setting.VarName, setting.VarValue, setting.VarHandle));
        }

        private void txtCarNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CarNum = int.Parse(txtCarNum.Text);
            }
            catch
            {
                MessageBox.Show("小车数必须为整数！");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ComboBoxItem item = (ComboBoxItem)cd.SelectedItem;
            //Direction = float.Parse(item.ToString());
        }

        private void cd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)cd.SelectedItem;
            Direction = float.Parse(item.Tag.ToString());
        }
    }
}
