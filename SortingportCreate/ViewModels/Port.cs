using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SortingportCreate.ViewModels
{
    public class Port : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private float startPoint { get; set; }
        public float StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                if (startPoint == value) return;
                startPoint = value;
                Notify("startPoint");
            }
        }

        private int portCount { get; set; }
        public int PortCount
        {
            get
            {
                return portCount;
            }
            set
            {
                if (portCount == value) return;
                portCount = value;
                Notify("portCount");
            }
        }

        private string exportSide { get; set; }
        public string ExportSide
        {
            get { return exportSide; }
            set
            {
                if (exportSide == value) return;
                exportSide = value;
                Notify("exportSide");
            }
        }
    }

    public class PortList : ObservableCollection<Port> { }
}
