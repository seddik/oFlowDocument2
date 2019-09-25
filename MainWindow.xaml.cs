using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using OptimIT.Reports;
using Path = System.Windows.Shapes.Path;

namespace oFlowDocument2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaitWorker ww;
        public MainWindow()
        {
            InitializeComponent();

            b_open.Click += B_open_Click;
            ww = new WaitWorker(UpdatePreview, 1.0);
            tb_source.TextChanged += Tb_source_TextChanged;
            DispatcherTimer tt = new DispatcherTimer();
            tt.Interval = TimeSpan.FromSeconds(1.0);
            tt.Start();
            tt.Tick += delegate
            {
                ww.DoEvent();
            };
            PreviewKeyUp += MainWindow_PreviewKeyUp;
            tb_source.TextChanged += Tb_source_TextChanged1;
            try
            {
                var args = Environment.GetCommandLineArgs();
                var a = args[1];
                if (a != null)
                {
                    tb_source.Text = File.ReadAllText(a);
                    t_file.Text = a;
                    Title = "oFlowDocument2 : " + a;
                }
            }
            catch
            {

            }

        }


        private void Tb_source_TextChanged1(object sender, TextChangedEventArgs e)
        {
            ww.DoEvent();
        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                if (e.Key == Key.S)
                {
                    if (t_file.Text.IsEmpty())
                        return;
                    File.WriteAllText(t_file.Text, tb_source.Text);

                    UpdatePreview();
                }
        }

        private void B_open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    t_file.Text = ofd.FileName;
                    Title = "oFlowDocument2 : " + ofd.FileName;
                    tb_source.Text = "";
                    tb_source.IsEnabled = false;
                    StartWatching(t_file.Text);
                    UpdatePreview();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private FileSystemWatcher fs;
        private void StartWatching(string file)
        {
            string watchingFolder = System.IO.Path.GetDirectoryName(file);
            if (fs != null)
            {
                fs.Changed -= fs_Changed;
            }
            fs = new FileSystemWatcher(watchingFolder, file);
            fs.EnableRaisingEvents = true;
            fs.IncludeSubdirectories = true;
            fs.Changed += fs_Changed;
        }

        private void fs_Changed(object sender, FileSystemEventArgs e)
        {
            if (tb_source.IsEnabled)
                return;
            base.Dispatcher.BeginInvoke((Action)delegate
            {
                ww.DoEvent();
            });
        }


        private void Tb_source_TextChanged(object sender, TextChangedEventArgs e)
        {
            ww.DoEvent();
        }

        private bool bzoomed = false;
        private void UpdatePreview()
        {
            try
            {
                ReportDocument reportDocument = new ReportDocument();

                if (tb_source.Text.IsEmpty())
                {
                    reportDocument.XamlData = HandleVars(File.ReadAllText(t_file.Text));
                    reportDocument.XamlImagePath = System.IO.Path.GetDirectoryName(t_file.Text);
                }
                else
                {
                    reportDocument.XamlData = HandleVars(tb_source.Text);
                    reportDocument.XamlImagePath = Tools.GetLocalFile("");
                }
                DateTime dateTimeStart = DateTime.Now;
                List<ReportData> listData = new List<ReportData>();
                ReportData data = new ReportData();
                listData.Add(data);
                XpsDocument xps = reportDocument.CreateXpsDocument(listData);
                fdViewer.Document = xps.GetFixedDocumentSequence();
                t_error.Text = "";

            }
            catch (Exception ex)
            {
                t_error.Text = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            if (!bzoomed)
            {
                bzoomed = true;
                fdViewer.FitToHeight();
            }
        }

        private string HandleVars(string text)
        {
            var ss = text;
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var s in t_vars.Text.ToSplit(";"))
            {
                try
                {
                    dict.Add(s.ToSplit("$", 0), s.ToSplit("$", 1));
                }
                catch
                {
                }
            }

            foreach (var one in dict)
            {
                ss = ss.Replace("@@" + one.Key + "@@", one.Value);
            }

            return ss;
        }
    }
}
