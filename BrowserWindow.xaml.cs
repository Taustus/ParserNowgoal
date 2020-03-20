using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telegram.Bot;

namespace ParserNowgoal
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        MainWindow _mw;
        public BrowserWindow(MainWindow mw)
        {
            InitializeComponent();
            _mw = mw;
            Cef.EnableHighDPISupport();
        }

        public async void Algorithm(object param)
        {
            var instanceAndCts = (List<object>)param;
            var instance = (MainWindow)instanceAndCts[0];
            var cts = (CancellationToken)instanceAndCts[1];

            string mainScript, api = "959846286:AAFvG7s6rS8zTUgMdWn6IA4NCE5-uVL6q8w";
            var ids = new List<string>();

            using (StreamReader r = new StreamReader(@"MainScript.js"))
            {
                mainScript = await r.ReadToEndAsync();
            }
            using (StreamReader sr = new StreamReader(@"IDs.txt"))
            {
                var temp = await sr.ReadToEndAsync();
                ids = temp.Split('\n').ToList();
            }
            TelegramBotClient botClient = new TelegramBotClient(api);
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    instance.UpdateStatus("Поиск матчей...");
                    Thread.Sleep(500);
                    var json = await EvaluateScript(mainScript);
                    var content = JsonConvert.DeserializeObject<List<Match>>(json);
                    instance.UpdateName(await EvaluateScript("var e = document.getElementById(\"CompanySel\");e.options[e.selectedIndex].innerText"));
                    for (int i = 0; i < content.Count; i++)
                    {
                        if (instance.AddRow(content[i]))
                        {
                            var temp = content[i];
                            /*foreach (var id in ids.Where(n => n.Length > 1))
                            {
                                await botClient.SendTextMessageAsync(chatId: id,
                                $"Команды: {temp.Team}\n" +
                                $"Параметры:\nHT = {temp.HT}\nX = {temp.X}\nJ = {temp.J}" +
                                $"\nY = {temp.Y}\nZ = {temp.Z}\nW = {temp.W}\n" +
                                $"Ссылка на событие: {temp.URL}");
                            }*/
                        }
                    }
                    instance.UpdateStatus("Ожидание...");
                    Thread.Sleep(1500);
                }
                catch
                {
                    Thread.Sleep(3000);
                }
            }
        }

        async Task<string> EvaluateScript(string script)
        {
            string toReturn = null;
            //We need to bu sure that V8 was fully loaded
            while (true)
            {
                if (ChromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    await ChromiumWebBrowser.EvaluateScriptAsync(script).ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            toReturn = response.Result.ToString();
                        }
                    });
                    break;
                }
                else Thread.Sleep(500);
            }

            if (toReturn == null)
            {
                toReturn = "fuck";
            }
            return toReturn;
        }

        private Task LoadPageAsync(string address = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler += (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    ChromiumWebBrowser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            ChromiumWebBrowser.LoadingStateChanged += handler;

            if (!string.IsNullOrEmpty(address))
            {
                ChromiumWebBrowser.Load(address);
            }
            return tcs.Task;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_mw.IsVisible)
            {
                e.Cancel = true;
            }
        }
    }
}
