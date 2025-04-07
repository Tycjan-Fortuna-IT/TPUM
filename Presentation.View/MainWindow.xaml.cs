using Client.Presentation.ViewModel;
using System.Diagnostics;
using System.Windows;

namespace Client.Presentation.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();
            this.DataContext = viewModel;

            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.DataContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}