using System;
using System.Linq;
using System.Windows;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                var root = new CompositionRoot();
                var main = root.GetMainWindow();

                MainWindow = main;
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                // Close extra windows that may have gotten created due to DI container verifcation
                foreach (var window in Windows.OfType<Window>().Where(w => !ReferenceEquals(w, main)))
                {
                    window.Close();
                }

                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal Program Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}