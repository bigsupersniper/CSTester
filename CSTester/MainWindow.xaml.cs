using CSTester.CSEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        IScriptBootstrap scriptBootstrap;

        void InitializeRequire()
        {
            //重定向控制台输出
            Console.SetOut(new TextWriterProvider(tbOutput));

            Task.Run(() =>
            {
                TextPrinter.WriteLine("开始载入脚本");
                btnReload.Dispatcher.Invoke(() => btnReload.IsEnabled = false);

                try
                {
                    scriptBootstrap = ScriptLoader.CreateBootstrap();
                    BindComboBox();
                    btnReload.Dispatcher.Invoke(() => btnReload.IsEnabled = true);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
                finally
                {
                    TextPrinter.WriteLine("脚本载入完成");
                    btnRestart.Dispatcher.Invoke(() => btnRestart.IsEnabled = true);
                }
            });
        }

        void BindComboBox()
        {
            if (scriptBootstrap.ScriptContexts != null)
            {
                cbbScriptList.Dispatcher.Invoke(() =>
                {
                    cbbScriptList.Items.Clear();
                    cbbScriptList.Items.Add(new { ScriptName = "=========请选择========" });

                    foreach (var item in scriptBootstrap.ScriptContexts)
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
                var items = cbbScriptList.SelectedItem as IScriptContext;
                cbbFunctionList.Items.Clear();
                cbbFunctionList.Items.Add(new { MethodName = "=========请选择========" });
                if (items.MethodContexts != null)
                {
                    foreach (var func in items.MethodContexts)
                    {
                        cbbFunctionList.Items.Add(func);
                    }
                }
            }
            else
            {
                cbbFunctionList.Items.Clear();
                cbbFunctionList.Items.Add(new { MethodName = "=========请选择========" });
            }

            cbbFunctionList.SelectedIndex = 0;
        }

        private void cbbFunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbFunctionList.SelectedIndex > 0)
            {
                btnExec.IsEnabled = true;

                var method = cbbFunctionList.SelectedItem as IMethodContext;
                if (method.Parameters != null)
                {
                    tbInput.Text = method.Parameters.ToString();
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

            try
            {
                var method = cbbFunctionList.SelectedItem as IMethodContext;
                if (method != null)
                {
                    var json = JsonConvert.DeserializeObject<JObject>(tbInput.Text);
                    if (json == null)
                    {
                        json = new JObject();
                    }
                    method.Parameters = json;
                    method.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            tbOutput.Clear();

            Task.Factory.StartNew(() =>
            {
                TextPrinter.WriteLine("开始重新载入脚本");

                try
                {
                    scriptBootstrap = ScriptLoader.CreateBootstrap(true);
                    BindComboBox();
                }
                catch (Exception ex)
                {
                    TextPrinter.WriteLine(ex);
                }
                finally
                {
                    TextPrinter.WriteLine("重新载入脚本完成");
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                btnExec_Click(this, new RoutedEventArgs());
            }
        }
    }
}
