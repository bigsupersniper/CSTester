using CSTester.CSEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace CSTester
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeRequire();
        }

        void InitializeRequire()
        {
            //重定向控制台输出
            Console.SetOut(new TextWriterProvider(tbOutput));

            Task.Factory.StartNew(() =>
            {
                Console.Out.WriteLine(DateTime.Now.ToString() + " --> 开始载入脚本");

                try
                {
                    ScriptBootstrap.Start();
                    BindComboBox();
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
                finally
                {
                    Console.Out.WriteLine(DateTime.Now.ToString() + " --> 脚本载入完成");
                }
            });
        }

        void BindComboBox()
        {
            if (ScriptBootstrap.Scripts != null)
            {
                cbbScriptList.Dispatcher.Invoke(() =>
                {
                    cbbScriptList.Items.Clear();
                    cbbScriptList.Items.Add(new { Name = "=========请选择========" });

                    foreach (var item in ScriptBootstrap.Scripts)
                    {
                        cbbScriptList.Items.Add(item);
                    }

                    cbbScriptList.SelectedIndex = 0;
                });
            }
        }

        private void cbbScriptList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbScriptList.SelectedIndex > 0)
            {
                var items = cbbScriptList.SelectedItem as IScript;
                cbbFunctionList.Items.Clear();
                cbbFunctionList.Items.Add(new { Name = "=========请选择========" });

                if (items.Functions != null)
                {
                    foreach (var func in items.Functions)
                    {
                        cbbFunctionList.Items.Add(func);
                    }
                }

                cbbFunctionList.SelectedIndex = 0;
            }
            else
            {
                cbbFunctionList.Items.Clear();
            }
        }

        private void cbbFunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbFunctionList.SelectedIndex > 0)
            {
                var func = cbbFunctionList.SelectedItem as IFunction;
                if (func.Json != null)
                {
                    tbInput.Text = func.Json.ToString();
                }
            }
            else
            {
                tbInput.Clear();
            }
        }

        private void btnExec_Click(object sender, RoutedEventArgs e)
        {
            if (cbbFunctionList.SelectedIndex <= 0) return;
            if (string.IsNullOrEmpty(tbInput.Text)) return;

            try
            {
                var func = cbbFunctionList.SelectedItem as IFunction;
                if (func.Invoker != null)
                {
                    var json = JsonConvert.DeserializeObject<JObject>(tbInput.Text);
                    if (json == null)
                    {
                        json = new JObject();
                    }

                    func.Json = json;
                    func.Invoker();
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if (!ScriptBootstrap.Started) return;

            Task.Factory.StartNew(() =>
            {
                Console.Out.WriteLine(DateTime.Now.ToString() + " --> 开始重新载入脚本");

                try
                {
                    ScriptBootstrap.Restart();
                    BindComboBox();
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex);
                }
                finally
                {
                    Console.Out.WriteLine(DateTime.Now.ToString() + " --> 重新载入脚本完成");
                }
            });
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbOutput.Text))
            {
                tbOutput.Clear();
            }
        }

    }
}
