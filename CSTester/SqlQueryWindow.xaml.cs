using CSTester.CSEngine;
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
using Dapper;
using System.Dynamic;

namespace CSTester
{
    /// <summary>
    /// SqlQueryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SqlQueryWindow : Window
    {
        ISqlQuery query;
        bool jobDone = true;

        public SqlQueryWindow(ISqlQuery query)
        {
            InitializeComponent();
            this.query = query;
        }

        void BindItemsSource(IEnumerable<dynamic> list)
        {
            listGrid.Visibility = Visibility.Visible;
            lbResult.Visibility = Visibility.Collapsed;
            dynamic obj = list.FirstOrDefault();
            if (obj != null)
            {
                listGrid.Columns.Clear();
                var dict = (IDictionary<string, object>)obj;
                foreach (var key in dict.Keys)
                {
                    var column = new DataGridTextColumn();
                    column.Header = " " + key;
                    column.Binding = new Binding(key);
                    if (dict[key] is DateTime)
                    {
                        column.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss.fff";
                    }
                    column.IsReadOnly = true;
                    listGrid.Columns.Add(column);
                }
                listGrid.ItemsSource = list;
            }
            else
            {
                PrintResult(true, "未查询到数据");
            }
        }

        void PrintResult(bool success, string message)
        {
            listGrid.Visibility = Visibility.Collapsed;
            lbResult.Visibility = Visibility.Visible;
            lbResult.Foreground = Brushes.Black;

            if (!success)
            {
                lbResult.Foreground = Brushes.Red;
            }
            lbResult.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " --> " + message;
        }

        private void tbSql_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 && jobDone)
            {
                var sql = tbSql.Text.Trim();
                if (sql == "") return;
                jobDone = false;

                try
                {
                    if (gpResult.Visibility != Visibility.Visible)
                    {
                        gpResult.Visibility = Visibility.Visible;
                    }

                    if (sql.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var list = query.Query<dynamic>(sql);
                        BindItemsSource(list);
                    }
                    else if (sql.StartsWith("update", StringComparison.CurrentCultureIgnoreCase)
                       || sql.StartsWith("delete", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int effect = query.Execute(sql);
                        PrintResult(true, "(" + effect + " 行受影响)");
                    }
                    else
                    {
                        PrintResult(false, "不支持的语句 " + sql);
                    }
                }
                catch (Exception ex)
                {
                    PrintResult(false, ex.Message);
                }
                jobDone = true;
            }
        }
    }
}
