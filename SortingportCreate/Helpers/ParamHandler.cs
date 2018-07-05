using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SortingportCreate.Helpers
{
    /// <summary>
    /// 操作程序配置文件
    /// </summary>
    public static class ParamHandler
    {
        //将一个对象保存进配置文件
        public static void Save2Conf(string propname, string value)
        {
            //如果没有就添加,有就修改
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(propname))
            {
                Configuration con = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                con.AppSettings.Settings.Add(propname, value);
                con.Save();
            }
            else
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfg.AppSettings.Settings[propname].Value = value;
                cfg.Save();
            }
        }
    }
}
