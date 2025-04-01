using System.Windows;
using Presentation.ViewModel; // Need access to MainViewModel
using System; // For IDisposable

namespace Presentation.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            this.DataContext = viewModel;
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