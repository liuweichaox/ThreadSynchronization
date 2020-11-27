using System.Windows;
using System.ComponentModel;

namespace BackgroundWorkerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 后台工作者
        /// </summary>
        private BackgroundWorker backgroundWorker;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,//允许修改报告进度
                WorkerSupportsCancellation = true//允许人工取消
            };
            //调用 RunWorkerAsync() 时发生。
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            //当后台操作已完成、被取消或引发异常时发生。
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            //调用 ReportProgress(Int32) 时发生。
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
        }
        private void startAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker.IsBusy != true)
            {
                // 开始异步操作。
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void cancelAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker.WorkerSupportsCancellation == true)
            {
                // 取消异步操作。
                backgroundWorker.CancelAsync();
            }
        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            resultLabel.Content = (e.ProgressPercentage.ToString() + "%");
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                resultLabel.Content = "Canceled!";
            }
            else if (e.Error != null)
            {
                resultLabel.Content = "Error: " + e.Error.Message;
            }
            else
            {
                resultLabel.Content = "Done!";
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; i <= 10; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //执行耗时的操作并报告进度。
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }
    }
}
