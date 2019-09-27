using System;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace oFlowDocument2
{
    public class WaitWorker
    {
        private DispatcherTimer tt;

        public WaitWorker(Action action, double delay_sec = 0.5)
        {
            if (action != null)
            {
                tt = new DispatcherTimer();
                tt.Interval = TimeSpan.FromSeconds((delay_sec <= 0.0) ? 0.5 : delay_sec);
                tt.Tick += delegate
                {
                    tt.Stop();
                    action();
                };
            }
        }

        public void DoCancel()
        {
            tt.Stop();
        }

        public void DoEvent()
        {
            tt.Stop();
            tt.Start();
        }
    }

    public class oDocumentViewer : DocumentViewer
    {
        protected override void OnPrintCommand()
        {
            try
            {

                if (Document == null)
                    return;
 
                // get a print dialog, defaulted to default printer and default printer's preferences.
                PrintDialog printDialog = new PrintDialog();
                printDialog.ShowDialog();
                /*
                var server = new PrintServer();
                var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
                */
                /*
                                var temp_printer = PrinterName;
                                PrinterName = null;

                                var a = queues.FirstOrDefault(X => X.Name == temp_printer || X.FullName == temp_printer);
                                if (a == null)
                                {
                                    MessageBox.Show("L'imprimante" + " " + temp_printer + " " + "n'existe pas");
                                    return;
                                }

                                if (printDialog.PrintQueue.IsOffline)
                                {
                                    MessageBox.Show("L'imprimante" + " " + temp_printer + " " + "est hors connexion, vérifier votre cablage et sa mise en tension!");
                                    return;
                                }
                                */
                //     printDialog.PrintQueue = a;

                printDialog.PrintTicket.CopyCount = Count;
                
                if (bLandScape)
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;



                DocumentPaginator paginator = Document.DocumentPaginator;
                
                printDialog.PrintDocument(paginator, "Optim Core3 Print");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static string PrinterName { get; set; }
        public int Count { get; private set; } = 1;
        static bool bLandScape { get; set; }
        public void PrintWithParameters(string printerName = null, bool blandscape = false, int count = 1)
        {
            PrinterName = printerName;
            Count = count;
            bLandScape = blandscape;
            Print();
        }

    }
}