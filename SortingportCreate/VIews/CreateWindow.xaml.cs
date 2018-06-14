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

namespace SortingportCreate.VIews
{
    /// <summary>
    /// CreateWindows.xaml 的交互逻辑
    /// </summary>
    public partial class CreateWindows : Window
    {
        public PortList ports { get; set; }
        public CreateWindows()
        {
            InitializeComponent();
            this.Closing += CreateWindows_Closing;
            ports = new PortList();
            this.listPort.ItemsSource = ports;
        }

        private void CreateWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
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
            if(port != null && IsAdd == true)
            {
                ports.Add(port);
            }
        }

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
                            for(int i=0; i < p.PortCount; i++)
                            {
                                sw.WriteLine(string.Format("{0},{1},{2},120,打开,{3},{4}", num, p.StartPoint + i * 1.25, p.ExportSide, num.ToString().PadLeft(3, '0'), num));
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
    }
}
