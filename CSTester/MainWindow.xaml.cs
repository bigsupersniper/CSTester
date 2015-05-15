using CSTester.CSEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Task.Run(() =>
            {
                Console.Out.WriteLine(DateTime.Now.ToString() + " --> 开始载入脚本");

                btnReload.Dispatcher.Invoke(() => btnReload.IsEnabled = false);

                try
                {
                    ScriptBootstrap.Start();
                    BindComboBox();
                    btnReload.Dispatcher.Invoke(() => btnReload.IsEnabled = true);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
                finally
                {
                    Console.Out.WriteLine(DateTime.Now.ToString() + " --> 脚本载入完成");
                    btnRestart.Dispatcher.Invoke(() => btnRestart.IsEnabled = true);
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
            }
            else
            {
                cbbFunctionList.Items.Clear();
                cbbFunctionList.Items.Add(new { Name = "=========请选择========" });
            }

            cbbFunctionList.SelectedIndex = 0;
        }

        private void cbbFunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbFunctionList.SelectedIndex > 0)
            {
                btnExec.IsEnabled = true;

                var func = cbbFunctionList.SelectedItem as IFunction;
                if (func.Json != null)
                {
                    tbInput.Text = func.Json.ToString();
                }
            }
            else
            {
                tbInput.Clear();
                btnExec.IsEnabled = false;
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
            //clear output 
            tbOutput.Clear();

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

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            //关闭当前程序
            Application.Current.Shutdown();
            //打开新的程序
            string strAppFileName = Process.GetCurrentProcess().MainModule.FileName;
            Process thisMudule = new Process();
            thisMudule.StartInfo.FileName = strAppFileName;
            thisMudule.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            thisMudule.Start();
        }

    }
}
